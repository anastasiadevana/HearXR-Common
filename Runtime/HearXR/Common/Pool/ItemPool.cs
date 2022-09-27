//------------------------------------------------------------------------------
// ItemPool - generic pooling solution.
//------------------------------------------------------------------------------
//
// MIT License
//
// Copyright (c) 2020 Anastasia Devana
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("HearXR.Common.Tests")]

namespace HearXR.Common.Pool
{
    /// <summary>
    /// Item pool is a utility class, which allows to create pools of arbitrary objects.
    /// The pool size is unlimited, and it will keep instantiating new items whenever there are no items available.
    /// </summary>
    /// <typeparam name="T">Type of the pool item.</typeparam>
    public class ItemPool<T> where T : Object
    {
        #region Delegates
        // TODO: Add a delegate for custom item instantiation.
        public delegate bool ItemToStealIndexFinder(in List<T> inUseItems, out int index);
        public delegate void PreClearPool(List<T> inUseItems, Queue<T> poolItems);
        #endregion
        
        #region Private Fields
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly List<T> _inUse = new List<T>();
        private readonly T _prefab;
        private readonly int _size;
        private readonly Transform _parent;
        #endregion
        
        #region Constructor
        /// <summary>
        /// Instantiate an item pool class.
        /// Preloading items in advance will cause a bigger performance hit at the start, but less performance load
        /// at runtime.
        /// </summary>
        /// <param name="prefab">Prefab to use for instantiating items.</param>
        /// <param name="size">(optional) The size of the pool. If zero, the pool will be of unlimited size.</param>
        /// <param name="preloadItems">(optional) Number of items to preload into the pool.</param>
        /// <param name="parentTransform">(optional) Parent transform to place the pool items under.</param>
        public ItemPool(T prefab, int size = 0, int preloadItems = 0, Transform parentTransform = null)
        {
            if (prefab == null)
            {
                throw new ArgumentException("HEAR_XR: POOL: Item Pool requires a valid item prefab.", nameof(prefab));
            }
            _prefab = prefab;

            if (size < 0)
            {
                Debug.LogWarning("HEAR_XR: POOL: Negative size limit passed into the Item Pool class. The pool set to unlimited.");
            }
            _size = Mathf.Max(0, size);

            if (preloadItems < 0)
            {
                Debug.LogWarning("HEAR_XR: POOL: Number of items to preload is negative. Item Pool will not preload any items.");
                preloadItems = 0;
            }
            
            if (_size > 0 && preloadItems > _size)
            {
                Debug.LogWarning("HEAR_XR: POOL: Number of items to preload was higher than pool size limit. Item Pool will only preload up to the size limit.");
                preloadItems = _size;
            }

            _parent = parentTransform;

            for (int i = 0; i < preloadItems; ++i)
            {
                _pool.Enqueue(CreateItem(false));
            }
        }
        #endregion
        
        #region Public Methods
        public bool TryGetItem(out T item, ItemToStealIndexFinder itemToStealIndexFinder = null)
        {
            //Debug.Log($"HEAR_XR: POOL: Try get item. IN USE {_inUse.Count} IN POOL {_pool.Count} TOTAL {GetTotalNumberOfItems()}");
            
            item = default;
            lock (_pool)
            {
                // If there are available items in the pool, just get one of those.
                if (_pool.Count > 0)
                {
                    item = GetItem();
                }
            
                // There are no available items, and this is a size-limited pool.
                else if (_size > 0)
                {
                    if (itemToStealIndexFinder == null)
                    {
                        itemToStealIndexFinder = DefaultItemToStealIndexFinder;
                    }
                
                    // If we haven't exceeded the total item count, create a new item, otherwise - steal it.
                    //item = ((_pool.Count + _inUse.Count) < _size) ? CreateItem() : StealItem(itemToStealIndexFinder);
                    if (_pool.Count + _inUse.Count < _size)
                    {
                        item = CreateItem();
                    }
                    else
                    {
                        StealItem(itemToStealIndexFinder, out item);
                    }
                }
            
                // There are no available items, but it's an unlimited pool, so just create a new one.
                else
                {
                    item = CreateItem();
                }
            }

            //Debug.Log("HEAR_XR: POOL: Items in pool: " + _pool.Count + " in use: " + _inUse.Count);
            
            //Debug.Log($"HEAR_XR: POOL: Attempted to get item. Available: {GetNumberOfAvailableItems()}. in use: {GetNumberOfInUseItems()} (total: {GetTotalNumberOfItems()})");
            
            return (item != default);
        }

        /// <summary>
        /// Return an item into the pool.
        /// </summary>
        /// <param name="item">Item to return.</param>
        public void ReturnItem(T item)
        {
            lock (_pool)
            {
                _inUse.Remove(item);
                _pool.Enqueue(item);   
            }

            OnPoolItemReturned(ref item);
            //Debug.Log($"HEAR_XR: POOL: Item Returned. Available: {GetNumberOfAvailableItems()}. in use: {GetNumberOfInUseItems()} (total: {GetTotalNumberOfItems()})");
        }

        public void Clear(PreClearPool preClearPool = null)
        {
            if (preClearPool != null)
            {
                preClearPool?.Invoke(_inUse, _pool);
            }
            
            _inUse.Clear();
            _pool.Clear();
            //Debug.Log($"HEAR_XR: POOL: Pool cleared. Available: {GetNumberOfAvailableItems()}. in use: {GetNumberOfInUseItems()} (total: {GetTotalNumberOfItems()})");
        }
        #endregion

        #region Private Methods
        private T CreateItem(bool forUse = true)
        {
           //Debug.Log($"HEAR_XR: POOL: Creating pool item.");
            T item = Object.Instantiate(_prefab, _parent);
            
            OnPoolItemCreated(ref item);
            
            if (forUse)
            {
                _inUse.Add(item);   
            }
            return item;
        }

        private T GetItem()
        {
            //Debug.Log($"HEAR_XR: POOL: Getting pool item.");
            T item = _pool.Dequeue();
            _inUse.Add(item);
            return item;
        }

        private bool StealItem(ItemToStealIndexFinder itemToStealIndexFinder, out T item)
        {
            //Debug.Log($"HEAR_XR: POOL: Stealing pool item.");
            bool haveIndex = itemToStealIndexFinder(in _inUse, out int itemToStealIndex);
            item = null;
            
            // Validate index.
            if (haveIndex && (itemToStealIndex < 0 || itemToStealIndex >= _inUse.Count))
            {
                Debug.LogWarning("HEAR_XR: POOL: Item Pool received an invalid index for stealing an item. Unable to steal.");
                haveIndex = false;
            }
            
            if (haveIndex)
            {
                item = _inUse[itemToStealIndex];
                OnPoolItemStolen(ref item);
                // Move this item to the end of the list.
                _inUse.RemoveAt(itemToStealIndex);
                _inUse.Add(item);
            }

            if (!haveIndex)
            {
                Debug.LogWarning("HEAR_XR: POOL: Unable to steal item. Consider increasing the pool size.");
            }
            
            return haveIndex;
        }

        /// <summary>
        /// Default implementation of StealItemIndexDelegate.
        /// It just returns the first available item.
        /// </summary>
        /// <param name="inUseItems">List of available items for stealing.</param>
        /// <param name="index">Index of the item to steal.</param>
        /// <returns>TRUE if index was found. FALSE otherwise</returns>
        private bool DefaultItemToStealIndexFinder(in List<T> inUseItems, out int index)
        {
            index = 0;
            return true;
        }
        #endregion
        
        #region Unit Test Methods
        internal int GetNumberOfAvailableItems()
        {
            return _pool.Count;
        }

        internal int GetNumberOfInUseItems()
        {
            return _inUse.Count;
        }

        internal int GetTotalNumberOfItems()
        {
            return GetNumberOfAvailableItems() + GetNumberOfInUseItems();
        }

        internal int GetSizeLimit()
        {
            return _size;
        }
        #endregion
        
        #region Virtual Methods
        protected virtual void OnPoolItemCreated(ref T item) {}

        protected virtual void OnPoolItemReturned(ref T item) {}
        
        protected virtual void OnPoolItemStolen(ref T item) {}
        #endregion
    }
    
    /// <summary>
    /// TypedItem pool extends ItemPool class, but pool items must implement IPoolItem.
    /// It will notify the pool items of various state changes.
    /// </summary>
    /// <typeparam name="T">Type of the pool item.</typeparam>
    public class TypedItemPool<T> : ItemPool<T> where T : Object, IPoolItem
    {
        #region Constructor
        public TypedItemPool(T prefab, int size = 0, int preloadItems = 0, Transform parentTransform = null) : 
            base(prefab, size, preloadItems, parentTransform) {}
        #endregion

        #region Override Virtual Methods
        protected override void OnPoolItemCreated(ref T item)
        {
            base.OnPoolItemCreated(ref item);
            item.PoolItemCreated();
        }

        protected override void OnPoolItemReturned(ref T item)
        {
            base.OnPoolItemReturned(ref item);
            item.PoolItemReturned();
        }

        protected override void OnPoolItemStolen(ref T item)
        {
            base.OnPoolItemStolen(ref item);
            item.PoolItemStolen();
        }
        #endregion
    }
}

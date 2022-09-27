using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using HearXR.Common.Pool;

namespace HearXR.Common.Pool.Tests
{
    /// <summary>
    /// Tests for HearXR.Common.ItemPool
    /// </summary>
    public class item_pool
    {
        #region Unlimited Pool Constructor
        [Test]
        public void unlimited_with_bad_prefab_returns_exception()
        {
            // Arrange
            GameObject itemPrefab = null;
            bool exceptionCaught = false;
            
            // Act
            try
            {
                ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab);
            }
            catch (System.ArgumentException e)
            {
                exceptionCaught = true;
            }
            
            // Assert
            Assert.True(exceptionCaught);
        }
        
        [Test]
        public void unlimited_with_no_preload_has_no_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab);
            
            // Assert
            Assert.Zero(itemPool.GetSizeLimit());
        }
        
        [Test]
        public void unlimited_with_no_preload_has_0_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab);
            
            // Assert
            Assert.Zero(itemPool.GetNumberOfAvailableItems());
        }
        
        [Test]
        public void unlimited_with_preload_3_has_3_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 0, 3);
            
            // Assert
            Assert.AreEqual(3, itemPool.GetNumberOfAvailableItems());
        }
        
        [Test]
        public void unlimited_with_preload_3_has_no_size_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 0, 3);
            
            // Assert
            Assert.Zero(itemPool.GetSizeLimit());
        }
        
        [Test]
        public void unlimited_with_preload_minus3_has_0_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 0, -3);
            
            // Assert
            Assert.AreEqual(0, itemPool.GetNumberOfAvailableItems());
        }
        #endregion
        
        #region Limited Pool Constructor
        [Test]
        public void limited_to_minus5_with_no_preload_has_0_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, -5, 0);
            
            // Assert
            Assert.Zero(itemPool.GetNumberOfAvailableItems());
        }
        
        [Test]
        public void limited_to_minus5_with_no_preload_has_no_size_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, -5, 0);
            
            // Assert
            Assert.Zero(itemPool.GetSizeLimit());
        }

        [Test]
        public void limited_to_10_with_no_preload_has_0_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 10, 0);
            
            // Assert
            Assert.Zero(itemPool.GetNumberOfAvailableItems());
        }
        
        [Test]
        public void limited_to_10_with_no_preload_has_10_size_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 10, 0);
            
            // Assert
            Assert.AreEqual(10, itemPool.GetSizeLimit());
        }
        
        [Test]
        public void limited_to_10_with_preload_3_has_3_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 10, 3);
            
            // Assert
            Assert.AreEqual(3, itemPool.GetNumberOfAvailableItems());
        }
        
        [Test]
        public void limited_to_10_with_preload_3_has_10_size_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 10, 3);
            
            // Assert
            Assert.AreEqual(10, itemPool.GetSizeLimit());
        }

        [Test]
        public void limited_to_10_with_preload_20_has_10_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 10, 20);
            
            // Assert
            Assert.AreEqual(10, itemPool.GetNumberOfAvailableItems());
        }
        
        [Test]
        public void limited_to_10_with_preload_20_has_10_size_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, 10, 20);
            
            // Assert
            Assert.AreEqual(10, itemPool.GetSizeLimit());
        }
        
        [Test]
        public void limited_to_minus5_with_preload_minus10_has_0_size_limit()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, -5, -10);
            
            // Assert
            Assert.AreEqual(0, itemPool.GetSizeLimit());
        }
        
        [Test]
        public void limited_to_minus5_with_preload_minus10_has_0_items()
        {
            // Arrange
            GameObject itemPrefab = new GameObject();
        
            // Act
            ItemPool<GameObject> itemPool = new ItemPool<GameObject>(itemPrefab, -5, -10);
            
            // Assert
            Assert.AreEqual(0, itemPool.GetNumberOfAvailableItems());
        }
        #endregion
        
        #region Limited Pool Get Item
        [Test]
        public void limited_no_preload_get_one_item_has_0_items_1_in_use()
        {
            // Arrange
            ItemPool<GameObject> itemPool = NewUnlimitedNoPreload();

            // Act
            bool success = itemPool.TryGetItem(out GameObject item);

            // Assert
            Assert.True(success);
            Assert.AreEqual(0, itemPool.GetNumberOfAvailableItems());
            Assert.AreEqual(1, itemPool.GetNumberOfInUseItems());
        }
        
        [Test]
        public void limited_no_items_available_has_0_available()
        {
            // Arrange
            ItemPool<GameObject> itemPool = NewLimited3Preload3();

            // Act
            itemPool.TryGetItem(out GameObject item1);
            itemPool.TryGetItem(out GameObject item2);
            itemPool.TryGetItem(out GameObject item3);

            // Assert
            Assert.AreEqual(0, itemPool.GetNumberOfAvailableItems());
            Assert.AreEqual(3, itemPool.GetNumberOfInUseItems());
        }
        
        [Test]
        public void limited_no_items_available_cannot_steal_item()
        {
            // Arrange
            ItemPool<GameObject> itemPool = NewLimited3Preload3();
            itemPool.TryGetItem(out GameObject item1);
            itemPool.TryGetItem(out GameObject item2);
            itemPool.TryGetItem(out GameObject item3);

            // Act
            bool success = itemPool.TryGetItem(out GameObject item4, FindItemToStealFalse);

            // Assert
            Assert.False(success);
        }
        
        [Test]
        public void limited_no_items_available_can_steal_first_item()
        {
            // Arrange
            ItemPool<GameObject> itemPool = NewLimited3Preload3();
            itemPool.TryGetItem(out GameObject item1);
            itemPool.TryGetItem(out GameObject item2);
            itemPool.TryGetItem(out GameObject item3);
            item1.name = "Item 1";
            item2.name = "Item 2";
            item3.name = "Item 3";

            // Act
            bool success = itemPool.TryGetItem(out GameObject item4, FindItemToSteal0True);

            // Assert
            Assert.True(success);
            Assert.AreEqual("Item 1", item4.name);
        }
        
        [Test]
        public void limited_no_items_available_can_steal_second_item()
        {
            // Arrange
            ItemPool<GameObject> itemPool = NewLimited3Preload3();
            itemPool.TryGetItem(out GameObject item1);
            itemPool.TryGetItem(out GameObject item2);
            itemPool.TryGetItem(out GameObject item3);
            item1.name = "Item 1";
            item2.name = "Item 2";
            item3.name = "Item 3";

            // Act
            bool success = itemPool.TryGetItem(out GameObject item4, FindItemToSteal1True);

            // Assert
            Assert.True(success);
            Assert.AreEqual("Item 2", item4.name);
        }
        #endregion
        
        #region Helper Methods
        private ItemPool<GameObject> NewUnlimitedNoPreload()
        {
            GameObject itemPrefab = new GameObject();
            return new ItemPool<GameObject>(itemPrefab, 0, 0);
        }
        
        private ItemPool<GameObject> NewLimited3Preload3()
        {
            GameObject itemPrefab = new GameObject();
            return new ItemPool<GameObject>(itemPrefab, 3, 3);
        }

        private bool FindItemToStealFalse(in List<GameObject> inUseItems, out int index)
        {
            index = -1;
            return false;
        }
        
        private bool FindItemToSteal0True(in List<GameObject> inUseItems, out int index)
        {
            index = 0;
            return true;
        }
        
        private bool FindItemToSteal1True(in List<GameObject> inUseItems, out int index)
        {
            index = 1;
            return true;
        }
        #endregion
    }
}

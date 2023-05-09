//------------------------------------------------------------------------------
// Smooth Follower
//------------------------------------------------------------------------------
//
// Copyright (c) 2022 Anastasia Devana
//
// May be used for reference purposes only. Contact author for any intended
// duplication or intended use beyond exploratory read, compilation, testing.
// No license or rights transfer implied from the open publication. This
// software is made available strictly on an "as is" basis without warranty of
// any kind, express or implied.
//------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace HearXR.Common
{
    public class SmoothFollower : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private Transform _objectToFollow;
        [SerializeField] private bool _followMainCamera;
        [SerializeField] private bool _lateUpdate;
        
        [Header("Position")] 
        public bool followPosition = true;
        public bool smoothFollowPosition;
        public float smoothPositionFollowSpeed = 5.0f;
        public bool progressivePositionFollow;

        [Header("Rotation")] public bool followRotation = true;
        public bool smoothFollowRotation;
        public float smoothRotationFollowSpeed = 1.0f;
        #endregion
        
        #region Properties
        public Transform ObjectToFollow
        {
            get => _objectToFollow;
            set
            {
                _objectToFollow = value;
                _hasObjectToFollow = (_objectToFollow != null);
            }
        }

        public bool HasObjectToFollow => _hasObjectToFollow;

        public bool Follow
        {
            get => _follow;
            set => _follow = value;
        }
        #endregion

        #region Private Fields
        private bool _hasObjectToFollow;
        private bool _follow = true;
        #endregion

        #region Init
        private void Awake()
        {
            _hasObjectToFollow = (_objectToFollow != null);
        }

        private void Start()
        {
            if (_followMainCamera)
            {
                _objectToFollow = Camera.main.transform;
                _hasObjectToFollow = true;
            } 
        }
        #endregion
        
        #region Loop
        private void Update()
        {
            if (!_lateUpdate)
            {
                DoUpdate();
            }
        }

        private void LateUpdate()
        {
            if (_lateUpdate)
            {
                DoUpdate();
            }
        }

        private void DoUpdate()
        {
            if (!_follow || !_hasObjectToFollow) return;
            
            // Since the target object can get destroyed when we least expect it, basically just always expect it.
            try
            {
                if (followPosition) FollowObjectPosition();
                if (followRotation) FollowObjectRotation();
            }
            catch (MissingReferenceException e)
            {
                _objectToFollow = null;
                _hasObjectToFollow = false;
                //Debug.Log(e);
                //throw;
                // Fail quietly
            }
        }
        #endregion
        
        #region Private Methods
        private void FollowObjectPosition()
        {
            try
            {
                var targetPosition = _objectToFollow.position;
                var currentPosition = transform.position;
                
                if (!smoothFollowPosition)
                {
                    transform.position = targetPosition;
                    return;
                }

                var followSpeed = smoothPositionFollowSpeed;
                if (progressivePositionFollow)
                {
                    followSpeed *= (targetPosition - currentPosition).sqrMagnitude;
                }
                
                transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * followSpeed);
            }
            catch (Exception e)
            {
                _objectToFollow = null;
                _hasObjectToFollow = false;
            }
        }

        private void FollowObjectRotation()
        {
            try
            {
                var targetRotation = _objectToFollow.rotation;
                transform.rotation = (smoothFollowRotation)
                    ? Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothRotationFollowSpeed)
                    : targetRotation;
            }
            catch (Exception e)
            {
                _objectToFollow = null;
                _hasObjectToFollow = false;
            }
        }
        #endregion
    }   
}

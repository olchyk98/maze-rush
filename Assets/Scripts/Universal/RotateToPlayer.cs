using System;
using UnityEngine;

namespace Universal
{
    [RequireComponent(typeof(PlayerFinder))]
    public class RotateToPlayer : MonoBehaviour
    {
        private Transform _targetedTransform;
        private Transform _transform;

        // This is important to have another variable indicating if script found a player object,
        // as it's extremely expensive to check the transform object for null on each frame.
        private bool _hasFoundPlayer;

        private void Start()
        {
            _transform = GetComponent<Transform>();

            var player = GetComponent<PlayerFinder>().FindPlayer();
            if(player != null)
            {
                _targetedTransform = player.GetComponent<Transform>();
                _hasFoundPlayer = true;
            }
        }

        private void FixedUpdate()
        {
            HandleLookTick();
        }

        private void HandleLookTick()
        {
            if (!_hasFoundPlayer) return;

            _transform.LookAt(_targetedTransform);
            _transform.rotation = Quaternion.LookRotation(_targetedTransform.forward);
        }
    }
}

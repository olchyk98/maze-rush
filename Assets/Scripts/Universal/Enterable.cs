using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universal
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(PlayerFinder))]
    public class Enterable : MonoBehaviour
    {
        private GameObject _targetEntity;
        public UnityAction OnEnter;

        private void Start ()
        {
            _targetEntity = GetComponent<PlayerFinder>().FindPlayer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!GameObject.ReferenceEquals(_targetEntity, other.gameObject)) return;
            OnEnter?.Invoke();
        }
    }
}


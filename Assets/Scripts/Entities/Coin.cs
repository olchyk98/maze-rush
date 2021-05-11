using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using Player;

namespace Entities
{
    [RequireComponent(typeof(PlayerFinder))]
    [RequireComponent(typeof(SoundManager))]
    public class Coin : MonoBehaviour
    {
        private PlayerMoneyBag _moneyBag;
        private GameObject _player;
        private SoundManager _sound;
        private Transform _transform;

        private void Start ()
        {
            _sound = GetComponent<SoundManager>();
            _transform = GetComponent<Transform>();
            _player = GetComponent<PlayerFinder>().FindPlayer();
            _moneyBag = _player.GetComponent<PlayerMoneyBag>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!System.Object.ReferenceEquals(_player, other.gameObject)) return;

            _moneyBag.PutCoin();
            var clip = _sound.PlayRandom("PickUp");

            RemoveModel();

            // Destroy the object once the sound is played.
            Destroy(gameObject, clip.length);
        }

        /// <summary>
        /// Removes visible coin model.
        /// Hides the coin, but keeps the internal routines
        /// running.
        /// </summary>
        private void RemoveModel ()
        {
            foreach(Transform obj in _transform)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}

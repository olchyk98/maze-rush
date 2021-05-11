using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerMoneyBag : MonoBehaviour
    {
        public int Coins { get; private set; }
        private static string CacheStorageKey = "PlayerCoins";

        private void Start()
        {
            Coins = PlayerPrefs.GetInt(CacheStorageKey, 0);
        }

        /// <summary>
        /// Adds a coin to the moneybag.
        /// Updates the local cache.
        /// </summary>
        /// <returns>
        /// Updated number of coins in the bag.
        /// </returns>
        public int PutCoin ()
        {
            Coins++;

            PlayerPrefs.SetInt(CacheStorageKey, Coins);
            return Coins;
        }
    }
}

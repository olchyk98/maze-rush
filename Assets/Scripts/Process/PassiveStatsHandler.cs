using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Process
{
    public class PassiveStatsHandler : MonoBehaviour
    {
        private void Awake ()
        {
            int coins = PlayerPrefs.GetInt("PlayerCoins", 0);
            int lives = PlayerPrefs.GetInt("PlayerLives", 0);

            if(lives <= 0)
            {
                PlayerPrefs.SetInt("PlayerLives", 5);
                PlayerPrefs.SetInt("PlayerCoins", 0);
            }

            if(coins <= 0)
            {
                PlayerPrefs.SetInt("PlayerCoins", 0);
            }
        }
    }

}

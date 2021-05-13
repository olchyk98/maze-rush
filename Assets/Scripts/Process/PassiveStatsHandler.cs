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
            int lifes = PlayerPrefs.GetInt("PlayerLifes", 0);

            if(lifes <= 0)
            {
                PlayerPrefs.SetInt("PlayerLifes", 5);
                PlayerPrefs.SetInt("PlayerCoins", 0);
            }

            if(coins <= 0)
            {
                PlayerPrefs.SetInt("PlayerCoins", 0);
            }
        }
    }

}

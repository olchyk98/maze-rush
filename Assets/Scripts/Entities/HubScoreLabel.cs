using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Entities
{
    public class HubScoreLabel : MonoBehaviour
    {
        private void Awake()
        {
            int coins = PlayerPrefs.GetInt("PlayerCoins", 0);
            GetComponent<TextMeshPro>().text = $"{coins}¢";
        }
    }
}

using UnityEngine;
using TMPro;

namespace Entities
{
    public class HubScoreLabel : MonoBehaviour
    {
        private void Start()
        {
            int coins = PlayerPrefs.GetInt("PlayerCoins", 0);
            int lives = PlayerPrefs.GetInt("PlayerLives", 0);

            GetComponent<TextMeshPro>().text = $"{coins}¢ | {lives}♥️";
        }
    }
}

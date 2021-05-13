using UnityEngine;
using TMPro;

namespace Entities
{
    public class HubScoreLabel : MonoBehaviour
    {
        private void Start()
        {
            int coins = PlayerPrefs.GetInt("PlayerCoins", 0);
            int lifes = PlayerPrefs.GetInt("PlayerLifes", 0);

            GetComponent<TextMeshPro>().text = $"{coins}¢ | {lifes}♥️";
        }
    }
}

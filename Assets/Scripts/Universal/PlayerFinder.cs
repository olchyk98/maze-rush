using UnityEngine;

namespace Universal
{
    public class PlayerFinder : MonoBehaviour
    {
        public GameObject FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            
            if (player == null)
            {
                Debug.LogWarning("Tried to find player object when the object it's not present in the scene");
                return null;
            }

            return player;
        }
    }
}

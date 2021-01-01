using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entities
{
    [RequireComponent(typeof(BoxCollider))]
    public class ExitPortal : MonoBehaviour
    {
        private void Start() {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                HandleExit();
            }
        }

        /// <summary>
        /// Resets the level and makes the mazeGeneration create a new environment. 
        /// </summary>
        private void HandleExit()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

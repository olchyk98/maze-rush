using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Universal;
using UnityEngine.SceneManagement;

namespace Portals
{
    [RequireComponent(typeof(Enterable))]
    public class MazeToHub : MonoBehaviour
    {
        private void Start() {
            GetComponent<Enterable>().OnEnter += () =>
            {
                SceneManager.LoadScene("HubScene");
            };
        }
    }

}

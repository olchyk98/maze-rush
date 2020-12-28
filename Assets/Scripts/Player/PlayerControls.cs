using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerControls : MonoBehaviour
    {
        public UnityAction<Vector3> OnMove;
        public UnityAction<Vector2> OnLook;

        private void FixedUpdate()
        {
            HandleMoveTick();
            HandleLookTick();
        }
        
        private void HandleMoveTick()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if (x != 0 || y != 0)
            {
                OnMove?.Invoke(new Vector3(x, 0, y));
            }
        }

        private void HandleLookTick()
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            if (x != 0 || y != 0)
            {
                OnLook?.Invoke(new Vector2(x, y));
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerControls : MonoBehaviour
    {
        [SerializeField] private LayerMask nearUseLayers;
        [Range(.25f, 10f)] [SerializeField] private float nearUseRange;

        private Transform _transform;

        private bool _isProtecting = false;

        public UnityAction<Vector3, bool> OnMove;
        public UnityAction<Vector2> OnLook;
        public UnityAction<bool> OnProtect;

        private void Start()
        {
            _transform = GetComponent<Transform>();
        }

        private void FixedUpdate()
        {
            HandleMoveTick();
            HandleLookTick();
            HandleNearUseTick();
        }

        private void HandleMoveTick()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            bool isShifting = Input.GetKey(KeyCode.LeftShift);

            if (x != 0 || y != 0)
            {
                OnMove?.Invoke(new Vector3(x, 0, y), isShifting);
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

        private void HandleNearUseTick()
        {
            bool isProtecting = Input.GetKey(KeyCode.E);

            if(isProtecting != _isProtecting) {
                _isProtecting = isProtecting;
                OnProtect?.Invoke(_isProtecting);
            }
        }
    }
}

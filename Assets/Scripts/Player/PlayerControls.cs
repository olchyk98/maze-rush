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
        
        public UnityAction<Vector3> OnMove;
        public UnityAction<Vector2> OnLook;
        public UnityAction<GameObject> OnNearUse;

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

        private void HandleNearUseTick()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var hasCollided = Physics.Raycast(
                    _transform.position, 
                    _transform.forward,
                    out var hit,
                    Mathf.Infinity,
                    nearUseLayers
                );

                if (hasCollided)
                {
                    OnNearUse?.Invoke(hit.collider.gameObject);
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerControls))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [Range(5f, 15f)] [SerializeField] private float _mouseSensitivity;
        [Range(5f, 30f)] [SerializeField] private float _movementSpeed;

        public UnityAction<Transform> OnStep;

        private Transform _bodyTransform;
        private Rigidbody _rb;
        private PlayerControls _playerControls;

        private float _rotationY;
        private const float SpeedMultiplier = 100f;

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Start()
        {
            _bodyTransform = GetComponent<Transform>();
            _rb = GetComponent<Rigidbody>();
            _playerControls = GetComponent<PlayerControls>();

            _playerControls.OnMove += HandleMove;
            _playerControls.OnLook += HandleLook;
        }

        private void OnDestroy()
        {
            _playerControls.OnMove -= HandleMove;
            _playerControls.OnLook -= HandleLook;
        }

        private void HandleMove(Vector3 direction, bool isShifting)
        {
            var verticalForce = _bodyTransform.forward * direction.z;
            var horizontalForce = _bodyTransform.right * direction.x;
            var force = (verticalForce + horizontalForce) * _movementSpeed;

            if (isShifting) force /= 3;

            _rb.AddForce(force, ForceMode.Impulse);

            if (!isShifting)
            {
                OnStep?.Invoke(_bodyTransform);
            }
        }

        private void HandleLook(Vector2 direction)
        {
            var horizontalRotation = _bodyTransform.rotation.eulerAngles;
            var nextY = _rotationY - direction.y * _mouseSensitivity * SpeedMultiplier * Time.deltaTime;

            _rotationY = Mathf.Clamp(nextY, -90f, 90f);
            horizontalRotation.y += direction.x * _mouseSensitivity * SpeedMultiplier * Time.deltaTime;

            // Apply rotation changes
            cameraTransform.localRotation = Quaternion.Euler(_rotationY, 0f, 0f);
            _bodyTransform.rotation = Quaternion.Euler(horizontalRotation);
        }

        public void ApplyHit(Transform sourceTransform, float attackEffect)
        {
            var force = Vector2.right * attackEffect;

            // TODO: Implement Game Over
            _rb.AddForceAtPosition(force, sourceTransform.position, ForceMode.VelocityChange);
        }
    }
}

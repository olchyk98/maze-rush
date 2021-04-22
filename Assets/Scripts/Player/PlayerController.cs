using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerControls))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Animator _torchAnimator;

        [Range(5f, 15f)] [SerializeField] private float _mouseSensitivity;
        [Range(5f, 30f)] [SerializeField] private float _movementSpeed;

        public UnityAction<Transform> OnStep;

        private Transform _bodyTransform;
        private Rigidbody _rb;
        private PlayerControls _playerControls;

        private float _rotationY;
        private bool _isProtecting = false;
        private const float SpeedMultiplier = 100f;

        private static int AnimationCameraDiesHash = Animator.StringToHash("PlayerCameraDies");

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
            _playerControls.OnProtect += HandleProtect;
        }

        private void OnDestroy()
        {
            _playerControls.OnMove -= HandleMove;
            _playerControls.OnLook -= HandleLook;
            _playerControls.OnProtect -= HandleProtect;
        }

        private void HandleMove(Vector3 direction, bool isShifting)
        {
            var verticalForce = _bodyTransform.forward * direction.z;
            var horizontalForce = _bodyTransform.right * direction.x;
            var force = (verticalForce + horizontalForce) * _movementSpeed;
            if(_isProtecting) force /= 7;
            else if (isShifting) force /= 3;

            _rb.AddForce(force, ForceMode.Impulse);

            if (!isShifting)
            {
                OnStep?.Invoke(_bodyTransform);
            }
        }

        private void HandleProtect(bool isProtecting)
        {
            _isProtecting = isProtecting;
            _torchAnimator.SetBool("hostIsUsing", isProtecting);
        }

        private void Update()
        {
            var totalVelocity = Mathf.Abs(_rb.velocity.x + _rb.velocity.z);
            _torchAnimator.SetFloat("hostVelocity", totalVelocity);
        }

        private void HandleLook(Vector2 direction)
        {
            var horizontalRotation = _bodyTransform.rotation.eulerAngles;
            var nextY = _rotationY - direction.y * _mouseSensitivity * SpeedMultiplier * Time.deltaTime;

            _rotationY = Mathf.Clamp(nextY, -90f, 90f);
            horizontalRotation.y += direction.x * _mouseSensitivity * SpeedMultiplier * Time.deltaTime;
            // Apply rotation changes
            _cameraTransform.localRotation = Quaternion.Euler(_rotationY, 0f, 0f);
            _bodyTransform.rotation = Quaternion.Euler(horizontalRotation);
        }

        /// <summary>
        /// Applies hit to the entity.
        /// </summary>
        /// <param name="sourceTransform">
        /// Transform of the attacking entity.
        /// </param>
        /// <param name="attackEffect">
        /// Punch strength of the attack. Affects
        /// force of the entity.
        /// </param>
        /// <returns>
        /// Boolean that represents if the entity was hit.
        /// May return false if the entity is dead or protecting itself.
        /// </returns>
        public bool ApplyHit(Transform sourceTransform, float attackEffect)
        {
            if (_isProtecting) return false;

            var force = Vector2.right * attackEffect;

            _rb.AddForceAtPosition(force, sourceTransform.position, ForceMode.VelocityChange);
            FireDeathAnimation();
            return true;
        }

        /// <summary>
        /// Inits the entity death animation.
        /// </summary>
        private void FireDeathAnimation()
        {
            var animator = _cameraTransform
                .gameObject
                .GetComponent<Animator>();

            animator.applyRootMotion = false;
            animator.Play(AnimationCameraDiesHash);
        }
    }
}

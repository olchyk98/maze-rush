﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Entities;
using Universal;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerControls))]
    [RequireComponent(typeof(SoundManager))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Animator _torchAnimator;
        [SerializeField] private TorchHandler _torch;

        [Range(5f, 15f)] [SerializeField] private float _mouseSensitivity;
        [Range(5f, 30f)] [SerializeField] private float _movementSpeed;

        public UnityAction<Transform> OnStep;

        private Transform _bodyTransform;
        private Rigidbody _rb;
        private PlayerControls _playerControls;
        private SoundManager _audio;

        private bool _isProtecting;
        private bool _isDead;
        private float _rotationY;
        private const float SpeedMultiplier = 100f;

        private static int AnimationCameraDiesHash = Animator.StringToHash("PlayerCameraDies");
        private static string RunAudioELF = "_RUNNING";
        private static string ShiftAudioELF = "_SHIFTING";

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
            _audio = GetComponent<SoundManager>();

            _playerControls.OnMove += HandleMove;
            _playerControls.OnLook += HandleLook;
            _playerControls.OnProtect += HandleProtect;

            _isDead = false;
            _isProtecting = false;
        }

        private void Update()
        {
            var totalVelocity = Mathf.Abs(_rb.velocity.x + _rb.velocity.z);
            if (_torchAnimator != default)
            {
                _torchAnimator.SetFloat("hostVelocity", totalVelocity);
            }

            if (totalVelocity <= 0.001f)
            {
                _audio.StopELF(RunAudioELF);
                _audio.StopELF(ShiftAudioELF);
            }
        }

        /// <summary>
        /// Entity movement handler.
        /// Controls rigidbody and speed logic
        /// in case to move the entity.
        /// </summary>
        /// <param name="direction">
        /// Direction info taken from the input buffer.
        /// </param>
        /// <param name="isShifting">
        /// Boolean that represents if the entity is shifting
        /// with no making noise.
        /// If set to true, handler will reduce entity's speed
        /// and prevent other entities from hearing it.
        /// </param>
        private void HandleMove(Vector3 direction, bool isShifting)
        {
            var verticalForce = _bodyTransform.forward * direction.z;
            var horizontalForce = _bodyTransform.right * direction.x;
            var force = (verticalForce + horizontalForce) * _movementSpeed;
            if (_isProtecting) force /= 7;
            else if (isShifting) force /= 3;

            _rb.AddForce(force, ForceMode.Impulse);

            if (!isShifting)
            {
                OnStep?.Invoke(_bodyTransform);

                _audio.StopELF(ShiftAudioELF);
                _audio.PlayELF(RunAudioELF, "Run");
                return;
            }

            _audio.PlayELF(ShiftAudioELF, "Shift");
            _audio.StopELF(RunAudioELF);
        }

        /// <summary>
        /// Protection Handler.
        /// Controls animation and changes behaviour state.
        /// </summary>
        /// <param name="isProtecting">
        /// Boolean that represents if the entity is currently
        /// using its hand to protect itself.
        /// </param>
        private void HandleProtect(bool isProtecting)
        {
            _isProtecting = isProtecting;
            _torchAnimator?.SetBool("hostIsUsing", isProtecting);

            if (isProtecting)
            {
                _torch?.Audio.PlayRandom("Use");
            }
        }

        /// <summary>
        /// Look handler.
        /// Controls the camera in case to give entity
        /// the ability to look around.
        /// </summary>
        /// <param name="direction">
        /// Input vector taken from the input buffer.
        /// </param>
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
            if (_isProtecting || _isDead) return false;

            var force = Vector2.right * attackEffect;

            _rb.AddForceAtPosition(force, sourceTransform.position, ForceMode.VelocityChange);

            _isDead = true;
            StartCoroutine(StartDying());
            return true;
        }

        private IEnumerator StartDying()
        {
            FireDeathAnimation();

            yield return new WaitForSeconds(3.5f);

            DecreaseLives();
            SceneManager.LoadScene("HubScene");
        }

        private void DecreaseLives()
        {
            int lives = PlayerPrefs.GetInt("PlayerLives");
            PlayerPrefs.SetInt("PlayerLives", lives - 1);
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

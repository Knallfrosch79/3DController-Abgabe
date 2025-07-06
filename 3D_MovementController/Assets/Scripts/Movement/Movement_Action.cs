using System;
using UnityEngine;
using DefaultNamespace.TargetSystem;

namespace DefaultNamespace.PlayerSystem
{

    [RequireComponent(typeof(Rigidbody))]
    public class Movement_Action : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float baseSpeed = 3f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float sprintMultiplier = 2f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpPower = 5f;
        [SerializeField] private float fallSpeedModifier = 1f;
        [SerializeField] private float jumpSpeedModifier = 1f;
        [SerializeField] private float groundCheckDistance = 1.1f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Camera Settings")]
        [Tooltip("Transform des Spielers für Rotation")]
        [SerializeField] private Transform playerModel;
        [Tooltip("Kamera-Transform für Blick- und Bewegungsrichtung")]
        [SerializeField] private Transform cameraTransform;

        [Header("Interactable Settings")]
        [SerializeField] TargetProvider TargetProvider;

        private Rigidbody _rb;
        private Transform _transform;

        private Vector2 _moveInput;
        private bool _jumpRequested;
        private float _currentSpeed;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _transform = transform;
            _currentSpeed = baseSpeed;
        }

        private void FixedUpdate()
        {
            HandleHorizontalMovement();
            HandleJump();
            UpdateVerticalMovement();
        }

        public void UpdateHorizontalMovement(Vector2 input)
        {
            _moveInput = input;
        }

        public void RequestJump()
        {
            _jumpRequested = true;
        }

        public void StartSprint()
        {
            _currentSpeed = baseSpeed * sprintMultiplier;
        }

        public void StopSprint()
        {
            _currentSpeed = baseSpeed;
        }

        private void HandleHorizontalMovement()
        {
            if (_moveInput.sqrMagnitude < 0.01f)
                return;

            // Align the player's transform with the camera's yaw angle
            float yaw = cameraTransform.eulerAngles.y;
            _transform.rotation = Quaternion.Euler(0f, yaw, 0f);

            // Determine local forward and right directions based on the player's transform
            Vector3 forward = _transform.forward;
            Vector3 right = _transform.right;

            // Calculate movement direction relative to the player's orientation
            Vector3 direction = (forward * _moveInput.y + right * _moveInput.x).normalized;
            Vector3 targetVelocity = direction * _currentSpeed;

            // Compute the change in velocity needed and apply it to the Rigidbody
            Vector3 currentVelocity = _rb.linearVelocity;
            Vector3 delta = new Vector3(targetVelocity.x - currentVelocity.x, 0f, targetVelocity.z - currentVelocity.z);
            delta = Vector3.ClampMagnitude(delta, maxSpeed);
            _rb.AddForce(delta, ForceMode.VelocityChange);
        }


        private void HandleJump()
        {
            if (_jumpRequested && IsGrounded())
            {
                Vector3 currentVelocity = _rb.linearVelocity;
                currentVelocity.y = jumpPower;
                _rb.linearVelocity = currentVelocity;
            }
            _jumpRequested = false;
        }

        private void UpdateVerticalMovement()
        {
            Vector3 currentVelocity = _rb.linearVelocity;
            if (currentVelocity.y < 0f)
                currentVelocity += Vector3.up * Physics.gravity.y * (fallSpeedModifier - 1f) * Time.fixedDeltaTime;
            else if (currentVelocity.y > 0f && !_jumpRequested)
                currentVelocity += Vector3.up * Physics.gravity.y * (jumpSpeedModifier - 1f) * Time.fixedDeltaTime;
            _rb.linearVelocity = currentVelocity;
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(_transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }
    }
}
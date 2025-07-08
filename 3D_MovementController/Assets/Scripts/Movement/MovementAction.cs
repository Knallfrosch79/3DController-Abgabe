using System;
using UnityEngine;
using DefaultNamespace.TargetSystem;
using UnityEngine.EventSystems;

namespace DefaultNamespace.PlayerSystem
{

    [RequireComponent(typeof(Rigidbody))]
    public class MovementAction : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float baseSpeed = 3f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 25f; 

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
        [SerializeField] private float rotationSpeed = 10f;  // Wie schnell sich dein Charakter zur Laufrichtung dreht


        [Header("Interactable Settings")]
        [SerializeField] TargetProvider TargetProvider;

        private Rigidbody _rb;
        private Transform _transform;

        private Vector3 moveDirection = Vector3.zero; // Direction in which the player is moving
        private Vector3 _moveInput;
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

        public void UpdateHorizontalMovement(Vector3 input)
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

        public void Move(Vector3 direction)
        {
            moveDirection = direction;
        }

        private void HandleHorizontalMovement()
        {
            // 1) Roh‑Input
            Vector3 rawInput = new Vector3(_moveInput.x, 0f, _moveInput.z);
            if (rawInput.sqrMagnitude < 0.01f)
                return;

            // 2) Mapping in lokalen Raum des Charakters
            Vector3 moveDir = transform.forward * rawInput.z + transform.right * rawInput.x;
            moveDir.y = 0f;
            moveDir.Normalize();

            // 3) Charakter-Model dreht sich in Bewegungsrichtung
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            playerModel.rotation = Quaternion.Slerp(
                playerModel.rotation,
                targetRot,
                rotationSpeed * Time.fixedDeltaTime
            );

            // 4) Gewünschte Geschwindigkeit
            Vector3 desiredVelocity = moveDir * _currentSpeed;

            // 5) Beschleunigung/Verzögerung (deine Logik)
            Vector3 currentVel = _rb.linearVelocity;
            currentVel.y = 0f;
            bool accelerating = Vector3.Dot(desiredVelocity, currentVel) > 0f;
            float rate = accelerating ? acceleration : deceleration;
            float maxDelta = rate * Time.fixedDeltaTime;

            Vector3 newVel = Vector3.MoveTowards(currentVel, desiredVelocity, maxDelta);
            if (newVel.magnitude > maxSpeed)
                newVel = newVel.normalized * maxSpeed;

            Vector3 delta = newVel - currentVel;
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
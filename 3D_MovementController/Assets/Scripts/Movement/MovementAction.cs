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

        [Header("Air Control")]
        [SerializeField] private float airControl = 0.3f;      // only 30% control in the air

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
        [SerializeField] private float rotationSpeed = 10f;  // How fast the player rotates towards the movement direction


        [Header("Interactable Settings")]
        [SerializeField] TargetProvider TargetProvider;

        private Rigidbody _rb;
        private Transform _transform;

        private Vector3 moveDirection = Vector3.zero; // Direction in which the player is moving
        private Vector3 _moveInput;
        private bool _jumpRequested;
        private float _currentSpeed;
        private Vector3 desiredVelocity;

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
            // 1) Raw input from player
            Vector3 rawInput = new Vector3(_moveInput.x, 0f, _moveInput.z);
            if (rawInput.sqrMagnitude < 0.01f)
                return;

            // 2) Convert input to world‐space direction
            Vector3 moveDir = transform.forward * rawInput.z + transform.right * rawInput.x;
            moveDir.y = 0f;
            moveDir.Normalize();

            // 3) Rotate character root only if move‑direction is in front
            // Rotate only if the player is NOT moving backwards
            if (_moveInput.z >= 0f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.fixedDeltaTime
                );
            }

            // 4) Check if the player is on the ground
            bool grounded = IsGrounded();

            // 5) Read current full velocity, then isolate horizontal component
            Vector3 currentVel = _rb.linearVelocity;
            Vector3 horizontalVel = new Vector3(currentVel.x, 0f, currentVel.z);

            if (grounded)
            {
                // — Ground movement: full control and acceleration —

                // Determine desired horizontal velocity
                Vector3 desiredVelocity = moveDir * _currentSpeed;

                // Decide whether to accelerate or decelerate
                bool accelerating = Vector3.Dot(desiredVelocity, horizontalVel) > 0f;
                float rate = accelerating ? acceleration : deceleration;
                float maxDelta = rate * Time.fixedDeltaTime;

                // Smoothly move current velocity toward desired velocity
                Vector3 newHoriz = Vector3.MoveTowards(horizontalVel, desiredVelocity, maxDelta);

                // Clamp to maximum ground speed
                if (newHoriz.magnitude > maxSpeed)
                    newHoriz = newHoriz.normalized * maxSpeed;

                // Apply the difference as an instantaneous velocity change
                Vector3 delta = newHoriz - horizontalVel;
                _rb.AddForce(delta, ForceMode.VelocityChange);
            }
            else
            {
                // — Air movement: dampened input impulse only —

                // Compute a small impulse proportional to baseSpeed and airControl
                Vector3 impulse = moveDir * (_currentSpeed * airControl * Time.fixedDeltaTime);

                // Apply only the input‑impulse, leave existing momentum untouched
                _rb.AddForce(impulse, ForceMode.VelocityChange);
            }
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

        public bool IsGrounded()
        {
            return Physics.Raycast(_transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }
    }
}
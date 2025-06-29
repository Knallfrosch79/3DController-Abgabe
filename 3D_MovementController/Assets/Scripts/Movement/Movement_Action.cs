using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement_Action : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float fallSpeedModifier = 1.5f;  // >1 = schneller fallen
    [SerializeField] private float jumpSpeedModifier = 0.5f;  // <1 = kürzerer Sprung
    [SerializeField] private float groundCheckDist = 1.1f;
    [SerializeField] private float sprintMultiplier = 2f; // Multiplier for sprint speed
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [Tooltip("Die Kamera, relativ zu der sich bewegt wird.")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private Vector2 moveInput;            // X = horizontal, Y = vor/zur�ck
    private bool jumpRequested;

    private void Awake() => rb = GetComponent<Rigidbody>();

    /*  ---- Physik - Loop ----
        Alles, was rb.velocity manipuliert, MUSS in FixedUpdate passieren  */
    private void FixedUpdate()
    {
        HandleHorizontal();
        HandleJump();
        HandleGravityModifiers();
    }

    // ---------- API für Player_Behaviour ----------
    public void UpdateHorizontalMovement(Vector2 dir) => moveInput = dir;
    public void RequestJump() => jumpRequested = true;
    // ------------------------------------------------

    /* ---------------- Internes Movement ---------------- */
    private void HandleHorizontal()
    {
        // Kamera‑Basics (y‑Komponente raus)
        Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();

        // 2D‑Input in 3D‑Weltbewegung umrechnen
        Vector3 desired = camF * moveInput.y + camR * moveInput.x;
        Vector3 targetVel = desired * movementSpeed;

        // Auf maxSpeed clampen
        Vector3 hor = new Vector3(targetVel.x, 0f, targetVel.z);
        if (hor.magnitude > maxSpeed)
            hor = hor.normalized * maxSpeed;

        // Alte y‑Komponente holen und neue linearVelocity setzen
        float currentY = rb.linearVelocity.y;
        rb.linearVelocity = new Vector3(hor.x, currentY, hor.z);
    }


    private void HandleJump()
    {
        if (jumpRequested && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x,
                                      jumpPower * jumpSpeedModifier,
                                      rb.linearVelocity.z);
        }
        jumpRequested = false;
    }

    private void HandleGravityModifiers()
    {
        // schneller runterfallen
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallSpeedModifier - 1f) * Time.fixedDeltaTime;
        }
        // Sprung früh abbrechen (Taste losgelassen & JumpSpeedModifier < 1)
        else if (rb.linearVelocity.y > 0f && !jumpRequested)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (jumpSpeedModifier - 1f) * Time.fixedDeltaTime;
        }
    }
    

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDist, groundLayer);
    }

    public void StartSprint() => movementSpeed *= sprintMultiplier;
    public void StopSprint() => movementSpeed /= sprintMultiplier;
}

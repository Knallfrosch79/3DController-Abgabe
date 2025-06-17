using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Behaviour : MonoBehaviour
{
    private PlayerControls controls;
    private Movement_Action movementInput;

    private void Awake()
    {
        controls = new PlayerControls();
        movementInput = GetComponent<Movement_Action>();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector2 move = controls.Player.Move.ReadValue<Vector2>();
        movementInput.MoveHorizontally(move);
    }
}

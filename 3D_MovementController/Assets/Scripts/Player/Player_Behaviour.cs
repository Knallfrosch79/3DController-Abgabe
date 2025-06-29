using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Movement_Action))]
public class Player_Behaviour : MonoBehaviour
{
    private Movement_Action movement;

    private void Awake()
    {
        movement = GetComponent<Movement_Action>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        movement.UpdateHorizontalMovement(input);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
            movement.RequestJump();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started) movement.StartSprint();
        if (ctx.canceled) movement.StopSprint();
    }
}
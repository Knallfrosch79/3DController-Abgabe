using UnityEngine;
using UnityEngine.InputSystem;
using DefaultNamespace.InteractableSystem;
using DefaultNamespace.TargetSystem;

namespace DefaultNamespace.PlayerSystem
{

    [RequireComponent(typeof(MovementAction))]
    public class PlayerController : MonoBehaviour
    {
        private MovementAction movement;
        private TargetProvider targetProvider; // Add a reference to TargetProvider
        private MovementAction movementAction;


        private void Awake()
        {
            movement = GetComponent<MovementAction>();
            targetProvider = GetComponent<TargetProvider>(); // Initialize TargetProvider
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            Vector2 input2D = ctx.ReadValue<Vector2>();
            Vector3 input3D = new Vector3(input2D.x, 0f, input2D.y);
            movement.UpdateHorizontalMovement(input3D);
            movement.UpdateHorizontalMovement(input3D);
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
                movement.RequestJump();
        }

        private void OnSprint(InputAction.CallbackContext ctx)
        {
            if (ctx.started) movement.StartSprint();
            if (ctx.canceled) movement.StopSprint();
        }

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                var interactable = targetProvider.GetTarget<Interactable>(); // Use the instance of TargetProvider
                if (interactable == null)
                {
                    return;
                }
                interactable.Interact();
            }
        }
    }
}
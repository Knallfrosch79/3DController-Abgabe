using UnityEngine;
using UnityEngine.InputSystem;
using DefaultNamespace.InteractableSystem;
using DefaultNamespace.TargetSystem;

namespace DefaultNamespace.PlayerSystem
{

    [RequireComponent(typeof(Movement_Action))]
    public class Player_Behaviour : MonoBehaviour
    {
        private Movement_Action movement;
        private TargetProvider targetProvider; // Add a reference to TargetProvider

        private void Awake()
        {
            movement = GetComponent<Movement_Action>();
            targetProvider = GetComponent<TargetProvider>(); // Initialize TargetProvider
        }

        private void OnMove(InputAction.CallbackContext moveInputAction)
        {
            Vector2 input = moveInputAction.ReadValue<Vector2>();
            movement.UpdateHorizontalMovement(input);
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
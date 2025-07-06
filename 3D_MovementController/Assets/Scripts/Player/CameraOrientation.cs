using UnityEngine;

namespace DefaultNamespace.CameraSystem
{
    public class CameraOrientation : MonoBehaviour
    {
        [SerializeField] private Transform orientation; // empty GameObject to control camera rotation
        [SerializeField] private float rotationSpeed = 5f;

        private float yaw;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // Mouse input for camera rotation
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            yaw += mouseX;

            // Orientation rotation
            orientation.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
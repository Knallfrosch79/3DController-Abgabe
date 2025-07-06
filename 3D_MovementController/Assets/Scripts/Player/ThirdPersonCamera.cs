using UnityEngine;

namespace DefaultNamespace.CameraSystem
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float followSpeed = 10f;

        private float yaw;
        private float pitch;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            // Camera-Rotation with mouse
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -35f, 60f); // FOV stop at 60 degrees

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 targetPosition = player.position + rotation * offset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.LookAt(player.position + Vector3.up * 1.5f); // look at player with a slight upward offset
        }
    }
}
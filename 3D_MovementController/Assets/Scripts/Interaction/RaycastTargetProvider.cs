using UnityEngine;
using System.Collections;

namespace DefaultNamespace.TargetSystem
{
    public class RaycastTargetProvider : TargetProvider
    {
        public Camera Camera;
        public float Distance = 5f;
        public LayerMask TargetLayerMask;

        private Transform cameraTransform;

        private void Awake()
        {
            cameraTransform = Camera.GetComponent<Transform>();
        }

        public override T GetTarget<T>()
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, Distance, TargetLayerMask))
            {
                return hit.collider.GetComponent<T>();
            }
            return null;
        }
    }
}
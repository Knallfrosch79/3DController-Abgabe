using UnityEngine;

namespace DefaultNamespace.TargetSystem
{
    public abstract class TargetProvider : MonoBehaviour
    {
        public abstract T GetTarget<T>() where T : MonoBehaviour;
    }
}
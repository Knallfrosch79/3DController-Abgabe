using UnityEngine;
using UnityEngine.Events;
using System;


namespace DefaultNamespace.InteractableSystem
{
    public class SwitchInteractable : Interactable
    {
        [Header("Settings")]
        public Color ColorOff = Color.red;
        public Color ColorOn = Color.green;

        [Header("Events")]
        public UnityEvent OnSwitchOn;
        public UnityEvent OnSwitchOff;

        private bool switchIsOn = false;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            ChangeColor(ColorOff);
        }

        public override void Interact()
        {
            switchIsOn = !switchIsOn;
            if (switchIsOn == false)
            {
                SwitchOff();
            }

            if (switchIsOn == true)
            {
                SwitchOn();
            }
        }

        private void SwitchOn()
        {
            ChangeColor(ColorOn);
            OnSwitchOn?.Invoke();
        }

        private void SwitchOff()
        {
            ChangeColor(ColorOff);
            OnSwitchOff?.Invoke();
        }

        private void ChangeColor(Color color)
        {
            meshRenderer.material.color = color;
        }
    }
}

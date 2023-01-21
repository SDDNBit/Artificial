using UnityEngine;
using UnityEngine.InputSystem;

namespace SoftBit.Autohand.Custom
{
    public class OpenXRHandDistanceGrabLink : MonoBehaviour
    {
        [SerializeField] private HandDistanceGrabberCustom handDistanceGrabberCustom;
        [SerializeField] private InputActionProperty grabAction;
        [SerializeField] private InputActionProperty stopGrabAction;

        void OnEnable()
        {
            if (grabAction.action != null) grabAction.action.Enable();
            if (grabAction.action != null) grabAction.action.performed += OnGrab;
            if (stopGrabAction.action != null) stopGrabAction.action.Enable();
            if (stopGrabAction.action != null) stopGrabAction.action.performed += OnStopGrab;
        }

        private void OnDisable()
        {
            if (grabAction.action != null) grabAction.action.performed -= OnGrab;
            if (stopGrabAction.action != null) stopGrabAction.action.performed -= OnStopGrab;
        }

        private void OnGrab(InputAction.CallbackContext e)
        {
            handDistanceGrabberCustom.SelectTarget();
        }

        private void OnStopGrab(InputAction.CallbackContext e)
        {
            handDistanceGrabberCustom.CancelSelect();
        }
    }
}

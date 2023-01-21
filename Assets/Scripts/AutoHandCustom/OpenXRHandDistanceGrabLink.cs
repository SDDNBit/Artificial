using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoftBit.Autohand.Custom
{
    public class OpenXRHandDistanceGrabLink : MonoBehaviour
    {
        public HandDistanceGrabberCustom pointGrab;
        public InputActionProperty grabAction;
        public InputActionProperty stopGrabAction;
        //public InputActionProperty selectAction;
        //public InputActionProperty stopSelectAction;

        void OnEnable()
        {
            if (grabAction.action != null) grabAction.action.Enable();
            if (grabAction.action != null) grabAction.action.performed += OnGrab;
            if (stopGrabAction.action != null) stopGrabAction.action.Enable();
            if (stopGrabAction.action != null) stopGrabAction.action.performed += OnStopGrab;

            //if (selectAction.action != null) selectAction.action.Enable();
            //if (selectAction.action != null) selectAction.action.performed += OnSelect;
            //if (stopSelectAction.action != null) stopSelectAction.action.Enable();
            //if (stopSelectAction.action != null) stopSelectAction.action.performed += OnDeselect;
        }

        private void OnDisable()
        {
            if (grabAction.action != null) grabAction.action.performed -= OnGrab;
            if (stopGrabAction.action != null) stopGrabAction.action.performed -= OnStopGrab;

            //if (selectAction.action != null) selectAction.action.performed -= OnSelect;
            //if (stopSelectAction.action != null) stopSelectAction.action.performed -= OnDeselect;

        }

        void OnGrab(InputAction.CallbackContext e)
        {
            //pointGrab.StartPointing();
            pointGrab.SelectTarget();
        }

        void OnStopGrab(InputAction.CallbackContext e)
        {
            //pointGrab.StopPointing();
            pointGrab.CancelSelect();
        }

        //private void OnSelect(InputAction.CallbackContext e)
        //{
        //    pointGrab.SelectTarget();
        //}

        //void OnDeselect(InputAction.CallbackContext e)
        //{
        //    pointGrab.CancelSelect();
        //}

    }
}

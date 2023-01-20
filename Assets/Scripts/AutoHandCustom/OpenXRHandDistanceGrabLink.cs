using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoftBit.Autohand.Custom
{
    public class OpenXRHandDistanceGrabLink : MonoBehaviour
    {
        public HandDistanceGrabberCustom pointGrab;
        public InputActionProperty pointAction;
        public InputActionProperty stopPointAction;
        //public InputActionProperty selectAction;
        //public InputActionProperty stopSelectAction;

        void OnEnable()
        {
            if (pointAction.action != null) pointAction.action.Enable();
            if (pointAction.action != null) pointAction.action.performed += OnPoint;
            if (stopPointAction.action != null) stopPointAction.action.Enable();
            if (stopPointAction.action != null) stopPointAction.action.performed += OnStopPoint;

            //if (selectAction.action != null) selectAction.action.Enable();
            //if (selectAction.action != null) selectAction.action.performed += OnSelect;
            //if (stopSelectAction.action != null) stopSelectAction.action.Enable();
            //if (stopSelectAction.action != null) stopSelectAction.action.performed += OnDeselect;
        }

        private void OnDisable()
        {
            if (pointAction.action != null) pointAction.action.performed -= OnPoint;
            if (stopPointAction.action != null) stopPointAction.action.performed -= OnStopPoint;

            //if (selectAction.action != null) selectAction.action.performed -= OnSelect;
            //if (stopSelectAction.action != null) stopSelectAction.action.performed -= OnDeselect;

        }

        void OnPoint(InputAction.CallbackContext e)
        {
            pointGrab.StartPointing();
            pointGrab.SelectTarget();
        }

        void OnStopPoint(InputAction.CallbackContext e)
        {
            pointGrab.StopPointing();
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

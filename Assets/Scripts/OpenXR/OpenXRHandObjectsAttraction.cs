using SoftBit.Mechanics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoftBit.OpenXR{
    public class OpenXRHandObjectsAttraction : MonoBehaviour{
        public HandObjectsAttraction HandObjectsAttraction;
        public InputActionProperty AttractingAction;
        public InputActionProperty StopAttractingAction;

        private void OnEnable(){
            if(AttractingAction.action != null) AttractingAction.action.Enable();
            if (AttractingAction.action != null) AttractingAction.action.performed += AttractingActionListener;
            if (StopAttractingAction.action != null) StopAttractingAction.action.Enable();
            if (StopAttractingAction.action != null) StopAttractingAction.action.performed += StopAttractingActionListener;
        }
        
        private void OnDisable() {
            if (AttractingAction.action != null) AttractingAction.action.performed -= AttractingActionListener;
            if (StopAttractingAction.action != null) StopAttractingAction.action.performed -= StopAttractingActionListener;
        }

        void AttractingActionListener(InputAction.CallbackContext e) {
            HandObjectsAttraction.ShouldAttract(true);
        }

        void StopAttractingActionListener(InputAction.CallbackContext e) {
            HandObjectsAttraction.ShouldAttract(false);
        }
    }
}

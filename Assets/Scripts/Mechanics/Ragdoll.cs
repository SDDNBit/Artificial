using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class Ragdoll : MonoBehaviour
    {
        //[SerializeField] private Transform hipsBone;
        [SerializeField] private Transform root;
        [SerializeField] private List<Rigidbody> ragdollRigidbodies;

        [ContextMenu("DisableRagdoll")]
        public void DisableRagdoll()
        {
            
            foreach (var rigidbody in ragdollRigidbodies)
            {
                rigidbody.isKinematic = true;
            }
        }

        [ContextMenu("EnableRagdoll")]
        public void EnableRagdoll()
        {
            foreach (var rigidbody in ragdollRigidbodies)
            {
                rigidbody.isKinematic = false;
            }
        }

        public void SetRagdollForScrap(List<ConnectionPart> partsThatAreActive)
        {
            if (partsThatAreActive.Count > 1)
            {

            }
        }

        //private void AlignPositionToHips()
        //{
        //    var hipsPos = hipsBone.position;
        //}

        #region BakeMethods
        [ContextMenu("SetRagdollRigidbodies")]
        private void SetRagdollRigidbodies()
        {
            ragdollRigidbodies.Clear();
            var rigidbodies = root.GetComponentsInChildren<Rigidbody>();
            foreach (var rigidbody in rigidbodies)
            {
                ragdollRigidbodies.Add(rigidbody);
            }
        }
        #endregion
    }
}

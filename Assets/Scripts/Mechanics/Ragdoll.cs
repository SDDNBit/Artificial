using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private List<Rigidbody> ragdollRigidbodies;

        [ContextMenu("DisableRagdoll")]
        public void DisableRagdoll()
        {
            foreach(var rigidbody in ragdollRigidbodies)
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

        #region BakeMethods
        [ContextMenu("SetRagdollRigidbodies")]
        private void SetRagdollRigidbodies()
        {
            ragdollRigidbodies.Clear();
            var rigidbodies = root.GetComponentsInChildren<Rigidbody>();
            foreach(var rigidbody in rigidbodies)
            {
                ragdollRigidbodies.Add(rigidbody);
            }
        }
        #endregion
    }
}

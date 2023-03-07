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

        public void SetRagdollForScrap(List<ConnectionPart> partsToActivate)
        {
            var firstEnemyCollider = partsToActivate[0].colliders[0].GetComponent<EnemyCollider>();
            var secondEnemyCollider = partsToActivate[1].colliders[0].GetComponent<EnemyCollider>();
            if (firstEnemyCollider == null || secondEnemyCollider == null)
            {
                Debug.LogError("You should use the collider with EnemyCollider script as the first collider in the ConnectionPart Colliders list. Not the ones that doesn't have the EnemyCollider script attached on them.");
            }
            var characterJoint = firstEnemyCollider.RagdollRigidbodyToApplyForceTo.GetComponent<CharacterJoint>();
            var secondCharacterJoint = secondEnemyCollider.RagdollRigidbodyToApplyForceTo.GetComponent<CharacterJoint>();
            if (characterJoint.connectedBody != secondCharacterJoint.connectedBody)
            {
                characterJoint.connectedBody = secondEnemyCollider.RagdollRigidbodyToApplyForceTo;
            }
            EnableRagdoll();
        }

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

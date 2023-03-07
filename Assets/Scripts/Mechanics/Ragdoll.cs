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
            ragdollRigidbodies.Clear();
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
            DestroyUnusedRigidbodies(partsToActivate);

            var firstEnemyCollider = partsToActivate[0].EnemyColliders[0];
            var secondEnemyCollider = partsToActivate[1].EnemyColliders[0];
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

        private void DestroyUnusedRigidbodies(List<ConnectionPart> partsToActivate)
        {
            var rigidbodiesInUse = GetRigidbodiesInUse(partsToActivate);
            CharacterJoint currentCharacterJoint;

            foreach (var ragdollRigidbody in ragdollRigidbodies)
            {
                if (!rigidbodiesInUse.Contains(ragdollRigidbody))
                {
                    currentCharacterJoint = ragdollRigidbody.GetComponent<CharacterJoint>();
                    if(currentCharacterJoint != null)
                    {
                        Destroy(currentCharacterJoint);
                    }
                    Destroy(ragdollRigidbody);
                }
            }
        }

        private List<Rigidbody> GetRigidbodiesInUse(List<ConnectionPart> partsToActivate)
        {
            var rigidbodiesInUse = new List<Rigidbody>();
            CharacterJoint currentCharacterJoint;
            foreach (var part in partsToActivate)
            {
                foreach (var enemyCollider in part.EnemyColliders)
                {
                    if (enemyCollider != null)
                    {
                        if (!rigidbodiesInUse.Contains(enemyCollider.RagdollRigidbodyToApplyForceTo))
                        {
                            rigidbodiesInUse.Add(enemyCollider.RagdollRigidbodyToApplyForceTo);
                            currentCharacterJoint = enemyCollider.RagdollRigidbodyToApplyForceTo.GetComponent<CharacterJoint>();
                            if (currentCharacterJoint != null)
                            {
                                if (!rigidbodiesInUse.Contains(currentCharacterJoint.connectedBody))
                                {
                                    rigidbodiesInUse.Add(currentCharacterJoint.connectedBody);
                                }
                            }
                        }
                    }
                }
            }
            return rigidbodiesInUse;
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

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
            ragdollRigidbodies.RemoveAll(item => item == null);
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
            partsToActivate[0].EnemyColliders[0].CharacterJoint.connectedBody = partsToActivate[1].EnemyColliders[0].RagdollRigidbodyToApplyForceTo;
            DestroyUnusedRigidbodies(partsToActivate);

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
                    if (currentCharacterJoint != null)
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
            foreach (var part in partsToActivate)
            {
                foreach (var enemyCollider in part.EnemyColliders)
                {
                    if (enemyCollider != null)
                    {
                        if (!rigidbodiesInUse.Contains(enemyCollider.RagdollRigidbodyToApplyForceTo))
                        {
                            rigidbodiesInUse.Add(enemyCollider.RagdollRigidbodyToApplyForceTo);
                            if (enemyCollider.CharacterJoint != null)
                            {
                                if (!rigidbodiesInUse.Contains(enemyCollider.CharacterJoint.connectedBody))
                                {
                                    rigidbodiesInUse.Add(enemyCollider.CharacterJoint.connectedBody);
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

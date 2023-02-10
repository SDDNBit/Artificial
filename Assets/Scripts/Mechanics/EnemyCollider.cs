using SoftBit.Autohand.Custom;
using SoftBit.States;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
        public FollowPivot AttachedObject;
        public Rigidbody RagdollRigidbodyToApplyForceTo;

        [SerializeField] private ConnectionPart connectionPart;

        [HideInInspector] public bool IsDestroyed = false;

        /// <summary>
        /// To be removed, only needed for testing, also awake
        /// </summary>
        [SerializeField] private EnemyStateMachine enemyStateMachine;
        private void Awake()
        {
            enemyStateMachine = GetComponentInParent<EnemyStateMachine>();
        }

        public void DestroyPart(Collision collision)
        {
            IsDestroyed = true;
            DetachAttachedObject();
            connectionPart.DestroyPart(collision);
        }

        public void RemoveJointAndRigidbody()
        {
            if (RagdollRigidbodyToApplyForceTo != null)
            {
                Destroy(RagdollRigidbodyToApplyForceTo.GetComponent<CharacterJoint>());
                Destroy(RagdollRigidbodyToApplyForceTo);
            }
        }

        private void DetachAttachedObject()
        {
            if (AttachedObject != null)
            {
                AttachedObject.PivotToFollow = null;
            }
        }

        [ContextMenu("TestDestroy")]
        private void TestDestroy()
        {
            DestroyPart(null);
            enemyStateMachine.SwitchState(enemyStateMachine.EnemyRagdollState);
            RagdollRigidbodyToApplyForceTo.AddForceAtPosition(500f * Vector3.up, RagdollRigidbodyToApplyForceTo.position, ForceMode.Impulse);
        }
    }
}
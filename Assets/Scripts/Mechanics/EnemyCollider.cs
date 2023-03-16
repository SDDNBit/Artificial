using SoftBit.Autohand.Custom;
using SoftBit.States;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
        public FollowPivot AttachedObject;
        public Rigidbody RagdollRigidbodyToApplyForceTo;
        //public Rigidbody FallbackRigidbodyToConnectTo;

        [HideInInspector] public bool IsDestroyed = false;
        [HideInInspector] public CharacterJoint CharacterJoint;

        [SerializeField] private ConnectionPart connectionPart;
        

        private Collider selfCollider;

        /// <summary>
        /// To be removed, only needed for testing, also awake
        /// </summary>
        [HideInInspector] private EnemyStateMachine enemyStateMachine;
        private void Awake()
        {
            enemyStateMachine = GetComponentInParent<EnemyStateMachine>();
            selfCollider = GetComponent<Collider>();
            CharacterJoint = RagdollRigidbodyToApplyForceTo.GetComponent<CharacterJoint>();
        }

        private void OnDestroy()
        {
            Destroy(selfCollider);
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
                if(CharacterJoint != null)
                {
                    Destroy(CharacterJoint);
                }
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
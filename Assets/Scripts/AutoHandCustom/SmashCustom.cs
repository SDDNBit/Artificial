using Autohand;
using NaughtyAttributes;
using SoftBit.Mechanics;
using SoftBit.States;
using UnityEngine;
using UnityEngine.Events;

namespace SoftBit.Autohand.Custom
{
    public class SmashCustom : MonoBehaviour
    {
        [SerializeField] private EnemyStateMachine enemyStateMachine;

        [Header("Smash Options")]
        [Tooltip("Required velocity magnitude from Smasher to smash")]
        [SerializeField] private float smashForce = 2;


        private void Awake()
        {
            enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            SmasherCustom smasher;
            EnemyCollider enemyCollider;
            if (collision.transform.CanGetComponent(out smasher))
            {
                if (smasher.LastCollisionGameObject != null && smasher.LastCollisionGameObject.CanGetComponent(out enemyCollider))
                {
                    if (!enemyCollider.IsDestroyed && smasher.GetMagnitude() >= smashForce)
                    {
                        enemyCollider.DestroyPart(collision);
                        TransitionToRagdoll();
                        enemyCollider.RagdollRigidbodyToApplyForceTo.AddForceAtPosition(500f * Vector3.up, enemyCollider.RagdollRigidbodyToApplyForceTo.position, ForceMode.Impulse);
                    }
                }
            }
        }

        //[ContextMenu("ApplyForceToRigidbody")]
        //private void ApplyForceToRigidbody()
        //{
        //    ragdollRigidbodyToApplyForceTo
        //}

        [ContextMenu("TransitionToRagdoll")]
        private void TransitionToRagdoll()
        {
            if (enemyStateMachine != null)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyRagdollState);
            }
        }
    }
}

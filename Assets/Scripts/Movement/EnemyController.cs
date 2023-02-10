using SoftBit.Mechanics;
using SoftBit.States;
using SoftBit.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

namespace SoftBit.Movement
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Ragdoll ragdoll;
        
        private Animator animator;
        private NavMeshAgent navMeshAgent;
        private Transform selfTransform;

        private void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        [ContextMenu("ActivateRagdoll")]
        private void ActivateRagdoll()
        {
            ragdoll.EnableRagdoll();
            animator.enabled = false;
            navMeshAgent.enabled = false;
        }

        [ContextMenu("DeactivateRagdoll")]
        private void DeactivateRagdoll()
        {
            ragdoll.DisableRagdoll();
            animator.enabled = true;
            navMeshAgent.enabled = true;
        }

        [ContextMenu("Stop")]
        private void Stop()
        {
            navMeshAgent.isStopped = !navMeshAgent.isStopped;
        }
    }
}

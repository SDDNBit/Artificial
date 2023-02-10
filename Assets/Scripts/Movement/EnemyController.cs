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
        private const float FacingAngle = 10f;
        private const float ChaseRange = 4f;
        private const float AttackRange = 1.5f;
        private const float RangeMargin = 0.5f;

        [SerializeField] private Transform player;
        [SerializeField] private Ragdoll ragdoll;
        [SerializeField] [Range(0f, 50f)] private float waitDelay = 1f;

        private Animator animator;
        private NavMeshAgent navMeshAgent;
        private EnemyState enemyState;
        private Transform selfTransform;
        private float distanceToThePlayer;
        private NavMeshTriangulation triangulation;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            triangulation = NavMesh.CalculateTriangulation();
            selfTransform = transform;
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            SetState();

            CheckForPatrolling();
            CheckForChasing();
            CheckForAttacking();
        }

        [ContextMenu("ActivateRagdoll")]
        private void ActivateRagdoll()
        {
            ragdoll.EnableRagdoll();
            animator.enabled = false;
            navMeshAgent.enabled = false;
        }

        [ContextMenu("Stop")]
        private void Stop()
        {
            navMeshAgent.isStopped = !navMeshAgent.isStopped;
        }

        private void CheckForAttacking()
        {
            if (enemyState == EnemyState.Attacking)
            {
                animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), true);
                animator.SetBool("Move", false);
                navMeshAgent.SetDestination(player.position);

                var relative = transform.InverseTransformPoint(player.position);
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                if (Mathf.Abs(angle) < FacingAngle)
                {
                    animator.SetBool("TurnRight", false);
                    animator.SetBool("TurnLeft", false);
                    navMeshAgent.updateRotation = true;
                }
                else
                {
                    if (angle > 0)
                    {
                        animator.SetBool("TurnRight", true);
                        animator.SetBool("TurnLeft", false);
                        navMeshAgent.updateRotation = false;
                    }
                    else
                    {
                        animator.SetBool("TurnRight", false);
                        animator.SetBool("TurnLeft", true);
                        navMeshAgent.updateRotation = false;
                    }
                }
            }
        }

        private void CheckForChasing()
        {
            if (enemyState == EnemyState.Chasing)
            {
                animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), false);
                animator.SetBool("Move", true);
                navMeshAgent.updateRotation = true;
                navMeshAgent.SetDestination(player.position);
            }
        }

        private void CheckForPatrolling()
        {
            if (enemyState == EnemyState.Patrolling)
            {
                animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), false);
                animator.SetBool("Move", true);
                navMeshAgent.updateRotation = true;
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
                {
                    NavigateAtRandomPosition();
                }
            }
        }

        private void SetState()
        {
            distanceToThePlayer = Vector3.Distance(selfTransform.position, player.position);
            if (distanceToThePlayer < ChaseRange)
            {
                if (distanceToThePlayer < AttackRange)
                {
                    enemyState = EnemyState.Attacking;
                }
                else
                {
                    if (enemyState == EnemyState.Attacking && distanceToThePlayer < AttackRange + RangeMargin)
                    {
                        enemyState = EnemyState.Attacking;
                    }
                    else
                    {
                        enemyState = EnemyState.Chasing;
                    }
                }
            }
            else
            {
                enemyState = EnemyState.Patrolling;
            }
        }

        private void NavigateAtRandomPosition()
        {
            int index = Random.Range(1, triangulation.vertices.Length - 1);
            navMeshAgent.SetDestination(Vector3.Lerp(navMeshAgent.transform.position, triangulation.vertices[index], 0.5f));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ChaseRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }
}

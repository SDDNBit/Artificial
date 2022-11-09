using SoftBit.States;
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

        public Camera cam;
        public NavMeshAgent navMeshAgent;
        public ThirdPersonCharacter thirdPersonCharacter;
        [SerializeField] private List<Transform> navPoints;
        [SerializeField] private Transform player;

        private RaycastHit hit;
        private int navPointIndex = 0;
        private EnemyState enemyState;
        private Transform selfTransform;
        private float distanceToThePlayer;

        private void Start()
        {
            selfTransform = transform;
            navMeshAgent.updateRotation = false;
            CheckForChase();
        }

        private void FixedUpdate()
        {
            if (enemyState == EnemyState.Patrolling)
            {
                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    thirdPersonCharacter.Move(navMeshAgent.desiredVelocity, false, false, false);
                }
                else
                {
                    SetDestination();
                    //thirdPersonCharacter.Move(Vector3.zero, false, false);
                }
            }

            if (enemyState == EnemyState.Chasing)
            {
                navMeshAgent.SetDestination(player.position);
                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    thirdPersonCharacter.Move(navMeshAgent.desiredVelocity, false, false, false);
                }
                else
                {
                    //make sure to treat the case when is just chasing because it can't attack
                }
            }

            if (enemyState == EnemyState.Attacking)
            {
                navMeshAgent.SetDestination(player.position);
                //if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                //{
                //    thirdPersonCharacter.Move(navMeshAgent.desiredVelocity, false, false, true);
                //}
                //else
                //{
                //}

                var direction = player.position - selfTransform.position;
                var x = Vector3.Angle(direction, selfTransform.forward);
                if (x < FacingAngle)
                {
                    thirdPersonCharacter.Move(Vector3.zero, false, false, true);
                }
                else
                {
                    //thirdPersonCharacter.Move(Vector3.zero, false, false, false);
                    thirdPersonCharacter.RotateTowardsInPlace(player.position);
                }
            }

            CheckForChase();
        }

        private void CheckForChase()
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
                    enemyState = EnemyState.Chasing;
                }
            }
            else
            {
                if (enemyState != EnemyState.Patrolling)
                {
                    enemyState = EnemyState.Patrolling;
                    SetDestination();
                }
            }
        }

        private void SetDestination()
        {
            navMeshAgent.SetDestination(navPoints[navPointIndex].position);
            navPointIndex = (navPointIndex + 1) % navPoints.Count;
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

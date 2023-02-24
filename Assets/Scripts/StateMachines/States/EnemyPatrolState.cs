using SoftBit.States.Abstract;
using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyPatrolState : IState
    {
        private const float TravelDistanceBeforeResting = 100f;

        private EnemyStateMachine enemyStateMachine;
        private float traveledDistance;
        private Vector3 previousPosition;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;

            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), false);
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.Move.ToString(), true);
            enemyStateMachine.NavMeshAgent.enabled = true;
            enemyStateMachine.NavMeshAgent.updateRotation = true;
            previousPosition = GetPosition();
        }

        public void Update()
        {
            if (enemyStateMachine.DistanceToPlayer < Constants.ChaseRange)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyChaseState);
                return;
            }
            UpdateTraveledDistance();

            if (enemyStateMachine.NavMeshAgent.enabled && enemyStateMachine.NavMeshAgent.remainingDistance < enemyStateMachine.NavMeshAgent.stoppingDistance)
            {
                if (traveledDistance > TravelDistanceBeforeResting)
                {
                    traveledDistance = 0f;
                    TakeSomeRest();
                }
                else
                {
                    NavigateAtRandomPosition();
                }
            }
        }

        private void NavigateAtRandomPosition()
        {
            int index = Random.Range(1, enemyStateMachine.NavMeshTriangulation.vertices.Length - 1);
            enemyStateMachine.NavMeshAgent.SetDestination(Vector3.Lerp(GetPosition(), enemyStateMachine.NavMeshTriangulation.vertices[index], 0.5f));
        }

        private void UpdateTraveledDistance()
        {
            traveledDistance += Vector3.Distance(previousPosition, GetPosition());
            previousPosition = GetPosition();
        }

        private Vector3 GetPosition()
        {
            return enemyStateMachine.NavMeshAgent.transform.position;
        }

        private void TakeSomeRest()
        {
            enemyStateMachine.SwitchState(enemyStateMachine.EnemyIdleState);
        }
    }

}

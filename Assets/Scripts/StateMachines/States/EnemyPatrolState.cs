using SoftBit.States.Abstract;
using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyPatrolState : IState
    {
        private EnemyStateMachine enemyStateMachine;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;

            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), false);
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.Move.ToString(), true);
            enemyStateMachine.NavMeshAgent.updateRotation = true;
        }

        public void Update()
        {
            if (enemyStateMachine.NavMeshAgent.remainingDistance < enemyStateMachine.NavMeshAgent.stoppingDistance)
            {
                NavigateAtRandomPosition();
            }
        }

        public void Exit() { }

        private void NavigateAtRandomPosition()
        {
            int index = Random.Range(1, enemyStateMachine.NavMeshTriangulation.vertices.Length - 1);
            enemyStateMachine.NavMeshAgent.SetDestination(
                Vector3.Lerp(enemyStateMachine.NavMeshAgent.transform.position, enemyStateMachine.NavMeshTriangulation.vertices[index], 0.5f)
                );
        }
    }

}

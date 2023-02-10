using SoftBit.States.Abstract;
using SoftBit.Utils;

namespace SoftBit.States
{
    public class EnemyChaseState : IState
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
            if (enemyStateMachine.DistanceToPlayer < Constants.AttackRange)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyAttackState);
                return;
            }
            if (enemyStateMachine.DistanceToPlayer > Constants.ChaseRange)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyPatrolState);
                return;
            }
            enemyStateMachine.NavMeshAgent.SetDestination(enemyStateMachine.Player.position);
        }
    }
}
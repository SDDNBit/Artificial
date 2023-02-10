using SoftBit.States.Abstract;
using SoftBit.Utils;

namespace SoftBit.States
{
    public class EnemyIdleState : IState
    {
        private EnemyStateMachine enemyStateMachine;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.Move.ToString(), false);
        }

        public void Update()
        {
            if (enemyStateMachine.DistanceToPlayer < Constants.ChaseRange)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyChaseState);
                return;
            }
        }

    }

}
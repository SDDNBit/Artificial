using SoftBit.States;
using SoftBit.States.Abstract;
using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyIdleState : IState
    {
        private EnemyStateMachine enemyStateMachine;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;
            enemyStateMachine.Animator.SetBool("Move", false);

            Debug.Log($"Enter in Idle State");
        }

        public void Update()
        {
            if (enemyStateMachine.DistanceToPlayer < Constants.ChaseRange)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyChaseState);
                return;
            }
            Debug.Log("Update in Idle State");
        }

    }

}
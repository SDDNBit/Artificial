using SoftBit.DataModels;
using SoftBit.States.Abstract;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyStandUpState : IState
    {
        private EnemyStateMachine enemyStateMachine;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;

            enemyStateMachine.Animator.enabled = true;
            enemyStateMachine.NavMeshAgent.enabled = true;
            enemyStateMachine.NavMeshAgent.updateRotation = false;
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), false);
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.Move.ToString(), false);
            enemyStateMachine.Animator.Play(Constants.EnemyAnimationStateNames.StandUpFromBack.ToString());
        }

        public void Update()
        {
            if (enemyStateMachine.Animator.GetCurrentAnimatorStateInfo(0).IsName(Constants.EnemyAnimationStateNames.StandUpFromBack.ToString()) == false)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyPatrolState);
            }
        }
    }
}
using SoftBit.DataModels;
using SoftBit.States.Abstract;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;
using static SoftBit.Utils.Constants;

namespace SoftBit.States
{
    public class EnemyStandUpState : IState
    {
        private EnemyStateMachine enemyStateMachine;
        private AnimatorStateInfo animatorStateInfo;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;

            enemyStateMachine.Animator.enabled = true;
            enemyStateMachine.NavMeshAgent.enabled = true;
            enemyStateMachine.NavMeshAgent.updateRotation = false;
            enemyStateMachine.Animator.SetBool(EnemyAnimatorParams.IsAttacking.ToString(), false);
            enemyStateMachine.Animator.SetBool(EnemyAnimatorParams.Move.ToString(), false);

            if (enemyStateMachine.LastRagdollOrientation == RagdollFacingOrientation.Up)
            {
                enemyStateMachine.Animator.Play(EnemyAnimationStateNames.StandUpFromBack.ToString());
            }
            else
            {
                enemyStateMachine.Animator.Play(EnemyAnimationStateNames.StandUpFromFront.ToString());
            }
        }

        public void Update()
        {
            animatorStateInfo = enemyStateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.IsName(EnemyAnimationStateNames.StandUpFromBack.ToString()) == false &&
                animatorStateInfo.IsName(EnemyAnimationStateNames.StandUpFromFront.ToString()) == false)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyPatrolState);
            }
        }
    }
}
using SoftBit.States.Abstract;
using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyAttackState : IState
    {
        EnemyStateMachine enemyStateMachine;

        public void Enter(IStateMachine stateMachine)
        {
            enemyStateMachine = (EnemyStateMachine)stateMachine;

            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.IsAttacking.ToString(), true);
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.Move.ToString(), false);
        }

        public void Update()
        {
            if (enemyStateMachine.DistanceToPlayer > Constants.AttackRange + Constants.RangeMargin)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyChaseState);
                return;
            }

            enemyStateMachine.NavMeshAgent.SetDestination(enemyStateMachine.Player.position);
            FaceThePlayer();
        }

        private void FaceThePlayer()
        {
            var relative = enemyStateMachine.transform.InverseTransformPoint(enemyStateMachine.Player.position);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
            if (Mathf.Abs(angle) < Constants.FacingAngle)
            {
                RotateEnemy(true, false, false);
            }
            else
            {
                if (angle > 0)
                {
                    RotateEnemy(false, false, true);
                }
                else
                {
                    RotateEnemy(false, true, false);
                }
            }
        }

        private void RotateEnemy(bool isRotatingByAgent, bool shouldTurnLeft, bool shouldTurnRight)
        {
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.TurnRight.ToString(), shouldTurnRight);
            enemyStateMachine.Animator.SetBool(Constants.EnemyAnimatorParams.TurnLeft.ToString(), shouldTurnLeft);
            enemyStateMachine.NavMeshAgent.updateRotation = isRotatingByAgent;
        }
    }
}
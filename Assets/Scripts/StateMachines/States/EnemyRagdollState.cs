using SoftBit.States.Abstract;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyRagdollState : IState
    {
        private const float TimeUntilGetUp = 2f;

        private EnemyStateMachine enemyStateMachine;
        private float enteredStateTime;

        public void Enter(IStateMachine stateMachine)
        {
            enteredStateTime = Time.time;
            enemyStateMachine = (EnemyStateMachine)stateMachine;
            enemyStateMachine.Animator.enabled = false;
            enemyStateMachine.NavMeshAgent.enabled = false;
            enemyStateMachine.Ragdoll.EnableRagdoll();
        }

        public void Update()
        {
            if (Time.time - enteredStateTime > TimeUntilGetUp)
            {
                ExitFromRagdollState();
            }
        }

        private void ExitFromRagdollState()
        {
            enemyStateMachine.Animator.enabled = true;
            enemyStateMachine.NavMeshAgent.enabled = true;
            enemyStateMachine.Ragdoll.DisableRagdoll();
            enemyStateMachine.SwitchState(enemyStateMachine.EnemyPatrolState);
        }
    }
}
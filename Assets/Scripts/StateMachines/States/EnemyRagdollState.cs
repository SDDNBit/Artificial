using SoftBit.DataModels;
using SoftBit.States.Abstract;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.States
{
    public class EnemyRagdollState : IState
    {
        private const float TimeUntilGetUp = 2f;
        private const float TimeToResetBones = 0.5f;

        private EnemyStateMachine enemyStateMachine;
        private float enteredStateTime;

        private List<BoneTransform> ragdollLastFrameAllBoneTransforms;
        private bool isResettingBones;
        private float elapsedTimeSinceResetBones;
        private float elapsedPercentage;

        public void Enter(IStateMachine stateMachine)
        {
            elapsedTimeSinceResetBones = 0;
            isResettingBones = false;
            enteredStateTime = Time.time;
            enemyStateMachine = (EnemyStateMachine)stateMachine;
            enemyStateMachine.Animator.enabled = false;
            enemyStateMachine.NavMeshAgent.enabled = false;
            enemyStateMachine.Ragdoll.EnableRagdoll();
        }

        public void Update()
        {
            if (!isResettingBones)
            {
                if (Time.time - enteredStateTime > TimeUntilGetUp)
                {
                    BeforeResettingBones();
                }
            }
            else
            {
                ResettingBones();
            }
        }

        private void ResettingBones()
        {
            elapsedTimeSinceResetBones += Time.deltaTime;
            elapsedPercentage = elapsedTimeSinceResetBones / TimeToResetBones;

            for (int i = 0; i < enemyStateMachine.AllBoneTransforms.Length; ++i)
            {
                enemyStateMachine.AllBoneTransforms[i].localPosition
                    = Vector3.Lerp(
                        ragdollLastFrameAllBoneTransforms[i].Position,
                        enemyStateMachine.StandUpFirstFrameAllBoneTransforms[i].Position,
                        elapsedPercentage);

                enemyStateMachine.AllBoneTransforms[i].localRotation
                    = Quaternion.Lerp(
                        ragdollLastFrameAllBoneTransforms[i].Rotation,
                        enemyStateMachine.StandUpFirstFrameAllBoneTransforms[i].Rotation,
                        elapsedPercentage);
            }

            if (elapsedPercentage >= 1)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyStandUpState);
            }
        }

        private void BeforeResettingBones()
        {
            isResettingBones = true;
            //enemyStateMachine.Animator.enabled = true;
            //enemyStateMachine.NavMeshAgent.enabled = true;
            //enemyStateMachine.EnemyMovement.AlignPositionToHips();
            //enemyStateMachine.Ragdoll.DisableRagdoll();
            AlignPositionToHips();

            ragdollLastFrameAllBoneTransforms = new();
            enemyStateMachine.PopulateBoneTransforms(ragdollLastFrameAllBoneTransforms);

            enemyStateMachine.Ragdoll.DisableRagdoll();
            //enemyStateMachine.SwitchState(enemyStateMachine.EnemyStandUpState);
        }

        public void AlignPositionToHips()
        {
            var originalHipsPosition = enemyStateMachine.HipsBone.position;
            enemyStateMachine.SelfTransform.position = enemyStateMachine.HipsBone.position;

            //if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit))
            //{
            //    transform.position = new Vector3(transform.position.x, raycastHit.point.y, transform.position.z);
            //}

            enemyStateMachine.HipsBone.position = originalHipsPosition;
        }
    }
}
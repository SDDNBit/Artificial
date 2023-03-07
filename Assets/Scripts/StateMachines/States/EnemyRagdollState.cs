using SoftBit.DataModels;
using SoftBit.States.Abstract;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static SoftBit.Utils.Constants;

namespace SoftBit.States
{
    public class EnemyRagdollState : IState
    {
        private const float RaycastLength = 1.5f;
        private const float TimeUntilGetUp = 5f;
        private const float TimeToResetBones = 0.5f;
        private const float NavmeshReachDistance = 0.1f;

        private EnemyStateMachine enemyStateMachine;
        private float enteredStateTime;

        private List<BoneTransform> ragdollLastFrameAllBoneTransforms;
        private bool isResettingBones;
        private float elapsedTimeSinceResetBones;
        private float elapsedPercentage;
        private int layerMaskForRaycast;

        public void Enter(IStateMachine stateMachine)
        {
            layerMaskForRaycast = ~LayerMask.GetMask(Layers.EnemyColliders.ToString());
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
                if (enemyStateMachine.AllBoneTransforms[i] != null)
                {
                    enemyStateMachine.AllBoneTransforms[i].localPosition
                        = Vector3.Lerp(
                            ragdollLastFrameAllBoneTransforms[i].Position,
                            GetBoneTransformBasedOnOrientation(i).Position,
                            elapsedPercentage);

                    enemyStateMachine.AllBoneTransforms[i].localRotation
                        = Quaternion.Lerp(
                            ragdollLastFrameAllBoneTransforms[i].Rotation,
                            GetBoneTransformBasedOnOrientation(i).Rotation,
                            elapsedPercentage);
                }
            }

            if (elapsedPercentage >= 1)
            {
                enemyStateMachine.SwitchState(enemyStateMachine.EnemyStandUpState);
            }
        }

        private BoneTransform GetBoneTransformBasedOnOrientation(int index)
        {
            if (enemyStateMachine.LastRagdollOrientation == RagdollFacingOrientation.Up)
            {
                return enemyStateMachine.StandUpFromBackFirstFrameAllBoneTransforms[index];
            }
            else
            {
                return enemyStateMachine.StandUpFromFrontFirstFrameAllBoneTransforms[index];
            }
        }

        private void BeforeResettingBones()
        {
            isResettingBones = true;

            EstablishRagdollOrientation();
            AlignRotationToHips();
            AlignPositionToHips();

            ragdollLastFrameAllBoneTransforms = new List<BoneTransform>();
            enemyStateMachine.PopulateBoneTransforms(ragdollLastFrameAllBoneTransforms);

            enemyStateMachine.Ragdoll.DisableRagdoll();
        }

        private void EstablishRagdollOrientation()
        {
            if (Vector3.Angle(enemyStateMachine.HipsBone.right, Vector3.up) < Vector3.Angle(enemyStateMachine.HipsBone.right, Vector3.down))
            {
                enemyStateMachine.LastRagdollOrientation = RagdollFacingOrientation.Down;
            }
            else
            {
                enemyStateMachine.LastRagdollOrientation = RagdollFacingOrientation.Up;
            }
        }

        private void AlignRotationToHips()
        {
            var originalHipsPosition = enemyStateMachine.HipsBone.position;
            var originalHipsRotation = enemyStateMachine.HipsBone.rotation;

            var desiredDirection = enemyStateMachine.HipsBone.up * (enemyStateMachine.LastRagdollOrientation == RagdollFacingOrientation.Down ? 1 : -1);
            desiredDirection.y = 0;
            desiredDirection.Normalize();

            var fromToRotation = Quaternion.FromToRotation(enemyStateMachine.SelfTransform.forward, desiredDirection);
            enemyStateMachine.SelfTransform.rotation *= fromToRotation;

            enemyStateMachine.HipsBone.position = originalHipsPosition;
            enemyStateMachine.HipsBone.rotation = originalHipsRotation;
        }

        /// <summary>
        /// In case there is still a small issue when position is aligned, the commented code might be needed
        /// </summary>
        private void AlignPositionToHips()
        {
            var originalHipsPosition = enemyStateMachine.HipsBone.position;
            enemyStateMachine.SelfTransform.position = enemyStateMachine.HipsBone.position;
            //var positionOffset = GetHipsPositionFromStandUpAnim();
            //positionOffset.y = 0;
            //positionOffset = enemyStateMachine.SelfTransform.rotation * positionOffset;
            //enemyStateMachine.SelfTransform.position -= positionOffset;

            if (Physics.Raycast(enemyStateMachine.SelfTransform.position, Vector3.down, out RaycastHit raycastHit, RaycastLength, layerMaskForRaycast))
            {
                if (NavMesh.SamplePosition(raycastHit.point, out NavMeshHit navMeshHit, NavmeshReachDistance, NavMesh.AllAreas))
                {
                    enemyStateMachine.SelfTransform.position = navMeshHit.position;
                }
                else
                {
                    enemyStateMachine.SelfTransform.position = new Vector3(enemyStateMachine.SelfTransform.position.x, raycastHit.point.y, enemyStateMachine.SelfTransform.position.z);
                }
            }

            enemyStateMachine.HipsBone.position = originalHipsPosition;
        }

        private Vector3 GetHipsPositionFromStandUpAnim()
        {
            if (enemyStateMachine.LastRagdollOrientation == RagdollFacingOrientation.Up)
            {
                return enemyStateMachine.StandUpFromBackFirstFrameAllBoneTransforms[0].Position;
            }
            else
            {
                return enemyStateMachine.StandUpFromFrontFirstFrameAllBoneTransforms[0].Position;
            }
        }
    }
}
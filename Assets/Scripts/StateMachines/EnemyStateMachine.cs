using SoftBit.DataModels;
using SoftBit.Mechanics;
using SoftBit.States.Abstract;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static SoftBit.Utils.Constants;

namespace SoftBit.States
{
    public class EnemyStateMachine : MonoBehaviour, IStateMachine
    {
        #region States
        public EnemyIdleState EnemyIdleState = new();
        public EnemyAttackState EnemyAttackState = new();
        public EnemyPatrolState EnemyPatrolState = new();
        public EnemyChaseState EnemyChaseState = new();
        public EnemyRagdollState EnemyRagdollState = new();
        public EnemyStandUpState EnemyStandUpState = new();
        #endregion

        public Transform Player;
        public Ragdoll Ragdoll;
        public Transform RagdollPivot;

        [HideInInspector] public float DistanceToPlayer;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public NavMeshAgent NavMeshAgent;
        [HideInInspector] public NavMeshTriangulation NavMeshTriangulation;
        [HideInInspector] public Transform SelfTransform;
        [HideInInspector] public EnemyMovement EnemyMovement;
        [HideInInspector] public Transform[] AllBoneTransforms;
        [HideInInspector] public Transform HipsBone;
        [HideInInspector] public RagdollFacingOrientation LastRagdollOrientation;

        [HideInInspector] public List<BoneTransform> StandUpFromBackFirstFrameAllBoneTransforms = new();
        [HideInInspector] public List<BoneTransform> StandUpFromFrontFirstFrameAllBoneTransforms = new();

        private IState currentState;

        private void Awake()
        {
            EnemyMovement = GetComponent<EnemyMovement>();
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            SelfTransform = transform;
            HipsBone = Animator.GetBoneTransform(HumanBodyBones.Hips);
            AllBoneTransforms = HipsBone.GetComponentsInChildren<Transform>(true);
            SampleBonesFromFirstFrameOfStandUpAnimations();

            NavMeshAgent.isStopped = false;
            NavMeshTriangulation = NavMesh.CalculateTriangulation();

            SwitchState(EnemyPatrolState);
        }

        private void Update()
        {
            DistanceToPlayer = Vector3.Distance(SelfTransform.position, Player.position);
            currentState.Update();
        }

        public void SwitchState(IState state)
        {
            currentState = state;
            currentState.Enter(this);
        }

        public void PopulateBoneTransforms(List<BoneTransform> boneTransforms)
        {
            for (int i = 0; i < AllBoneTransforms.Length; ++i)
            {
                boneTransforms.Add(new BoneTransform
                {
                    Position = AllBoneTransforms[i].localPosition,
                    Rotation = AllBoneTransforms[i].localRotation,
                    Tag = AllBoneTransforms[i].tag
                });
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, ChaseRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }

        private void SampleBonesFromFirstFrameOfStandUpAnimations()
        {
            var positionBeforeSampling = SelfTransform.position;
            var rotationBeforeSampling = SelfTransform.rotation;

            foreach (var animationClip in Animator.runtimeAnimatorController.animationClips)
            {
                if (animationClip.name == EnemyAnimationClipNames.StandUpFromBack.ToString())
                {
                    PopulateStandUpBoneTransforms(animationClip, StandUpFromBackFirstFrameAllBoneTransforms);
                }
                if (animationClip.name == EnemyAnimationClipNames.StandUpFromFront.ToString())
                {
                    PopulateStandUpBoneTransforms(animationClip, StandUpFromFrontFirstFrameAllBoneTransforms);
                }
            }

            SelfTransform.position = positionBeforeSampling;
            SelfTransform.rotation = rotationBeforeSampling;
        }

        private void PopulateStandUpBoneTransforms(AnimationClip animationClip, List<BoneTransform> boneTransforms)
        {
            animationClip.SampleAnimation(gameObject, 0);
            PopulateBoneTransforms(boneTransforms);
        }
    }
}

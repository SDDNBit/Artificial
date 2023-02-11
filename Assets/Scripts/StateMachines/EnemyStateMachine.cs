using SoftBit.DataModels;
using SoftBit.Mechanics;
using SoftBit.States.Abstract;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

        [HideInInspector] public float DistanceToPlayer;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public NavMeshAgent NavMeshAgent;
        [HideInInspector] public NavMeshTriangulation NavMeshTriangulation;
        [HideInInspector] public Transform SelfTransform;
        [HideInInspector] public EnemyMovement EnemyMovement;
        [HideInInspector] public Transform[] AllBoneTransforms;
        [HideInInspector] public Transform HipsBone;
        [HideInInspector] public List<BoneTransform> StandUpFirstFrameAllBoneTransforms = new();

        private IState currentState;

        private void Awake()
        {
            EnemyMovement = GetComponent<EnemyMovement>();
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            SelfTransform = transform;
            HipsBone = Animator.GetBoneTransform(HumanBodyBones.Hips);
            AllBoneTransforms = HipsBone.GetComponentsInChildren<Transform>(true);
            print($"Bones: {AllBoneTransforms.Length}");
            SampleBonesFromFirstFrameOfStandUpAnimation();

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
                    Rotation = AllBoneTransforms[i].localRotation
                });
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Constants.ChaseRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Constants.AttackRange);
        }

        private void SampleBonesFromFirstFrameOfStandUpAnimation()
        {
            var positionBeforeSampling = SelfTransform.position;
            var rotationBeforeSampling = SelfTransform.rotation;

            foreach (var animationClip in Animator.runtimeAnimatorController.animationClips)
            {
                if (animationClip.name == Constants.EnemyAnimationClipNames.StandUpFromBack.ToString())
                {
                    animationClip.SampleAnimation(gameObject, 0);
                    PopulateBoneTransforms(StandUpFirstFrameAllBoneTransforms);
                    break;
                }
            }

            SelfTransform.position = positionBeforeSampling;
            SelfTransform.rotation = rotationBeforeSampling;
        }
    }
}

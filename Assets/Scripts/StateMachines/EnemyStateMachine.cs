using SoftBit.States.Abstract;
using SoftBit.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace SoftBit.States
{
    public class EnemyStateMachine: MonoBehaviour, IStateMachine
    {
        #region States
        public EnemyIdleState EnemyIdleState = new();
        public EnemyAttackState EnemyAttackState = new();
        public EnemyPatrolState EnemyPatrolState = new();
        public EnemyChaseState EnemyChaseState = new();
        #endregion

        public Transform Player;
        [HideInInspector] public float DistanceToPlayer;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public NavMeshAgent NavMeshAgent;
        [HideInInspector] public NavMeshTriangulation NavMeshTriangulation;
        [HideInInspector] public Transform SelfTransform;
        
        private IState currentState;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshTriangulation = NavMesh.CalculateTriangulation();
            SelfTransform = transform;

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Constants.ChaseRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Constants.AttackRange);
        }
    }
}

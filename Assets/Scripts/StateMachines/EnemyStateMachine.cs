using SoftBit.States.Abstract;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoftBit.States
{
    public class EnemyStateMachine: MonoBehaviour, IStateMachine
    {
        public Transform Player;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public NavMeshAgent NavMeshAgent;
        [HideInInspector] public NavMeshTriangulation NavMeshTriangulation;
        [HideInInspector] public Transform SelfTransform;

        private IState currentState;
        private EnemyIdleState enemyIdleState = new();

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshTriangulation = NavMesh.CalculateTriangulation();
            SelfTransform = transform;

            SetInitState();
        }

        private void Update()
        {
            currentState.Update();
        }

        private void SetInitState()
        {
            currentState = enemyIdleState;
            currentState.Enter(this);
        }
    }
}

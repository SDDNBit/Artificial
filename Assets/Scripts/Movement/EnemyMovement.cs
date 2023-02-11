using UnityEngine.AI;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private LookAt lookAt;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 rootPosition;
    private Vector3 worldDeltaPosition;
    private Vector2 deltaPosition;

    //private Transform hipsBone;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        //agent.updateRotation = false;

        //hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
    }

    private void OnAnimatorMove()
    {
        rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        if (!agent.updateRotation)
        {
            transform.rotation = animator.rootRotation;
        }
        agent.nextPosition = rootPosition;
    }

    private void SynchronizeAnimatorAndAgent()
    {
        worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        deltaPosition = new Vector2(Vector3.Dot(transform.right, worldDeltaPosition), Vector3.Dot(transform.forward, worldDeltaPosition));
        deltaPosition = deltaPosition.normalized;

        //animator.SetBool("Move", !agent.isStopped);
        animator.SetFloat("Turn", deltaPosition.x);
        animator.SetFloat("Forward", deltaPosition.y);

        if (lookAt != null)
        {
            lookAt.lookAtTargetPosition = agent.destination;
        }
    }
}
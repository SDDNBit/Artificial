using UnityEngine.AI;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 rootPosition;
    private Vector3 worldDeltaDirection;
    private Vector2 deltaPosition;
    private Vector2 smoothDeltaPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        smoothDeltaPosition = Vector2.zero;
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
        worldDeltaDirection = agent.nextPosition - transform.position;
        worldDeltaDirection.y = 0;
        // Map 'worldDeltaDirection' to local space
        deltaPosition = new Vector2(Vector3.Dot(transform.right, worldDeltaDirection), Vector3.Dot(transform.forward, worldDeltaDirection));
        deltaPosition = deltaPosition.normalized;
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, Mathf.Min(1, Time.deltaTime / 0.1f));

        animator.SetFloat("Turn", smoothDeltaPosition.x);
        animator.SetFloat("Forward", smoothDeltaPosition.y);

        //if (lookAt != null)
        //{
        //    lookAt.lookAtTargetPosition = agent.destination;
        //}
    }
}
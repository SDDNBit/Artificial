using UnityEngine.AI;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private LookAt lookAt;
    [SerializeField] [Range(0f, 50f)] private float waitDelay = 1f;

    private NavMeshTriangulation triangulation;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        triangulation = NavMesh.CalculateTriangulation();
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void Start()
    {
        GoToRandomPoint();
    }

    public void GoToRandomPoint()
    {
        StartCoroutine(DoMoveToRandomPoint());
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        transform.rotation = animator.rootRotation;
        agent.nextPosition = rootPosition;
    }

    private void Update()
    {
        SynchronizeAnimatorAndAgent();
    }

    private void SynchronizeAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);
        deltaPosition = deltaPosition.normalized;

        bool shouldMove = deltaPosition.magnitude > 0.5f && agent.remainingDistance > agent.stoppingDistance;

        animator.SetBool("Move", shouldMove);
        animator.SetFloat("Turn", deltaPosition.x);
        animator.SetFloat("Forward", deltaPosition.y);

        if (lookAt != null)
        {
            lookAt.lookAtTargetPosition = agent.destination;
        }
    }

    public void StopMoving()
    {
        agent.isStopped = true;
        StopAllCoroutines();
    }

    private IEnumerator DoMoveToRandomPoint()
    {
        agent.enabled = true;
        agent.isStopped = false;
        WaitForSeconds Wait = new WaitForSeconds(waitDelay);
        while (true)
        {
            int index = Random.Range(1, triangulation.vertices.Length - 1);
            agent.SetDestination(Vector3.Lerp(agent.transform.position, triangulation.vertices[index], 0.5f));

            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            yield return Wait;
        }
    }
}
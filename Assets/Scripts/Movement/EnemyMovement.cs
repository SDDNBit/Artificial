using UnityEngine.AI;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private LookAt lookAt;
    [SerializeField]
    [Range(0f, 3f)]
    private float waitDelay = 1f;

    private NavMeshTriangulation triangulation;
    private NavMeshAgent agent;
    private Animator animator;

    private Vector2 velocity;
    private Vector2 smoothDeltaPosition;

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

        // Low-pass filter the deltaMove
        //float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        //smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        velocity = deltaPosition / Time.deltaTime;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            velocity = Vector2.Lerp(Vector2.zero, velocity, agent.remainingDistance);
        }

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.stoppingDistance;

        animator.SetBool("Move", shouldMove);
        animator.SetFloat("Turn", velocity.x);
        animator.SetFloat("Forward", velocity.y);

        if (lookAt != null)
        {
            lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;
        }

        //float deltaMagnitude = worldDeltaPosition.magnitude;
        //if (deltaMagnitude > Agent.radius / 2)
        //{
        //    transform.position = Vector3.Lerp(Animator.rootPosition, Agent.nextPosition, smooth);
        //}
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
            agent.SetDestination(Vector3.Lerp(
                triangulation.vertices[index],
                triangulation.vertices[index + (Random.value > 0.5f ? -1 : 1)],
                Random.value)
            );

            yield return null;
            yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
            yield return Wait;
        }
    }
}
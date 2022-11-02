using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent navMeshAgent;
    public ThirdPersonCharacter thirdPersonCharacter;
    [SerializeField] private List<Transform> navPoints;

    private RaycastHit hit;
    private int navPointIndex = 0;

    private void Start()
    {
        navMeshAgent.updateRotation = false;
        SetDestination();
    }

    private void Update()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            thirdPersonCharacter.Move(navMeshAgent.desiredVelocity, false, false);
        }
        else
        {
            SetDestination();
            //thirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }

    private void SetDestination()
    {
        navMeshAgent.SetDestination(navPoints[navPointIndex].position);
        navPointIndex = (navPointIndex + 1) % navPoints.Count;
    }
}

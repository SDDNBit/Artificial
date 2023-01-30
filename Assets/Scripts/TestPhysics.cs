using SoftBit.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPhysics : MonoBehaviour
{
    private const float Ratio = 0.005f;

    [SerializeField] private float strength;
    [SerializeField] private LayerMask layers;
    [SerializeField] private bool applyCustomCollisionDetection;

    private Vector3 lastPosition;
    private Transform selfTransform;
    private Vector3 directionFromLastPosition;
    private RaycastHit hitInfo;
    private Rigidbody selfRigidbody;

    private void Start()
    {
        if (layers == 0)
        {
            layers = LayerMask.GetMask(Constants.DefaultLayer);
        }
        selfTransform = transform;
        lastPosition = transform.position;
        selfRigidbody = GetComponent<Rigidbody>();
        selfRigidbody.AddForce(strength * Vector3.down, ForceMode.Impulse);
        selfRigidbody.AddTorque(strength * Vector3.right, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (applyCustomCollisionDetection)
        {
            directionFromLastPosition = selfTransform.position - lastPosition;

            if (Physics.Raycast(lastPosition, directionFromLastPosition, out hitInfo, directionFromLastPosition.magnitude, layers, QueryTriggerInteraction.Ignore))
            {
                selfRigidbody.velocity = Vector3.zero;
                selfRigidbody.angularVelocity = Vector3.zero;
                selfTransform.position = hitInfo.point + (hitInfo.point - selfTransform.position).normalized * Ratio;
            }
            lastPosition = selfTransform.position;
        }
    }
}

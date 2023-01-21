using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandTargeting : MonoBehaviour
{
    [SerializeField] private DistanceGrabbable distanceGrabbable;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        distanceGrabbable.StartTargeting.AddListener(OnStartTargeting);
        distanceGrabbable.StopTargeting.AddListener(OnStopTargeting);
    }

    private void OnDisable()
    {
        distanceGrabbable.StartTargeting.RemoveListener(OnStartTargeting);
        distanceGrabbable.StopTargeting.RemoveListener(OnStopTargeting);
    }

    private void OnStartTargeting(Hand hand, Grabbable grabbable)
    {
        meshRenderer.enabled = true;
    }

    private void OnStopTargeting(Hand hand, Grabbable grabbable)
    {
        meshRenderer.enabled = false;
    }
}

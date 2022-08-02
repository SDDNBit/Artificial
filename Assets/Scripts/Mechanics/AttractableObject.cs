using Autohand;
using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(FlyToObject))]
    public class AttractableObject : MonoBehaviour
    {
        [HideInInspector] public bool IsGrabbed = false;
        [HideInInspector] public bool IsAlreadyOrbiting = false;
        [HideInInspector] public FlyToObject FlyToObjectComponent;
        [HideInInspector] public DestroyIfNotInUse DestroyIfNotInUseComponent;
        [HideInInspector] public Rigidbody RigidbodyComponent;

        private Grabbable grabbable;

        private void Awake()
        {
            RigidbodyComponent = GetComponent<Rigidbody>();
            FlyToObjectComponent = GetComponent<FlyToObject>();
            DestroyIfNotInUseComponent = GetComponent<DestroyIfNotInUse>();
            grabbable = GetComponent<Grabbable>();
            grabbable.onGrab.AddListener(GrabListener);
            grabbable.onRelease.AddListener(ReleaseListener);
        }

        private void GrabListener(Hand hand, Grabbable grabbable)
        {
            DestroyIfNotInUseComponent.InUse = true;
            IsGrabbed = true;
        }

        private void ReleaseListener(Hand hand, Grabbable grabbable)
        {
            DestroyIfNotInUseComponent.InUse = false;
            IsGrabbed = false;
        }
    }
}

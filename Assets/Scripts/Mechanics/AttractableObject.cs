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

        private HandObjectsAttraction handObjectsAttraction;
        private Grabbable grabbable;

        private void Awake()
        {
            RigidbodyComponent = GetComponent<Rigidbody>();
            FlyToObjectComponent = GetComponent<FlyToObject>();
            DestroyIfNotInUseComponent = GetComponent<DestroyIfNotInUse>();
            grabbable = GetComponent<Grabbable>();
            grabbable.onGrab.AddListener(GrabListener);
            grabbable.onRelease.AddListener(ReleaseListener);
            var layer = LayerMask.NameToLayer(Utils.Constants.AttractableObjectLayer);
            if (layer != -1)
            {
                SetLayerRecursively(gameObject, layer);
            }
        }

        public void SetObjectAttractionComponent(HandObjectsAttraction handObjectsAttraction)
        {
            this.handObjectsAttraction = handObjectsAttraction;
        }

        private void GrabListener(Hand hand, Grabbable grabbable)
        {
            if (IsAlreadyOrbiting)
            {
                handObjectsAttraction.DetachAttractedObject(this);
            }
            DestroyIfNotInUseComponent.InUse = true;
            IsGrabbed = true;
        }

        private void ReleaseListener(Hand hand, Grabbable grabbable)
        {
            DestroyIfNotInUseComponent.InUse = false;
            IsGrabbed = false;
        }

        private void SetLayerRecursively(GameObject gameObject, int layerNumber)
        {
            foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
            {
                transform.gameObject.layer = layerNumber;
            }
        }
    }
}

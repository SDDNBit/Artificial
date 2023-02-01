using Autohand;
using SoftBit.Utils;
using System;
using UnityEngine;
using UnityEngine.Events;
using Constants = SoftBit.Utils.Constants;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(FlyToObject))]
    public class AttractableObject : MonoBehaviour
    {
        private const float Ratio = 0.005f;

        public bool AddSmasherOnShoot = false;
        public LayerMask layersForCustomCollision;
        [HideInInspector] public bool IsGrabbed = false;
        [HideInInspector] public bool IsAlreadyOrbiting = false;
        [HideInInspector] public FlyToObject FlyToObjectComponent;
        [HideInInspector] public DestroyIfNotInUse DestroyIfNotInUseComponent;
        [HideInInspector] public Rigidbody RigidbodyComponent;

        [Tooltip("Called when the object is or not targeted")]
        [HideInInspector] public UnityEvent<Grabbable, bool> ObjectTargeted = new();

        private HandObjectsAttraction handObjectsAttraction;
        private Grabbable grabbable;

        private Vector3 lastPosition;
        private Transform selfTransform;
        private Vector3 directionFromLastPosition;
        private RaycastHit hitInfo;

        private void Awake()
        {
            if (layersForCustomCollision == 0)
            {
                layersForCustomCollision = LayerMask.GetMask(Constants.DefaultLayer);
            }
            selfTransform = transform;
            lastPosition = selfTransform.position;
            RigidbodyComponent = GetComponent<Rigidbody>();
            FlyToObjectComponent = GetComponent<FlyToObject>();
            DestroyIfNotInUseComponent = GetComponent<DestroyIfNotInUse>();
            grabbable = GetComponent<Grabbable>();
            grabbable.onGrab.AddListener(GrabListener);
            grabbable.onRelease.AddListener(ReleaseListener);
            var layer = LayerMask.NameToLayer(Constants.AttractableObjectLayer);
            if (layer != -1)
            {
                SetLayerRecursively(gameObject, layer);
            }
        }

        private void FixedUpdate()
        {
            //CheckForClampVelocity(); use it just if you want extra validations to restrict the velocity
            PreventTunnelingThroughObjects();
        }

        public void SetTargetOnAttractable(bool isTargeted)
        {
            ObjectTargeted.Invoke(grabbable, isTargeted);
        }

        public void SetAttractableState(bool isAttracted, bool inUse, Transform target = null)
        {
            if (DestroyIfNotInUseComponent != null)
            {
                DestroyIfNotInUseComponent.InUse = inUse;
            }
            FlyToObjectComponent.Target = target;
            IsAlreadyOrbiting = isAttracted;

            if (isAttracted)
            {
                gameObject.layer = LayerMask.NameToLayer(Constants.DefaultLayer);
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer(Constants.AttractableObjectLayer);
            }
        }

        public void SetObjectAttractionComponent(HandObjectsAttraction handObjectsAttraction)
        {
            this.handObjectsAttraction = handObjectsAttraction;
        }

        private void PreventTunnelingThroughObjects()
        {
            directionFromLastPosition = selfTransform.position - lastPosition;

            if (Physics.Raycast(lastPosition, directionFromLastPosition, out hitInfo, directionFromLastPosition.magnitude, layersForCustomCollision, QueryTriggerInteraction.Ignore))
            {
                RigidbodyComponent.velocity = Vector3.zero;
                RigidbodyComponent.angularVelocity = Vector3.zero;
                selfTransform.position = hitInfo.point + (hitInfo.point - selfTransform.position).normalized * Ratio;
            }
            lastPosition = selfTransform.position;
        }

        private void CheckForClampVelocity()
        {
            if (RigidbodyComponent.velocity.magnitude > Constants.AttractableShootPower)
            {
                RigidbodyComponent.velocity = RigidbodyComponent.velocity.normalized * Utils.Constants.AttractableShootPower;
            }
            if (RigidbodyComponent.angularVelocity.magnitude > Constants.AttractableShootPower)
            {
                RigidbodyComponent.angularVelocity = RigidbodyComponent.angularVelocity.normalized * Utils.Constants.AttractableShootPower;
            }
        }

        private void GrabListener(Hand hand, Grabbable grabbable)
        {
            if (IsAlreadyOrbiting)
            {
                handObjectsAttraction.DetachAttractedObject(this);
            }
            if (DestroyIfNotInUseComponent != null)
            {
                DestroyIfNotInUseComponent.InUse = true;
            }
            IsGrabbed = true;
        }

        private void ReleaseListener(Hand hand, Grabbable grabbable)
        {
            if (DestroyIfNotInUseComponent != null)
            {
                DestroyIfNotInUseComponent.InUse = false;
            }
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

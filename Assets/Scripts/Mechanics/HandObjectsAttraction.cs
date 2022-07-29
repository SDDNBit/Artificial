using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class HandObjectsAttraction : MonoBehaviour
    {
        public const string DefaultLayer = "Default";
        public const float WaitSecondsBufferForHandGrab = 0.2f;

        [Tooltip("Avoid Default layer, it is used to remove already attracted objects from raycast")]
        [SerializeField] private LayerMask layers;
        [SerializeField] private Hand hand;
        [SerializeField] private List<Transform> orbitingPoints;

        private bool isGrabbing;
        private bool handIsBusy;
        private bool onGrabbedCalled;
        private bool attractionPerformed;

        private List<Transform> availableOrbitingPoints = new List<Transform>();
        private List<AttractableObject> objectsAttracted;
        private Collider[] hitColliders;
        private Coroutine onGrabbedWasCalledCoroutine;

        private void Start()
        {
            onGrabbedCalled = false;
            handIsBusy = true;
            attractionPerformed = false;
            hitColliders = new Collider[8];
            objectsAttracted = new List<AttractableObject>();
            if (layers == 0)
            {
                layers = LayerMask.GetMask(DefaultLayer);
            }
            AddAvailableOrbitPoints();
        }

        private void OnEnable()
        {
            hand.OnGrabbed += OnGrabbedListener;
        }

        private void OnDisable()
        {
            hand.OnGrabbed -= OnGrabbedListener;
        }

        private void Update()
        {
            CheckForObjectAttraction();
        }

        public virtual void StartPointing()
        {
            isGrabbing = true;
            onGrabbedWasCalledCoroutine = StartCoroutine(CheckIfOnGrabbedWasCalled());
        }

        public virtual void StopPointing()
        {
            isGrabbing = false;
            handIsBusy = true;
            onGrabbedCalled = false;
            if (onGrabbedWasCalledCoroutine != null)
            {
                StopCoroutine(onGrabbedWasCalledCoroutine);
            }
        }

        public virtual void SelectTarget()
        {
            StopPointing();
        }

        public virtual void CancelSelect()
        {
        }

        private void OnGrabbedListener(Hand hand, Grabbable grabbable)
        {
            onGrabbedCalled = true;
        }

        private IEnumerator CheckIfOnGrabbedWasCalled()
        {
            yield return new WaitForSeconds(WaitSecondsBufferForHandGrab);
            if (onGrabbedCalled)
            {
                handIsBusy = true;
            }
            else
            {
                handIsBusy = false;
            }
        }

        private void CheckForObjectAttraction()
        {
            if (isGrabbing && !handIsBusy)
            {
                if (!attractionPerformed)
                {
                    var collidersFound = Physics.OverlapSphereNonAlloc(transform.position, Utils.Constants.HandAttractionRange, hitColliders, layers, QueryTriggerInteraction.Ignore);
                    if (collidersFound > 0)
                    {
                        AttractObjects(collidersFound);
                    }
                    attractionPerformed = true;
                }
            }
            else
            {
                if (objectsAttracted.Count > 0)
                {
                    foreach (var objectAttracted in objectsAttracted)
                    {
                        ShootAttractedObject(objectAttracted);
                    }
                    CleanupOrbitPoints();
                }
                attractionPerformed = false;
            }
        }

        private void AttractObjects(int collidersFound)
        {
            for (var i = 0; i < collidersFound; ++i)
            {
                if (hitColliders[i].transform.TryGetComponent(out AttractableObject attractableObject))
                {
                    if (!attractableObject.IsAlreadyOrbiting && availableOrbitingPoints.Count > 0)
                    {
                        attractableObject.IsAlreadyOrbiting = true;
                        attractableObject.DestroyIfNotInUseComponent.InUse = true;
                        attractableObject.FlyToObjectComponent.Target = availableOrbitingPoints[0].transform;
                        attractableObject.gameObject.layer = LayerMask.NameToLayer(DefaultLayer);
                        availableOrbitingPoints.RemoveAt(0);
                        objectsAttracted.Add(attractableObject);
                    }
                }
            }
        }

        private void ShootAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.FlyToObjectComponent.Target = null;
            attractableObject.IsAlreadyOrbiting = false;
            attractableObject.DestroyIfNotInUseComponent.InUse = false;
            attractableObject.gameObject.layer = LayerMask.NameToLayer(Utils.Constants.AttractableObjectLayer);
            var attractableObjectRigidbody = attractableObject.GetComponent<Rigidbody>();
            attractableObject.transform.rotation = transform.rotation;
            attractableObjectRigidbody.velocity = transform.forward * Utils.Constants.AttractableShootPower;
        }

        private void AddAvailableOrbitPoints()
        {
            availableOrbitingPoints = new List<Transform>();
            availableOrbitingPoints.AddRange(orbitingPoints);
        }

        private void CleanupOrbitPoints()
        {
            AddAvailableOrbitPoints();
            objectsAttracted = new List<AttractableObject>();
        }
    }
}

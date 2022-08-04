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
        [Tooltip("If set to true, objects are attracted by player, pointing the hand towards them; if set to false, attractables around within a range are attracted automatically")]
        [SerializeField] private bool attractByPlayersHandPointing = true;
        [SerializeField] private List<Transform> orbitingPoints;

        private bool isGrabbing;
        private bool handIsBusy;
        private bool onGrabbedCalled;

        private List<Transform> availableOrbitingPoints = new List<Transform>();
        private List<AttractableObject> objectsAttracted;
        private Collider[] hitColliders;
        private Coroutine onGrabbedWasCalledCoroutine;
        private Transform myTransform;
        private RaycastHit hit;
        private int attractablesArround;

        private void Start()
        {
            myTransform = transform;
            onGrabbedCalled = false;
            attractablesArround = 0;
            handIsBusy = true;
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

        public virtual void StartAttracting()
        {
            isGrabbing = true;
            onGrabbedWasCalledCoroutine = StartCoroutine(CheckIfOnGrabbedWasCalled());
        }

        public virtual void StopAttracting()
        {
            isGrabbing = false;
            handIsBusy = true;
            onGrabbedCalled = false;
            if (onGrabbedWasCalledCoroutine != null)
            {
                StopCoroutine(onGrabbedWasCalledCoroutine);
            }
        }

        public void DetachAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.FlyToObjectComponent.Target = null;
            attractableObject.IsAlreadyOrbiting = false;
            attractableObject.gameObject.layer = LayerMask.NameToLayer(Utils.Constants.AttractableObjectLayer);
            objectsAttracted.Remove(attractableObject);
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
                if (attractByPlayersHandPointing)
                {
                    if (Physics.SphereCast(myTransform.position, Utils.Constants.AttractionRadius, myTransform.forward, out hit, Utils.Constants.HandAttractionRange, layers, QueryTriggerInteraction.Ignore))
                    {
                        AttractObjectByPlayersHand(hit);
                    }
                }
                else
                {
                    attractablesArround = Physics.OverlapSphereNonAlloc(transform.position, Utils.Constants.HandAttractionRange, hitColliders, layers, QueryTriggerInteraction.Ignore);
                    if (attractablesArround > 0)
                    {
                        AttractObjectsArroundPlayer(attractablesArround);
                    }
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
            }
        }

        private void AttractObjectByPlayersHand(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out AttractableObject attractableObject))
            {
                AttractObject(attractableObject);
            }
        }

        private void AttractObjectsArroundPlayer(int collidersFound)
        {
            for (var i = 0; i < collidersFound; ++i)
            {
                if (hitColliders[i].transform.TryGetComponent(out AttractableObject attractableObject))
                {
                    AttractObject(attractableObject);
                }
            }
        }

        private void AttractObject(AttractableObject attractableObject)
        {
            if (!attractableObject.IsAlreadyOrbiting && !attractableObject.IsGrabbed && availableOrbitingPoints.Count > 0)
            {
                attractableObject.IsAlreadyOrbiting = true;
                attractableObject.DestroyIfNotInUseComponent.InUse = true;
                attractableObject.FlyToObjectComponent.Target = availableOrbitingPoints[0].transform;
                attractableObject.gameObject.layer = LayerMask.NameToLayer(DefaultLayer);
                attractableObject.SetObjectAttractionComponent(this);
                availableOrbitingPoints.RemoveAt(0);
                objectsAttracted.Add(attractableObject);
            }
        }

        private void ShootAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.FlyToObjectComponent.Target = null;
            attractableObject.IsAlreadyOrbiting = false;
            attractableObject.DestroyIfNotInUseComponent.InUse = false;
            attractableObject.gameObject.layer = LayerMask.NameToLayer(Utils.Constants.AttractableObjectLayer);
            attractableObject.transform.rotation = transform.rotation;
            attractableObject.RigidbodyComponent.velocity = transform.forward * Utils.Constants.AttractableShootPower;
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

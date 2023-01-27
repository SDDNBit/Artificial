using Autohand;
using SoftBit.Autohand.Custom;
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

        private bool isAttracting;

        private List<Transform> availableOrbitingPoints = new();
        private List<AttractableObject> objectsAttracted = new();
        private Collider[] hitColliders;
        private Coroutine onGrabbedWasCalledCoroutine;
        private Transform myTransform;
        private RaycastHit hit;
        private int attractablesArround;
        private List<AttractableObject> targetedAttractables = new();

        private void Start()
        {
            myTransform = transform;
            attractablesArround = 0;
            hitColliders = new Collider[8];
            if (layers == 0)
            {
                layers = LayerMask.GetMask(DefaultLayer);
            }
            AddAvailableOrbitPoints();
        }

        private void Update()
        {
            CheckForObjectAttraction();
        }

        public virtual void StartAttracting()
        {
            isAttracting = true;
        }

        public virtual void StopAttracting()
        {
            isAttracting = false;
            if (onGrabbedWasCalledCoroutine != null)
            {
                StopCoroutine(onGrabbedWasCalledCoroutine);
            }
        }

        public void DetachAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.SetAttractableState(false, true, null);
            objectsAttracted.Remove(attractableObject);
        }

        private void CheckForObjectAttraction()
        {
            if (!hand.holdingObj && !hand.IsGrabbing())
            {
                if (availableOrbitingPoints.Count > 0)
                {
                    if (attractByPlayersHandPointing)
                    {
                        if (Physics.SphereCast(myTransform.position, Utils.Constants.AttractionRadius, myTransform.forward, out hit, Utils.Constants.HandAttractionRange, layers, QueryTriggerInteraction.Ignore))
                        {
                            AttractObjectByPlayersHand(hit);
                        }
                        else
                        {
                            RemoveTagetedAttractables();
                        }
                    }
                    else
                    {
                        attractablesArround = Physics.OverlapSphereNonAlloc(transform.position, Utils.Constants.HandAttractionRange, hitColliders, layers, QueryTriggerInteraction.Ignore);
                        if (attractablesArround > 0)
                        {
                            AttractObjectsArroundPlayer(attractablesArround);
                        }
                        else
                        {
                            RemoveTagetedAttractables();
                        }
                    }
                }
                else
                {
                    RemoveTagetedAttractables();
                }

                if (!isAttracting && objectsAttracted.Count > 0)
                {
                    foreach (var objectAttracted in objectsAttracted)
                    {
                        ShootAttractedObject(objectAttracted);
                    }
                    CleanupOrbitPoints();
                }
            }
        }

        private void RemoveTagetedAttractables()
        {
            foreach (var attractable in targetedAttractables)
            {
                attractable.SetTargetOnAttractable(false);
            }
            targetedAttractables.Clear();
        }

        private void AttractObjectByPlayersHand(RaycastHit hit)
        {
            if (hit.transform.TryGetComponent(out AttractableObject attractableObject))
            {
                AttractOrTargetTheAttractable(attractableObject);
            }
        }

        private void AttractObjectsArroundPlayer(int collidersFound)
        {
            for (var i = 0; i < collidersFound; ++i)
            {
                if (hitColliders[i].transform.TryGetComponent(out AttractableObject attractableObject))
                {
                    AttractOrTargetTheAttractable(attractableObject);
                }
            }
        }

        private void AttractOrTargetTheAttractable(AttractableObject attractableObject)
        {
            if (isAttracting)
            {
                if (!attractableObject.IsAlreadyOrbiting && !attractableObject.IsGrabbed)
                {
                    if (targetedAttractables.Contains(attractableObject))
                    {
                        targetedAttractables.Remove(attractableObject);
                        attractableObject.SetTargetOnAttractable(false);
                    }
                    AttractObject(attractableObject);
                }
            }
            else
            {
                if (!attractableObject.IsAlreadyOrbiting && !attractableObject.IsGrabbed)
                {
                    if (!targetedAttractables.Contains(attractableObject))
                    {
                        targetedAttractables.Add(attractableObject);
                        attractableObject.SetTargetOnAttractable(true);
                    }
                }
            }
        }

        private void AttractObject(AttractableObject attractableObject)
        {
            attractableObject.SetAttractableState(true, true, availableOrbitingPoints[0].transform);
            attractableObject.SetObjectAttractionComponent(this);
            availableOrbitingPoints.RemoveAt(0);
            objectsAttracted.Add(attractableObject);
        }

        private void ShootAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.SetAttractableState(false, false, null);
            attractableObject.transform.rotation = transform.rotation;
            attractableObject.RigidbodyComponent.velocity = transform.forward * Utils.Constants.AttractableShootPower;
            AddSmasherOnShootBehaviour(attractableObject);
        }

        private void AddSmasherOnShootBehaviour(AttractableObject attractableObject)
        {
            if (attractableObject.AddSmasherOnShoot)
            {
                var smasher = attractableObject.gameObject.GetComponent<SmasherCustom>();
                if (smasher == null)
                {
                    var newSmasher = attractableObject.gameObject.AddComponent<SmasherCustom>();
                    newSmasher.SelfRemovable = true;
                    newSmasher.RemoveAfterSeconds = Utils.Constants.AddSmasherOnShootRemoveAfterSeconds;
                }
                else
                {
                    smasher.SelfRemovable = true;
                    smasher.RemoveAfterSeconds = Utils.Constants.AddSmasherOnShootRemoveAfterSeconds;
                    smasher.ResetSelfRemoveClock();
                }
            }
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

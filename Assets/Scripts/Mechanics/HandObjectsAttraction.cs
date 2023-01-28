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
        private Dictionary<AttractableObject, Transform> objectsAttractedWithOrbitingPoint = new();
        private Collider[] hitColliders;
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

        public void ShouldAttract(bool isAttracting)
        {
            this.isAttracting = isAttracting;
        }

        public void DetachAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.SetAttractableState(false, true, null);
            availableOrbitingPoints.Add(objectsAttractedWithOrbitingPoint[attractableObject]);
            objectsAttractedWithOrbitingPoint.Remove(attractableObject);
        }

        private void CheckForObjectAttraction()
        {
            if (!hand.holdingObj && !hand.IsGrabbing())
            {
                if (availableOrbitingPoints.Count > 0)
                {
                    if (attractByPlayersHandPointing)
                    {
                        HandPointingAttraction();
                    }
                    else
                    {
                        SurroundingAttraction();
                    }
                }
                else
                {
                    RemoveTagetedAttractables();
                }

                if (!isAttracting && objectsAttractedWithOrbitingPoint.Count > 0)
                {
                    foreach (var objectAttracted in objectsAttractedWithOrbitingPoint)
                    {
                        ShootAttractedObject(objectAttracted.Key);
                    }
                    CleanupOrbitPoints();
                }
            }
        }

        private void HandPointingAttraction()
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

        private void SurroundingAttraction()
        {
            attractablesArround = Physics.OverlapSphereNonAlloc(myTransform.position, Utils.Constants.HandAttractionRange, hitColliders, layers, QueryTriggerInteraction.Ignore);
            if (attractablesArround > 0)
            {
                AttractObjectsArroundPlayer(attractablesArround);
            }
            else
            {
                RemoveTagetedAttractables();
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
            objectsAttractedWithOrbitingPoint.Add(attractableObject, availableOrbitingPoints[0].transform);
            availableOrbitingPoints.RemoveAt(0);
        }

        private void ShootAttractedObject(AttractableObject attractableObject)
        {
            attractableObject.SetAttractableState(false, false, null);
            attractableObject.transform.rotation = myTransform.rotation;
            attractableObject.RigidbodyComponent.velocity = myTransform.forward * Utils.Constants.AttractableShootPower;
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
            availableOrbitingPoints.Clear();
            availableOrbitingPoints.AddRange(orbitingPoints);
        }

        private void CleanupOrbitPoints()
        {
            AddAvailableOrbitPoints();
            objectsAttractedWithOrbitingPoint.Clear();
        }
    }
}

using Autohand;
using SoftBit.Autohand.Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants = SoftBit.Utils.Constants;

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

        private List<Transform> availableOrbitingPoints = new List<Transform>();
        private Dictionary<AttractableObject, Transform> objectsAttractedWithOrbitingPoint = new Dictionary<AttractableObject, Transform>();
        private Transform myTransform;

        private void Start()
        {
            myTransform = transform;
            if (layers == 0)
            {
                layers = LayerMask.GetMask(DefaultLayer);
            }
            AddAvailableOrbitPoints();
        }

        private void Update()
        {
            CheckForShooting();
        }

        private void OnTriggerEnter(Collider other)
        {
            HandPointingAttraction(other);
        }

        private void OnTriggerStay(Collider other)
        {
            HandPointingAttraction(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!hand.holdingObj && !hand.IsGrabbing())
            {
                if (other.attachedRigidbody && other.attachedRigidbody.transform.TryGetComponent(out AttractableObject attractableObject))
                {
                    RemoveTagetedAttractable(attractableObject);
                }
            }
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

        private void CheckForShooting()
        {
            if (!isAttracting && objectsAttractedWithOrbitingPoint.Count > 0)
            {
                foreach (var objectAttracted in objectsAttractedWithOrbitingPoint)
                {
                    ShootAttractedObject(objectAttracted.Key);
                }
                CleanupOrbitPoints();
            }
        }

        private void HandPointingAttraction(Collider other)
        {
            if (!hand.holdingObj && !hand.IsGrabbing())
            {
                if (other.attachedRigidbody && other.attachedRigidbody.transform.TryGetComponent(out AttractableObject attractableObject))
                {
                    AttractOrTargetTheAttractable(attractableObject);
                }
            }
        }
           
        private void RemoveTagetedAttractable(AttractableObject attractableObject)
        {
            attractableObject.SetTargetOnAttractable(false);
        }

        private void AttractOrTargetTheAttractable(AttractableObject attractableObject)
        {
            if (isAttracting)
            {
                attractableObject.SetTargetOnAttractable(false);
                if (availableOrbitingPoints.Count > 0 && !attractableObject.IsAlreadyOrbiting && !attractableObject.IsGrabbed)
                {
                    AttractObject(attractableObject);
                }
            }
            else
            {
                if (!attractableObject.IsAlreadyOrbiting && !attractableObject.IsGrabbed && availableOrbitingPoints.Count > 0)
                {
                    attractableObject.SetTargetOnAttractable(true);
                }
                else
                {
                    attractableObject.SetTargetOnAttractable(false);
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
            attractableObject.RigidbodyComponent.angularVelocity = Vector3.zero;
            attractableObject.RigidbodyComponent.velocity = myTransform.forward * Constants.AttractableShootPower;
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
                    newSmasher.RemoveAfterSeconds = Constants.AddSmasherOnShootRemoveAfterSeconds;
                }
                else
                {
                    smasher.SelfRemovable = true;
                    smasher.RemoveAfterSeconds = Constants.AddSmasherOnShootRemoveAfterSeconds;
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

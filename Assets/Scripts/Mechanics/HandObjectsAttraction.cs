using Autohand;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class HandObjectsAttraction : MonoBehaviour
    {
        public const string DefaultLayer = "Default";

        [Tooltip("Avoid Default layer, it is used to remove already attracted objects from raycast")]
        [SerializeField] private LayerMask layers;

        [SerializeField] private List<Transform> orbitingPoints;

        private bool isGrabbing;
        private bool isTriggering;
        private bool attractionPerformed;
        private List<Transform> availableOrbitingPoints = new List<Transform>();
        private List<AttractableObject> objectsAttracted;
        private Collider[] hitColliders;

        private void Start()
        {
            attractionPerformed = false;
            hitColliders = new Collider[8];
            objectsAttracted = new List<AttractableObject>();
            if (layers == 0)
            {
                layers = LayerMask.GetMask(DefaultLayer);
            }
            AddAvailableOrbitPoints();
        }

        private void Update()
        {
            CheckDistanceGrabbable();
        }

        public virtual void StartPointing()
        {
            isGrabbing = true;
        }

        public virtual void StopPointing()
        {
            isGrabbing = false;
        }

        public virtual void SelectTarget()
        {
            isTriggering = true;
            StopPointing();
        }

        public virtual void CancelSelect()
        {
            isTriggering = false;
        }


        private void CheckDistanceGrabbable()
        {
            if (isGrabbing)
            {
                if (!attractionPerformed)
                {
                    var collidersFound = Physics.OverlapSphereNonAlloc(transform.position, Utils.Constants.HandAttractionRange, hitColliders, layers, QueryTriggerInteraction.Ignore);
                    if (collidersFound > 0)
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
                    attractionPerformed = true;
                }
            }
            else
            {
                if (objectsAttracted.Count > 0)
                {
                    foreach (var objectAttracted in objectsAttracted)
                    {
                        objectAttracted.FlyToObjectComponent.Target = null;
                        objectAttracted.IsAlreadyOrbiting = false;
                        objectAttracted.DestroyIfNotInUseComponent.InUse = false;
                        objectAttracted.gameObject.layer = LayerMask.NameToLayer(Utils.Constants.AttractableObjectLayer);
                        ShootAttractedObject(objectAttracted);
                    }
                    AddAvailableOrbitPoints();
                    objectsAttracted = new List<AttractableObject>();
                }
                attractionPerformed = false;
            }
        }

        private void ShootAttractedObject(AttractableObject attractableObject)
        {
            var attractableObjectRigidbody = attractableObject.GetComponent<Rigidbody>();
            attractableObject.transform.rotation = transform.rotation;
            attractableObjectRigidbody.velocity = transform.forward * Utils.Constants.AttractableShootPower;
        }

        private void AddAvailableOrbitPoints()
        {
            availableOrbitingPoints = new List<Transform>();
            availableOrbitingPoints.AddRange(orbitingPoints);
        }
    }
}

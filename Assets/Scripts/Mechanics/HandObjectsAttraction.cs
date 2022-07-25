using Autohand;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class HandObjectsAttraction : MonoBehaviour
    {
        public const string DefaultLayer = "Default";
        [SerializeField] private Hand hand;

        [Header("Pointing Options")]
        [SerializeField] private Transform forwardPointer;
        [SerializeField] private LineRenderer line;

        [Tooltip("Avoid Default layer, it is used to remove already attracted objects from raycast")]
        [SerializeField] private LayerMask layers;

        [SerializeField] private List<Transform> orbitingPoints;

        private RaycastHit hit;

        private bool isGrabbing;
        private bool isTriggering;
        private List<Transform> availableOrbitingPoints = new List<Transform>();
        private List<AttractableObject> objectsAttracted;

        //private void Awake()
        //{
        //    hand = GetComponent<Hand>();
        //}

        private void Start()
        {
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
            if (line != null)
            {
                line.positionCount = 0;
                line.SetPositions(new Vector3[0]);
            }
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
                bool didHit = Physics.SphereCast(forwardPointer.position, Utils.Constants.AttractionRadius, forwardPointer.forward, out hit, Utils.Constants.HandMaxObjectAttractionRange, layers);
                if (didHit)
                {
                    if (hit.transform.TryGetComponent(out AttractableObject attractableObject))
                    {
                        if (!attractableObject.IsAlreadyOrbiting && availableOrbitingPoints.Count > 0)
                        {
                            attractableObject.IsAlreadyOrbiting = true;
                            attractableObject.FlyToObjectComponent.Target = availableOrbitingPoints[0].transform;
                            attractableObject.gameObject.layer = LayerMask.NameToLayer(DefaultLayer);
                            availableOrbitingPoints.RemoveAt(0);
                            objectsAttracted.Add(attractableObject);
                        }
                    }
                }
                if (line != null)
                {
                    if (didHit)
                    {
                        line.positionCount = 2;
                        line.SetPositions(new Vector3[] { forwardPointer.position, hit.point });
                    }
                    else
                    {
                        line.positionCount = 2;
                        line.SetPositions(new Vector3[] { forwardPointer.position, forwardPointer.position + forwardPointer.forward * Utils.Constants.HandMaxObjectAttractionRange });
                    }
                }
            }
            else
            {
                if (objectsAttracted.Count > 0)
                {
                    foreach(var objectAttracted in objectsAttracted)
                    {
                        objectAttracted.FlyToObjectComponent.Target = null;
                        objectAttracted.IsAlreadyOrbiting = false;
                        objectAttracted.gameObject.layer = LayerMask.NameToLayer(Utils.Constants.AttractableObjectLayer);
                        ShootAttractedObject(objectAttracted);
                    }
                    AddAvailableOrbitPoints();
                    objectsAttracted = new List<AttractableObject>();
                }
            }
        }

        private void ShootAttractedObject(AttractableObject attractableObject)
        {
            var attractableObjectRigidbody = attractableObject.GetComponent<Rigidbody>();
            attractableObject.transform.rotation = forwardPointer.rotation;
            attractableObjectRigidbody.velocity = transform.forward * Utils.Constants.AttractableShootPower;
            //attractableObjectRigidbody.velocity = hand.ThrowVelocity() * Utils.Constants.AttractableThrowingPower;
            //var throwAngularVel = hand.ThrowAngularVelocity();
            //if (!float.IsNaN(throwAngularVel.x) && !float.IsNaN(throwAngularVel.y) && !float.IsNaN(throwAngularVel.z))
            //    attractableObjectRigidbody.angularVelocity = throwAngularVel;
        }

        private void AddAvailableOrbitPoints()
        {
            availableOrbitingPoints = new List<Transform>();
            availableOrbitingPoints.AddRange(orbitingPoints);
        }
    }
}

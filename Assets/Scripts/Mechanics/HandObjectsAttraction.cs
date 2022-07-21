using Autohand;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class HandObjectsAttraction : MonoBehaviour
    {
        public const string DefaultLayer = "Default";
        
        [Header("Pointing Options")]
        [SerializeField] private Transform forwardPointer;
        [SerializeField] private LineRenderer line;

        [Header("Attraction")]
        [SerializeField] private float maxRange = 5;
        [SerializeField] private float attractionRadius = 0.03f;
        [Tooltip("Defaults to Default layer on start if none")]
        [SerializeField] private LayerMask layers;

        [Header("GravityZone")]
        [SerializeField] private List<Transform> orbitingPoints;

        private RaycastHit hit;

        private bool pointing;
        private bool pulling;
        private List<Transform> availableOrbitingPoints = new List<Transform>();

        private void Start()
        {
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
            pointing = true;
        }

        public virtual void StopPointing()
        {
            pointing = false;
            if (line != null)
            {
                line.positionCount = 0;
                line.SetPositions(new Vector3[0]);
            }
        }

        public virtual void SelectTarget()
        {
            pulling = true;
            StopPointing();
        }

        public virtual void CancelSelect()
        {
            pulling = false;
        }


        private void CheckDistanceGrabbable()
        {
            if (!pulling && pointing)
            {
                bool didHit = Physics.SphereCast(forwardPointer.position, attractionRadius, forwardPointer.forward, out hit, maxRange, layers);
                if (didHit)
                {
                    if (hit.transform.TryGetComponent(out AttractableObject attractableObject))
                    {
                        if (!attractableObject.IsAlreadyOrbiting && availableOrbitingPoints.Count > 0)
                        {
                            attractableObject.IsAlreadyOrbiting = true;
                            attractableObject.FlyToObjectComponent.Target = availableOrbitingPoints[0].transform;
                            availableOrbitingPoints.RemoveAt(0);
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
                        line.SetPositions(new Vector3[] { forwardPointer.position, forwardPointer.position + forwardPointer.forward * maxRange });
                    }
                }
            }
            else if (pulling)
            {
                // shoot with attracted objects
            }
        }

        private void AddAvailableOrbitPoints()
        {
            availableOrbitingPoints.AddRange(orbitingPoints);
        }
    }
}

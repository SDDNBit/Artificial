using Autohand;
using SoftBit.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class BreakApart : MonoBehaviour
    {
        public List<ConnectionPart> connectionParts;
        [SerializeField] private GameObject prefab;

        private List<ConnectionPart> partsToSpawn;
        private int parentsActive;
        private int granpasActive;
        private GameObject scrap;
        private BreakApart scrapBreakApart;
        private Transform selfTransform;

        private void Awake()
        {
            selfTransform = transform;
            partsToSpawn = new List<ConnectionPart>();
            foreach (var connectionPart in connectionParts)
            {
                connectionPart.breakApart = this;
            }
        }

        public void DestroyPart(ConnectionPart connectionPart, Collision collision)
        {
            partsToSpawn.Clear();
            if (collision != null)
            {
                var explosionForce = collision.relativeVelocity.magnitude * Utils.Constants.CollisionForceMultiplier;
            }
            foreach (var cell in connectionPart.cells)
            {
                //cell.BakeMesh(explosionForce, collision.GetContact(0).point);
                cell.BakeMesh();
            }
            DeactivatePart(connectionPart);
            print(partsToSpawn.Count);
            partsToSpawn.RemoveAt(0);
            if (partsToSpawn.Count > 0)
            {
                scrap = Instantiate(prefab);
                scrap.AddComponent<Rigidbody>();
                scrap.AddComponent<Grabbable>();
                scrapBreakApart = scrap.GetComponent<BreakApart>();
                scrapBreakApart.SetActiveParts(partsToSpawn);
                scrapBreakApart.Cleanup();
                scrap.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
            DestroyIfGarbage();
        }

        public void Cleanup()
        {
            foreach (var connectionPart in connectionParts)
            {
                if (connectionPart.colliders != null && connectionPart.colliders.Count > 0)
                {
                    foreach (var collider in connectionPart.colliders)
                    {
                        if (collider != null)
                        {
                            var enemyCollider = collider.GetComponent<EnemyCollider>();
                            if (enemyCollider)
                            {
                                enemyCollider.AttachedObject = null;
                            }
                        }
                    }
                }
            }
        }

        public void SetActiveParts(List<ConnectionPart> activeParts)
        {
            foreach (var connectionPart in connectionParts)
            {
                var found = false;
                foreach (var activePart in activeParts)
                {
                    if (activePart.guid.Equals(connectionPart.guid))
                    {
                        connectionPart.gameObject.SetActive(true);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    connectionPart.gameObject.SetActive(false);
                    RemoveColliders(connectionPart);
                }
            }
        }

        private void DestroyIfGarbage()
        {
            foreach (var connectionPart in connectionParts)
            {
                if (connectionPart.gameObject.activeSelf)
                {
                    return;
                }
            }
            Destroy(gameObject);
        }

        private void CheckForDeactivatePart(ConnectionPart connectionPart)
        {
            if (connectionPart.gameObject.activeSelf)
            {
                parentsActive = 0;
                foreach (var parentPart in connectionPart.parentParts)
                {
                    if (parentPart.gameObject.activeSelf)
                    {
                        ++parentsActive;
                        if (parentPart.parentParts.Count > 0)
                        {
                            granpasActive = 0;
                            foreach (var grandpaPart in parentPart.parentParts)
                            {
                                if (grandpaPart.gameObject.activeSelf && grandpaPart != connectionPart)
                                {
                                    ++granpasActive;
                                }
                            }
                            if (granpasActive == 0)
                            {
                                DeactivateSibling(parentPart);
                                parentsActive = 0;
                            }
                        }
                    }
                }
                if (parentsActive == 0)
                {
                    DeactivatePart(connectionPart);
                }
            }
        }

        private void DeactivatePart(ConnectionPart connectionPart)
        {
            partsToSpawn.Add(connectionPart);
            connectionPart.gameObject.SetActive(false);
            RemoveColliders(connectionPart);
            DeactivateChilds(connectionPart);
        }

        private void RemoveColliders(ConnectionPart connectionPart)
        {
            if (connectionPart.colliders != null && connectionPart.colliders.Count > 0)
            {
                foreach (var collider in connectionPart.colliders)
                {
                    if (collider != null)
                    {
                        Destroy(collider.transform.gameObject);
                    }
                }
            }
        }

        private void DeactivateSibling(ConnectionPart sibling)
        {
            DeactivatePart(sibling);
        }

        private void DeactivateChilds(ConnectionPart connectionPart)
        {
            foreach (var childPart in connectionPart.childParts)
            {
                CheckForDeactivatePart(childPart);
            }
        }

        [ContextMenu("PreAssignAllConnectionParts")]
        private void PreAssignAllConnectionParts()
        {
            connectionParts = new List<ConnectionPart>();
            connectionParts.AddRange(gameObject.GetComponentsInChildren<ConnectionPart>());
        }
    }
}

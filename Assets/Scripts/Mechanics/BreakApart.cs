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
        [SerializeField] private CollisionDetectionMode collisionDetectionModeForParts;

        private List<ConnectionPart> partsToActivate;
        private int parentsActive;
        private int granpasActive;
        private GameObject prefabClone;
        private BreakApart scrapBreakApart;
        private Transform selfTransform;
        private Ragdoll ragdoll;
        private bool partFound;

        private void Awake()
        {
            ragdoll = GetComponent<Ragdoll>();
            selfTransform = transform;
            partsToActivate = new List<ConnectionPart>();
        }

        private void Start()
        {
            //connectionParts.RemoveAll(connectionPart => connectionPart == null);
            foreach (var connectionPart in connectionParts)
            {
                connectionPart.breakApart = this;
            }
        }

        public void DestroyPart(ConnectionPart connectionPart, Collision collision)
        {
            partsToActivate.Clear();
            var explosionForce = Utils.Constants.CollisionForceMultiplier;
            if (collision != null)
            {
                explosionForce *= collision.relativeVelocity.magnitude;
            }
            foreach (var cell in connectionPart.cells)
            {
                cell.BakeMesh(explosionForce, collision != null ? collision.GetContact(0).point : cell.transform.position);
            }
            DeactivatePart(connectionPart);
            //partsToActivate.RemoveAt(0);
            if (partsToActivate.Count > 1)
            {
                prefabClone = Instantiate(prefab); // this will clone this object at this moment
                SetRigidbodyOnPrefabClone(prefabClone);
                SetGrabbableOnPrefabClone(prefabClone);
                scrapBreakApart = prefabClone.GetComponent<BreakApart>();
                scrapBreakApart.SetupPrefabClone(partsToActivate);
                //scrapBreakApart.Cleanup();
                prefabClone.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
            DestroyIfGarbage();
        }

        //private void Cleanup()
        //{
        //    foreach (var connectionPart in connectionParts)
        //    {
        //        if (connectionPart.colliders != null && connectionPart.colliders.Count > 0)
        //        {
        //            foreach (var collider in connectionPart.colliders)
        //            {
        //                if (collider != null)
        //                {
        //                    var enemyCollider = collider.GetComponent<EnemyCollider>();
        //                    if (enemyCollider)
        //                    {
        //                        enemyCollider.AttachedObject = null;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private void SetupPrefabClone(List<ConnectionPart> partsToActivateFromPrefab)
        {
            foreach (var part in partsToActivateFromPrefab)
            {
                partsToActivate.Add(connectionParts.Find(connectionPart => connectionPart.guid == part.guid));
            }
            AdjustRagdollToWorkProperly(partsToActivate);
            RemovePartThatJustGotDestroyed(partsToActivate);

            for (var i = 0; i < connectionParts.Count; ++i)
            {
                partFound = false;
                foreach (var activePart in partsToActivate)
                {
                    if (activePart.guid.Equals(connectionParts[i].guid))
                    {
                        partFound = true;
                        connectionParts[i].gameObject.SetActive(true);
                        break;
                    }
                }
                if (!partFound)
                {
                    RemoveColliders(connectionParts[i]);
                    DestroyConnectionPart(connectionParts[i]);
                    i = -1;
                }
            }

            //foreach (var connectionPart in connectionParts)
            //{
            //    var found = false;
            //    foreach (var activePart in partsToActivate)
            //    {
            //        if (activePart.guid.Equals(connectionPart.guid))
            //        {
            //            connectionPart.gameObject.SetActive(true);
            //            found = true;
            //            break;
            //        }
            //    }
            //    //if (!found)
            //    //{
            //    //    //connectionPart.gameObject.SetActive(false);
            //    //    //RemoveColliders(connectionPart);
            //    //    //DestroyConnectionPart(connectionPart);
            //    //}
            //    if (found)
            //    {
            //        //if (connectionPart.colliders != null && connectionPart.colliders.Count > 0)
            //        //{
            //        //    foreach (var collider in connectionPart.colliders)
            //        //    {
            //        //        if (collider != null)
            //        //        {
            //        //            var enemyCollider = collider.GetComponent<EnemyCollider>();
            //        //            if (enemyCollider)
            //        //            {
            //        //                if (enemyCollider.RagdollRigidbodyToApplyForceTo != null)
            //        //                {
            //        //                    enemyCollider.RagdollRigidbodyToApplyForceTo.isKinematic = false;
            //        //                }
            //        //                enemyCollider.AttachedObject = null;
            //        //            }
            //        //        }
            //        //    }
            //        //}
            //    }
            //    else
            //    {
            //        //if (connectionPart.colliders != null && connectionPart.colliders.Count > 0)
            //        //{
            //        //    foreach (var collider in connectionPart.colliders)
            //        //    {
            //        //        if (collider != null)
            //        //        {
            //        //            var enemyCollider = collider.GetComponent<EnemyCollider>();
            //        //            if (enemyCollider)
            //        //            {
            //        //                enemyCollider.RemoveJointAndRigidbody();
            //        //            }
            //        //            Destroy(collider.gameObject);
            //        //        }
            //        //    }
            //        //}

            //    }
            //}
            //DestroyConnectionPart(connectionPart);

            //connectionParts.RemoveAll(connectionPart => connectionPart == null);
        }

        private void AdjustRagdollToWorkProperly(List<ConnectionPart> partsToActivate)
        {
            if (partsToActivate.Count > 2)
            {
                ragdoll.SetRagdollForScrap(partsToActivate);
            }
        }

        private void RemovePartThatJustGotDestroyed(List<ConnectionPart> partsToActivate)
        {
            partsToActivate.RemoveAt(0);
        }

        private void SetGrabbableOnPrefabClone(GameObject scrap)
        {
            Grabbable grabbable;
            if (!scrap.CanGetComponent(out grabbable))
            {
                scrap.AddComponent<Grabbable>();
            }
        }

        private void SetRigidbodyOnPrefabClone(GameObject scrap)
        {
            Rigidbody scrapRigidbody;
            if (!scrap.CanGetComponent(out scrapRigidbody))
            {
                scrapRigidbody = scrap.AddComponent<Rigidbody>();
            }
            SetScrapRigidbodyProperties(scrapRigidbody);
        }

        private void SetScrapRigidbodyProperties(Rigidbody scrapRigidbody)
        {
            scrapRigidbody.useGravity = true;
            scrapRigidbody.isKinematic = false;
            scrapRigidbody.collisionDetectionMode = collisionDetectionModeForParts;
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

        /// <summary>
        /// This is run on the object itself and not on the instantiated scrap one when it is called first time.
        /// </summary>
        /// <param name="connectionPart"></param>
        private void DeactivatePart(ConnectionPart connectionPart)
        {
            partsToActivate.Add(connectionPart);
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
                        Destroy(collider.gameObject);
                    }
                }
            }
        }

        private void DestroyConnectionPart(ConnectionPart connectionPart)
        {
            foreach (var cell in connectionPart.cells)
            {
                Destroy(cell.gameObject);
            }
            connectionParts.Remove(connectionPart);
            Destroy(connectionPart.gameObject);
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

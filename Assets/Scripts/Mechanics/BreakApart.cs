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

        private void Awake()
        {
            partsToSpawn = new List<ConnectionPart>();
            foreach (var connectionPart in connectionParts)
            {
                connectionPart.breakApart = this;
            }
        }

        public void DestroyPart(ConnectionPart connectionPart)
        {
            partsToSpawn.Clear();
            DeactivatePart(connectionPart);
            print(partsToSpawn.Count);
            partsToSpawn.RemoveAt(0);
            if (partsToSpawn.Count > 0)
            {
                scrap = Instantiate(prefab);
                scrap.GetComponent<Rigidbody>().isKinematic = false;
                scrapBreakApart = scrap.GetComponent<BreakApart>();
                scrapBreakApart.SetActiveParts(partsToSpawn);
                var animator = scrap.GetComponent<Animator>();
                if (animator != null)
                {
                    Destroy(animator);
                }
                scrap.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
            DestroyIfGarbage();
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
                    if (connectionPart.enemyCollider != null)
                    {
                        connectionPart.enemyCollider.DestroyCollider();
                    }
                }
            }
        }

        private void DestroyIfGarbage()
        {
            foreach(var connectionPart in connectionParts)
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
            DeactivateChilds(connectionPart);
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

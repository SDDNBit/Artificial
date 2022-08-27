using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class ConnectionPart : MonoBehaviour
    {
        public List<ConnectionPart> parentParts;
        public List<ConnectionPart> childParts;

        private int parentsActive;
        private int granpasActive;

        [ContextMenu("DestroyPart")]
        public void DestroyPart()
        {
            DeactivatePart();
            // create mesh with proper parts for detachment
        }

        public void CheckForDeactivatePart()
        {
            if (gameObject.activeSelf)
            {
                parentsActive = 0;
                foreach (var parentPart in parentParts)
                {
                    if (parentPart.gameObject.activeSelf)
                    {
                        ++parentsActive;
                        if (parentPart.parentParts.Count > 0)
                        {
                            granpasActive = 0;
                            foreach (var grandpaPart in parentPart.parentParts)
                            {
                                if (grandpaPart.gameObject.activeSelf && grandpaPart != this)
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
                    DeactivatePart();
                }
            }
        }

        public void DeactivatePart()
        {
            gameObject.SetActive(false);
            DeactivateChilds();
        }

        private void DeactivateSibling(ConnectionPart sibling)
        {
            sibling.DeactivatePart();
        }

        private void DeactivateChilds()
        {
            foreach (var childPart in childParts)
            {
                childPart.CheckForDeactivatePart();
            }
        }
    }
}
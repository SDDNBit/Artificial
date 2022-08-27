using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class BreakApart : MonoBehaviour
    {
        public List<ConnectionPart> connectionParts;

        private List<ConnectionPart> partsToSpawn;

        [ContextMenu("PreAssignAllConnectionParts")]
        private void PreAssignAllConnectionParts()
        {
            connectionParts = new List<ConnectionPart>();
            connectionParts.AddRange(gameObject.GetComponentsInChildren<ConnectionPart>());
        }
    }
}

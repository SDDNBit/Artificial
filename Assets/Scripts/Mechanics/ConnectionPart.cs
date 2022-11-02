using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class ConnectionPart : MonoBehaviour
    {
        public List<Collider> colliders;
        public string guid;
        public List<ConnectionPart> parentParts;
        public List<ConnectionPart> childParts;
        public List<BakeToMesh> cells;
        [HideInInspector] public BreakApart breakApart;

        [ContextMenu("DestroyPart")]
        public void DestroyPart()
        {
            breakApart.DestroyPart(this);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class ConnectionPart : MonoBehaviour
    {
        public List<EnemyCollider> EnemyColliders;
        public string guid;
        public List<ConnectionPart> parentParts;
        public List<ConnectionPart> childParts;
        public List<BakeToMesh> cells;
        [HideInInspector] public BreakApart breakApart;

        public void DestroyPart(Collision collision)
        {
            breakApart.DestroyPart(this, collision);
        }
    }
}
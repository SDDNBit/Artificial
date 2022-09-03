using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
        [SerializeField] private ConnectionPart connectionPart;

        [ContextMenu("DestroyPart")]
        public void DestroyPart()
        {
            connectionPart.DestroyPart();
        }

        public void DestroyCollider()
        {
            Destroy(gameObject);
        }
    }
}
using SoftBit.Autohand.Custom;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
        public FollowPivot AttachedObject;

        [SerializeField] private ConnectionPart connectionPart;

        [HideInInspector] public bool IsDestroyed = false;

        public void DestroyPart(Collision collision)
        {
            IsDestroyed = true;
            DetachAttachedObject();
            connectionPart.DestroyPart(collision);
        }

        private void DetachAttachedObject()
        {
            if (AttachedObject != null)
            {
                AttachedObject.PivotToFollow = null;
            }
        }
    }
}
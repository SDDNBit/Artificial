using SoftBit.Autohand.Custom;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
        public FollowPivot AttachedObject;

        [SerializeField] private ConnectionPart connectionPart;

        private Smash smash;

        private void Awake()
        {
            smash = GetComponent<Smash>();
            smash.OnSmash.AddListener(DestroyPart);
        }

        private void DestroyPart(Smasher smasher, Collision collision)
        {
            print("Destroy part");
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

        [ContextMenu("Test_DestroyPart")]
        private void DestroyTest()
        {
            DestroyPart(null, null);
        }
    }
}
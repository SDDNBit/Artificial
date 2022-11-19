using SoftBit.Autohand.Custom;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
        [SerializeField] private ConnectionPart connectionPart;
        [SerializeField] private FollowPivot attachedObject;

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
            if (attachedObject != null)
            {
                attachedObject.PivotToFollow = null;
            }
        }

        [ContextMenu("Test_DestroyPart")]
        private void DestroyTest()
        {
            DestroyPart(null, null);
        }
    }
}
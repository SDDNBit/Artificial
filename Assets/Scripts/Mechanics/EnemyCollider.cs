using SoftBit.Autohand.Custom;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class EnemyCollider : MonoBehaviour
    {
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
            connectionPart.DestroyPart(collision);
        }

        [ContextMenu("Test_DestroyPart")]
        private void DestroyTest()
        {
            DestroyPart(null, null);
        }
    }
}
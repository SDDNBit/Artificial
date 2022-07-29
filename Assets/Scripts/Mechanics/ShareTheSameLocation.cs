using UnityEngine;

namespace SoftBit.Mechanics
{
    public class ShareTheSameLocation : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private void FixedUpdate()
        {
            transform.position = target.position;
        }
    }
}

using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class ShareTheSameLocation : MonoBehaviour
    {
        public Transform Target;

        private void Update()
        {
            transform.position = Target ? Target.position : Constants.DefaultVisibleLocation;
        }

        public void ForceUpdatePosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}

using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class RotateMe : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;

        private void FixedUpdate()
        {
            transform.Rotate(direction, Time.deltaTime * Constants.OrbitingPointsSpeed);
        }
    }
}

using UnityEngine;

namespace SoftBit.Mechanics
{
    public class OrbitArround : MonoBehaviour
    {
        public Transform Pivot;
        public float Speed = 90f;
        public Vector3 direction;

        private void FixedUpdate()
        {
            transform.RotateAround(Pivot.position, direction, Time.deltaTime * Speed);
        }
    }
}

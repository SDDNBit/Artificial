using UnityEngine;

namespace SoftBit.Mechanics
{
    public class RotateMe : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        /// <summary>
        /// Be careful with this speed, if it is set to 200, then make sure the FlyToObject speed is set to .5f, otherwise jitter appear, 
        /// also, if you double one of them, make sure that you double the other one as well
        /// </summary>
        [SerializeField] private float speed = 200f;

        private void FixedUpdate()
        {
            transform.Rotate(direction, Time.deltaTime * speed);
        }
    }
}

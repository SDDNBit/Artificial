using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlyToObject : MonoBehaviour
    {
        public Transform Target;

        private Rigidbody rb;
        private float speed;

        private void Awake()
        {
            speed = Constants.FlyToObjectSpeed;
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Target)
            {
                transform.LookAt(Target.position);
                rb.velocity = transform.forward * speed;
            }
        }
    }
}

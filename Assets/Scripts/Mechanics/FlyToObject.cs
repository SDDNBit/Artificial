using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlyToObject : MonoBehaviour
    {
        public Transform Target;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (Target)
            {
                transform.LookAt(Target.position);
                rb.velocity = transform.forward *
                    Mathf.Clamp(Mathf.Abs(Vector3.Distance(Target.position, transform.position) * Constants.FlyToObjectMultiplier),
                    Constants.FlyToObjectMinSpeed,
                    Constants.FlyToObjectMaxSpeed);
            }
        }
    }
}

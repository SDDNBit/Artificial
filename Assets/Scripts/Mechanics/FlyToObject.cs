using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlyToObject : MonoBehaviour
    {
        public Transform Target;

        private Rigidbody rb;
        private Transform tr;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            tr = transform;
        }

        private void FixedUpdate()
        {
            if (Target)
            {
                rb.velocity = (Target.position - tr.position) *
                    Mathf.Clamp(Mathf.Abs(Vector3.Distance(Target.position, transform.position) * Constants.FlyToObjectMultiplier),
                    Constants.FlyToObjectMinSpeed,
                    Constants.FlyToObjectMaxSpeed);
            }
        }
    }
}

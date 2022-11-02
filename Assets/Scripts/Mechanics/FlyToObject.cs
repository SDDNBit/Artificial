using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlyToObject : MonoBehaviour
    {
        public Transform Target;

        private Rigidbody selfRigidbody;
        private Transform selfTransform;

        private void Awake()
        {
            selfRigidbody = GetComponent<Rigidbody>();
            selfTransform = transform;
        }

        private void FixedUpdate()
        {
            if (Target)
            {
                selfRigidbody.velocity = (Target.position - selfTransform.position) *
                    Mathf.Clamp(Mathf.Abs(Vector3.Distance(Target.position, transform.position) * Constants.FlyToObjectMultiplier),
                    Constants.FlyToObjectMinSpeed,
                    Constants.FlyToObjectMaxSpeed);
            }
        }
    }
}

using SoftBit.Utils;
using UnityEngine;

namespace SoftBit.Mechanics
{
    [RequireComponent(typeof(Rigidbody))]
    public class FlyToObject : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Rigidbody rigidbody;
        private float speed;

        private void Awake()
        {
            speed = Constants.FlyToObjectSpeed;
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            transform.LookAt(target.position);
            rigidbody.velocity = transform.forward * speed;
        }
    }
}

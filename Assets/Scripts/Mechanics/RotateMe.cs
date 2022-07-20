using UnityEngine;

namespace Mechanics
{
    public class RotateMe : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        [SerializeField] private float speed = 90f;

        private void FixedUpdate()
        {
            transform.Rotate(direction, Time.deltaTime * speed);
        }
    }
}

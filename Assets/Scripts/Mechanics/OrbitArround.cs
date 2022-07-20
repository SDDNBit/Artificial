using UnityEngine;

namespace Mechanics
{
    public class OrbitArround : MonoBehaviour
    {
        public Transform Pivot;
        public float Speed = 90f;
        public Vector3 direction;

        private void FixedUpdate()
        {
            transform.RotateAround(Pivot.position, direction, Time.deltaTime * Speed);

            ////Collider[] colliders = Physics.OverlapSphere(m_Pivot.position, m_Radius, m_Layers);

            //foreach (var collider in Colliders)
            //{
            //    //collider.transform.RotateAround()
            //    //Rigidbody body = collider.GetComponent<Rigidbody>();
            //    //if (body == null)
            //    //    continue;

            //    //Vector3 direction = m_Pivot.position - body.position;

            //    //float distance = direction.magnitude;

            //    //direction = direction.normalized;

            //    //if (distance < m_StopRadius)
            //    //    continue;

            //    //float forceRate = (m_Force / distance);

            //    //body.AddForce(direction * (forceRate / body.mass) * signal);
            //}
        }
    }
}

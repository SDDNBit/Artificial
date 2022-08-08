using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.IK
{
    public class FootSolverIK : MonoBehaviour
    {
        const float MaxDistance = 10f;
        const float FootSpacing = 0.114f;

        [SerializeField] private Transform body;
        [SerializeField] private Vector3 bodyOriginOffset;
        private Ray ray;
        private Transform myTransform;
        
        private void Awake()
        {
            myTransform = transform;
        }

        private void FixedUpdate()
        {
            ray = new Ray(body.position + bodyOriginOffset + (body.right * FootSpacing), Vector3.down);
            if(Physics.Raycast(ray, out RaycastHit raycastHit, MaxDistance))
            {
                myTransform.position = raycastHit.point;
            }
        }
    }
}

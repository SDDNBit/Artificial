using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class FollowPivot : MonoBehaviour
    {
        public Transform PivotToFollow;
        private Rigidbody selfRigidbody;

        private void Awake()
        {
            selfRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (PivotToFollow)
            {
                selfRigidbody.isKinematic = true;
                transform.position = PivotToFollow.position;
                transform.rotation = PivotToFollow.rotation;
            }
            else
            {
                selfRigidbody.isKinematic = false;
            }
        }
    }
}

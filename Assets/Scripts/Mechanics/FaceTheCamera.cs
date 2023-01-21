using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Mechanics
{
    public class FaceTheCamera : MonoBehaviour
    {
        private Transform mainCameraTransform;

        private void Awake()
        {
            mainCameraTransform = Camera.main.transform;    
        }

        private void LateUpdate()
        {
            transform.LookAt(mainCameraTransform);
        }
    }
}
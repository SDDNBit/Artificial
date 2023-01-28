using Autohand;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SoftBit.Autohand.Custom
{
    public delegate void SmashEvent(SmasherCustom smasher, SmashCustom smashable, Collision collision);

    [RequireComponent(typeof(Rigidbody))]
    public class SmasherCustom : MonoBehaviour
    {
        [Header("Options")]
        [Tooltip("How much to multiply the magnitude on smash")]
        public float forceMulti = 1;
        [Tooltip("Can be left empty - The center of mass point to calculate velocity magnitude - for example: the camera of the hammer is a better point vs the pivot center of the hammer object")]
        public Transform centerOfMassPoint;

        [Header("Cleanup")]
        public bool SelfRemovable = false;
        public float RemoveAfterSeconds = 0f;

        [HideInInspector] public GameObject LastCollisionGameObject;

        private Rigidbody rb;
        private Vector3[] velocityOverTime = new Vector3[3];
        private Vector3 lastPos;
        private Coroutine removeCoroutine;

        private void Start()
        {
            StartSelfRemove();
            rb = GetComponent<Rigidbody>();
        }
        private void FixedUpdate()
        {
            for (int i = 1; i < velocityOverTime.Length; i++)
            {
                velocityOverTime[i] = velocityOverTime[i - 1];
            }
            velocityOverTime[0] = lastPos - (centerOfMassPoint ? centerOfMassPoint.position : rb.position);

            lastPos = centerOfMassPoint ? centerOfMassPoint.position : rb.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            LastCollisionGameObject = collision.collider.gameObject;
        }

        public void ResetSelfRemoveClock()
        {
            if (removeCoroutine != null)
            {
                StopCoroutine(removeCoroutine);
            }
            StartSelfRemove();
        }

        public float GetMagnitude()
        {
            Vector3 velocity = Vector3.zero;
            for (int i = 0; i < velocityOverTime.Length; i++)
            {
                velocity += velocityOverTime[i];
            }

            return (velocity.magnitude / velocityOverTime.Length) * forceMulti * 10;
        }

        private void StartSelfRemove()
        {
            if (SelfRemovable)
            {
                removeCoroutine = StartCoroutine(SelfRemove());
            }
        }

        private IEnumerator SelfRemove()
        {
            yield return new WaitForSeconds(RemoveAfterSeconds);
            Destroy(this);
        }
    }
}

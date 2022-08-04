using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Autohand.Custom
{
    public class CubeBreak : MonoBehaviour
    {
        private const int DropObjectsCount = 8;
        private const float RandomFactorForTorque = 3f;

        public float Force = 10f;

        [SerializeField] private bool withRelativeForce = false;
        [SerializeField] private List<GameObject> commonCrystals;

        private Vector3[] offsets = { new Vector3(0.25f, 0.25f, 0.25f), new Vector3(-0.25f, 0.25f, 0.25f), new Vector3(0.25f, 0.25f, -0.25f), new Vector3(-0.25f, 0.25f, -0.25f),
                            new Vector3(0.25f, -0.25f, 0.25f), new Vector3(-0.25f, -0.25f, 0.25f), new Vector3(0.25f, -0.25f, -0.25f), new Vector3(-0.25f, -0.25f, -0.25f),};
        private Rigidbody rb;
        private Smash smash;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            smash = GetComponent<Smash>();
            smash.OnSmash.AddListener(Break);
        }

        [ContextMenu("Break")]
        public void Break(Smasher smasher, Collision collision)
        {
            for (var i = 0; i < DropObjectsCount; ++i)
            {
                var attractableObject = Instantiate(GetRandomFromList(commonCrystals), transform.position, transform.rotation);
                try
                {
                    attractableObject.transform.parent = transform;
                }
                catch { }
                attractableObject.transform.localPosition += offsets[i];
                attractableObject.transform.parent = null;
                var body = attractableObject.GetComponent<Rigidbody>();
                body.velocity = rb.velocity;
                if (withRelativeForce)
                {
                    body.AddRelativeForce(transform.rotation * (offsets[i] * Force), ForceMode.Impulse);
                    body.AddRelativeTorque(transform.rotation * (offsets[i] * Force + Vector3.one * (Random.value / RandomFactorForTorque)), ForceMode.Impulse);
                }
            }
            Destroy(gameObject);
        }

        private GameObject GetRandomFromList(List<GameObject> gameObjects)
        {
            return gameObjects[Random.Range(0, gameObjects.Count)];
        }

    }
}

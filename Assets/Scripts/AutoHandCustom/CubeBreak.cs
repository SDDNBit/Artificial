using Autohand.Demo;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBit.Autohand.Custom
{
    public class CubeBreak : MonoBehaviour
    {

        private const int DropObjectsCount = 8;

        public float force = 10f;
        [SerializeField] private bool UseSmasherVelocity = true;
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
        public void Break(Smasher smasher)
        {
            print("Smasher: " + smasher.gameObject.name);
            print(smasher.GetComponent<Rigidbody>().velocity);
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
                if (UseSmasherVelocity)
                {
                    body.AddRelativeForce(transform.rotation * (offsets[i] * force), ForceMode.Impulse);
                }
                else
                {
                    body.AddRelativeForce(transform.rotation * (offsets[i] * force), ForceMode.Impulse);
                }
                body.AddRelativeTorque(transform.rotation * (offsets[i] * force + Vector3.one * (Random.value / 3f)), ForceMode.Impulse);
            }
            Destroy(gameObject);
        }

        private GameObject GetRandomFromList(List<GameObject> gameObjects)
        {
            return gameObjects[Random.Range(0, gameObjects.Count)];
        }

    }
}

using Autohand;
using Autohand.Demo;
using UnityEngine;

namespace SoftBit.Autohand.Custom{
    public class CubeBreak : MonoBehaviour{
        private const int DropObjectsCount = 8;
        public float force = 10f;
        [SerializeField] private GameObject attractableObjectPrefab;
        
        private Vector3[] offsets = { new Vector3(0.25f, 0.25f, 0.25f), new Vector3(-0.25f, 0.25f, 0.25f), new Vector3(0.25f, 0.25f, -0.25f), new Vector3(-0.25f, 0.25f, -0.25f),
                            new Vector3(0.25f, -0.25f, 0.25f), new Vector3(-0.25f, -0.25f, 0.25f), new Vector3(0.25f, -0.25f, -0.25f), new Vector3(-0.25f, -0.25f, -0.25f),};
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        [ContextMenu("Break")]
        public void Break() {
            for(var i = 0; i < DropObjectsCount; ++i) {
                var attractableObject = Instantiate(attractableObjectPrefab, transform.position, transform.rotation);
                try{
                    attractableObject.transform.parent = transform;
                }
                catch { }
                attractableObject.transform.localPosition += offsets[i];
                attractableObject.transform.parent = null;
                var body = attractableObject.GetComponent<Rigidbody>();
                body.velocity = rb.velocity;
                body.AddRelativeForce(transform.rotation*(offsets[i]*force), ForceMode.Impulse);
                body.AddRelativeTorque(transform.rotation*(offsets[i]*force + Vector3.one*(Random.value/3f)), ForceMode.Impulse);
            }
            Destroy(gameObject);
        }

    }
}

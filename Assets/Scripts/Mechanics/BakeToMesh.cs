using UnityEngine;

namespace SoftBit.Mechanics
{
    public class BakeToMesh : MonoBehaviour
    {
        [ContextMenu("BakeMe")]
        public GameObject BakeMesh()
        {
            var mesh = new Mesh();
            var skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.BakeMesh(mesh);
            var bakedMeshGameObject = new GameObject(gameObject.name);
            var meshFilter = bakedMeshGameObject.AddComponent<MeshFilter>();
            var meshRenderer = bakedMeshGameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = skinnedMeshRenderer.material;
            meshFilter.mesh = mesh;
            bakedMeshGameObject.transform.position = transform.position;
            bakedMeshGameObject.transform.rotation = transform.rotation;
            var meshCollider = bakedMeshGameObject.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            bakedMeshGameObject.AddComponent<Rigidbody>();
            return bakedMeshGameObject;
        }
    }
}

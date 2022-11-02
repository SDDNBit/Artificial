using Autohand;
using SoftBit.Utils;
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

            var meshParent = new GameObject(gameObject.name + "Parent");
            meshParent.transform.position = meshRenderer.bounds.center;
            meshParent.transform.rotation = Quaternion.identity;
            bakedMeshGameObject.transform.SetParent(meshParent.transform);

            meshParent.AddComponent<Rigidbody>();
            var grabbable = meshParent.AddComponent<Grabbable>();
            grabbable.singleHandOnly = true;
            grabbable.jointBreakForce = float.PositiveInfinity;
            meshParent.AddComponent<FlyToObject>();
            meshParent.AddComponent<DestroyIfNotInUse>();
            meshParent.AddComponent<AttractableObject>();
            return meshParent;
        }
    }
}

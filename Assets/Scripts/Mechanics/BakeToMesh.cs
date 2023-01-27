using Autohand;
using SoftBit.Utils;
using UnityEngine;
using Constants = SoftBit.Utils.Constants;

namespace SoftBit.Mechanics
{
    public class BakeToMesh : MonoBehaviour
    {
        private const CollisionDetectionMode CollisionDetectionModeForPart = CollisionDetectionMode.Continuous;

        public void BakeMesh(float explosionForce, Vector3 explosionPosition)
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

            var meshParent = CorrectPivotWithParent(meshRenderer, bakedMeshGameObject);

            var parentRigidbody = AddAttractableBehaviour(meshParent);

            parentRigidbody.AddExplosionForce(explosionForce, explosionPosition, Constants.ExplosionRadius);
        }

        private GameObject CorrectPivotWithParent(MeshRenderer meshRenderer, GameObject bakedMeshGameObject)
        {
            var meshParent = new GameObject(gameObject.name + "Parent");
            meshParent.transform.position = meshRenderer.bounds.center;
            meshParent.transform.rotation = Quaternion.identity;
            bakedMeshGameObject.transform.SetParent(meshParent.transform);
            return meshParent;
        }

        private Rigidbody AddAttractableBehaviour(GameObject meshParent)
        {
            var parentRigidbody = meshParent.AddComponent<Rigidbody>();
            parentRigidbody.collisionDetectionMode = CollisionDetectionModeForPart;
            var grabbable = meshParent.AddComponent<Grabbable>();
            grabbable.singleHandOnly = true;
            grabbable.jointBreakForce = float.PositiveInfinity;
            meshParent.AddComponent<FlyToObject>();
            meshParent.AddComponent<DestroyIfNotInUse>();
            var attractableComponent = meshParent.AddComponent<AttractableObject>();
            attractableComponent.AddSmasherOnShoot = true;
            meshParent.AddComponent<HighlightBehaviour>();
            return parentRigidbody;
        }
    }
}

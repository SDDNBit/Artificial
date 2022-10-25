using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeToMesh : MonoBehaviour
{
    [ContextMenu("BakeMe")]
    private void BakeMe()
    {
        var mesh = new Mesh();
        GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
        var go = Instantiate(new GameObject("BakedMesh"));
        var meshFilter = go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
    }
}

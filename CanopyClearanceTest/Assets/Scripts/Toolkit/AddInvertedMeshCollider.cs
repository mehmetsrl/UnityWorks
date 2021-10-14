using System.Linq;
using UnityEngine;

public class AddInvertedMeshCollider : MonoBehaviour
{

    public bool removeExistingColliders = true;

    public void CreateInvertedMeshCollider()
    {
        if (removeExistingColliders)
            RemoveExistingColliders();
        InvertMesh();
        gameObject.AddComponent<MeshCollider>();

    }

    private void RemoveExistingColliders()
    {
        Collider[] cols = GetComponents<Collider>();
        for (int i = 0; i < cols.Length; i++)
        {
            DestroyImmediate(cols[i]);
        }
    }

    private void InvertMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();

    }

}

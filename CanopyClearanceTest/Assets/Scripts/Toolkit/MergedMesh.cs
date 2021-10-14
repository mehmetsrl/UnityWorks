using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergedMesh : MonoBehaviour
{
    public MeshFilter mergedMesh;
    public bool CreateCollider = false;
    private MeshCollider col;

    private Vector3 prevPos;
    private Quaternion prevRot;
    private Vector3 prevScale;

    void Start()
    {
        mergedMesh = CombineMesh(gameObject);
        if (CreateCollider)
        {
            col = gameObject.AddComponent<MeshCollider>();
            col.convex = true;
            col.inflateMesh = true;
            col.sharedMesh = mergedMesh.sharedMesh;
        }
        
    }

    MeshFilter CombineMesh(GameObject obj)
    {
        Matrix4x4 transformMatrix = obj.transform.worldToLocalMatrix;

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = transformMatrix * meshFilters[i].transform.localToWorldMatrix;
        }

        MeshFilter objMeshFilter = obj.GetComponent<MeshFilter>();

        if (objMeshFilter == null)
        {
            objMeshFilter = obj.AddComponent<MeshFilter>();
        }

        objMeshFilter.mesh = new Mesh();
        objMeshFilter.mesh.CombineMeshes(combine);

        return objMeshFilter;
    }

}

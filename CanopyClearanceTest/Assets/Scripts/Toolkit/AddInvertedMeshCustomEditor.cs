using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AddInvertedMeshCollider))]
public class AddInvertedMeshCustomEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AddInvertedMeshCollider script = (AddInvertedMeshCollider) target;
        if (GUILayout.Button("Create Inverted Mesh Collider"))
        {
            script.CreateInvertedMeshCollider();
        }
    }
}

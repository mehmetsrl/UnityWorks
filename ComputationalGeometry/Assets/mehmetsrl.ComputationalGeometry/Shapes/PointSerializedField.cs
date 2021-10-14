using mehmetsrl.ComputationalGeometry.Structures;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(Point))]
//public class PointSerializedField : Editor
//{

//    private SerializedProperty position;

//    private void OnEnable()
//    {
//        this.position = this.serializedObject.FindProperty("position");
//    }

//    public override void OnInspectorGUI()
//    {
//        this.serializedObject.Update();

//        SerializedProperty sp = position.Copy(); // copy so we don't iterate the original

//        if (sp.isArray)
//        {
//            int arrayLength = 0;

//            sp.Next(true); // skip generic field
//            sp.Next(true); // advance to array size field

//            // Get the array size
//            arrayLength = sp.intValue;

//            sp.Next(true); // advance to first array index

//            // Write values to list
//            List<int> values = new List<int>(arrayLength);
//            int lastIndex = arrayLength - 1;
//            for (int i = 0; i < arrayLength; i++)
//            {
//                values.Add(sp.intValue); // copy the value to the list
//                if (i < lastIndex) sp.Next(false); // advance without drilling into children
//            }

//            // iterate over the list displaying the contents
//            for (int i = 0; i < values.Count; i++)
//            {
//                EditorGUILayout.LabelField(i + " = " + values[i]);
//            }
//            EditorGUILayout.LabelField("asdasd");
//        }

//        this.DrawDefaultInspector(); // show the default inspector so we can set some values
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MeshCombine))]
public class MeshCombineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MeshCombine mc = target as MeshCombine;

        if(GUILayout.Button("Combine Meshes Basic"))
        {
            mc.CombineMeshesBas();
        }

        if (GUILayout.Button("Combine Meshes Advance"))
        {
            mc.CombineMeshesAdv();
        }
    }
}

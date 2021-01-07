using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProGen))]
public class ProGenEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        
        ProGen progen = (ProGen)target;

        if (GUILayout.Button("Generate"))
            progen.Generate();

        if (GUI.changed)
            progen.Generate();
    }
}
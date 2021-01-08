using UnityEngine;
using UnityEditor;

namespace Baks.Core.GenEditor
{
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
}
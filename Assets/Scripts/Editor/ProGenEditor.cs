using UnityEngine;
using UnityEditor;
using Action = System.Action;
using Baks.Core;

[CustomEditor(typeof(ProGen))]
public class ProGenEditor : Editor 
{
    private ProGen _proGen;
    private Editor _themeEditor;

    private void OnEnable() => _proGen = (ProGen)target;

    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
            
        if (GUILayout.Button("Generate"))
            _proGen.Generate();
            
        DrawSettingsOnEditor(_proGen.ProGenThemeSO, _proGen.Generate, true, ref _themeEditor);
    }

    private void DrawSettingsOnEditor(Object settings, Action onSettingsUpdated, bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    if (onSettingsUpdated != null)
                        onSettingsUpdated();
                }
            }
        }
    }
}

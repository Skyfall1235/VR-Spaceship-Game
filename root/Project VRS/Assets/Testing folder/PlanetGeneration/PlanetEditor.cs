using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor ShapeEditor;
    Editor ColorEditor;
    private void OnEnable()
    {
        planet = (Planet)target;
    }

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.ShapeSettingsFoldout, ref ShapeEditor);
        DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.ColorSettingsFoldout, ref ColorEditor);
    }

    //using a cached editor if available, fold out and display the settings editor
    void DrawSettingsEditor(Object Settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (Settings != null)
        {
            //create the foldout
            foldout = EditorGUILayout.InspectorTitlebar(foldout, Settings);

            //using the editor we checked, determine if we draw the new editor info
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    //if an editor doesnt exist, create one
                    CreateCachedEditor(Settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }
}
#endif

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FOVSensor))]
public class AISensorInspector : Editor
{
    SerializedProperty m_shouldDrawGizmos;
    SerializedProperty m_sightDistance;
    SerializedProperty m_horizontalSightAngle;
    SerializedProperty m_verticalSightAngle;
    SerializedProperty m_xSize;
    SerializedProperty m_ySize;
    private void OnEnable()
    {
        m_shouldDrawGizmos = serializedObject.FindProperty(nameof(m_shouldDrawGizmos));
        m_sightDistance = serializedObject.FindProperty(nameof(m_sightDistance));
        m_horizontalSightAngle = serializedObject.FindProperty(nameof(m_horizontalSightAngle));
        m_verticalSightAngle = serializedObject.FindProperty(nameof(m_verticalSightAngle));
        m_xSize = serializedObject.FindProperty(nameof(m_xSize));
        m_ySize = serializedObject.FindProperty(nameof(m_ySize));
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        FOVSensor scriptToUpdate = target as FOVSensor;
        EditorGUI.BeginChangeCheck();
        bool newshouldDrawGizmos = EditorGUILayout.Toggle("Should Draw Gizmos?", m_shouldDrawGizmos.boolValue);
        float newSightDistance = EditorGUILayout.FloatField("Sight Distance", m_sightDistance.floatValue);
        float newHorizontalSightAngle = EditorGUILayout.Slider("Horizontal Angle", m_horizontalSightAngle.floatValue, 0.01f, 180);
        float newVerticalSightAngle = EditorGUILayout.Slider("Horizontal Angle", m_verticalSightAngle.floatValue, 0.01f, 180);
        int newXSize = EditorGUILayout.IntField("X Vertex Count", m_xSize.intValue);
        int newYSize = EditorGUILayout.IntField("Y Vertex Count", m_ySize.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            m_shouldDrawGizmos.boolValue = newshouldDrawGizmos;
            m_sightDistance.floatValue = Mathf.Clamp(newSightDistance, 0.01f, float.MaxValue);
            m_horizontalSightAngle.floatValue = newHorizontalSightAngle;
            m_verticalSightAngle.floatValue = newVerticalSightAngle;
            m_xSize.intValue = Mathf.Clamp(newXSize, 1, int.MaxValue);
            m_ySize.intValue = Mathf.Clamp(newYSize, 1, int.MaxValue);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

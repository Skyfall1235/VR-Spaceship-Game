using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Project VRS/New Projectile Data Recursive")]
[System.Serializable]
public class SO_ProjectileRecursive : ScriptableObject, ISerializationCallbackReceiver
{
    [System.Serializable]
    public class ProjectileData
    {
        [SerializeField] public float Speed = 0;
        [field: SerializeField] public uint Damage  = 0;
        [field: SerializeField] public uint ArmorPenetration = 0;
        [field: SerializeField] public float Lifetime = 0;
        [field: SerializeField] public Vector2 Spread = new Vector2(0,0);
        [field: SerializeField] public GameObject Prefab = null;
        [field: SerializeField][HideInInspector] public bool SpawnSubProjectiles = false;
        [field: SerializeField][HideInInspector] public uint SubProjectileCount = 0;
        [field: SerializeField][HideInInspector] public float SubProjectileSpawnTime = 0;
        [HideInInspector][SerializeReference] public ProjectileData SubProjectileData = null;
    }
    ProjectileData m_rootProjectileData = new ProjectileData();
    [SerializeReference][HideInInspector] List<ProjectileData> m_projectileDataStructure = new List<ProjectileData>();

    public void OnBeforeSerialize()
    {
        m_projectileDataStructure.Clear();
        ProjectileData currentProjectileData = m_rootProjectileData;
        while (currentProjectileData != null)
        {
            m_projectileDataStructure.Add(currentProjectileData);
            currentProjectileData = currentProjectileData.SubProjectileData;
        }
    }

    public void OnAfterDeserialize()
    {
        if(m_projectileDataStructure.Count > 0)
        {
            m_rootProjectileData = m_projectileDataStructure[0];
            ProjectileData currentData = m_rootProjectileData;
            for (int i = 0; i < m_projectileDataStructure.Count; ++i)
            {
                if(i + 1 < m_projectileDataStructure.Count)
                {
                    currentData.SubProjectileData = m_projectileDataStructure[i + 1];
                    currentData = currentData.SubProjectileData;
                }
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SO_ProjectileRecursive))]
public class SO_ProjectileRecursiveEditor : Editor
{
    SerializedProperty m_projectileDataStructure;

    private void OnEnable()
    {
        m_projectileDataStructure = serializedObject.FindProperty(nameof(m_projectileDataStructure));
    }
    void ShowFieldsForData(SerializedProperty dataToShow, int arrayPosition, int arrayCount)
    {
        if(arrayPosition <= 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
        }
        SerializedProperty Speed = dataToShow.FindPropertyRelative(nameof(Speed));
        Speed.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Speed", Speed.floatValue), 0, float.MaxValue);
        SerializedProperty Damage = dataToShow.FindPropertyRelative(nameof(Damage));
        Damage.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Damage", (int)Damage.uintValue), 0, uint.MaxValue);
        SerializedProperty ArmorPenetration = dataToShow.FindPropertyRelative(nameof(ArmorPenetration));
        ArmorPenetration.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Armor Penetration", (int)ArmorPenetration.uintValue), 0, uint.MaxValue);
        SerializedProperty Lifetime = dataToShow.FindPropertyRelative(nameof(Lifetime));
        Lifetime.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Lifetime", Lifetime.floatValue), 0, float.MaxValue);
        SerializedProperty Spread = dataToShow.FindPropertyRelative(nameof(Spread));
        Vector2 incomingSpread = EditorGUILayout.Vector2Field("Spread", Spread.vector2Value);
        Spread.vector2Value = new Vector2(Mathf.Clamp(incomingSpread.x, 0, float.MaxValue), Mathf.Clamp(incomingSpread.y, 0, float.MaxValue));
        SerializedProperty Prefab = dataToShow.FindPropertyRelative(nameof(Prefab));
        Prefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", Prefab.objectReferenceValue, typeof(GameObject), false) as GameObject;
        SerializedProperty SpawnSubProjectiles = dataToShow.FindPropertyRelative(nameof(SpawnSubProjectiles));
        SpawnSubProjectiles.boolValue = EditorGUILayout.Toggle("Spawn Sub-Projectiles?", SpawnSubProjectiles.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        if (SpawnSubProjectiles.boolValue)
        {
            if(arrayPosition + 1 >= arrayCount || m_projectileDataStructure.GetArrayElementAtIndex(arrayPosition + 1) == null)
            {
                if(arrayPosition +1 >= arrayCount)
                {
                    m_projectileDataStructure.arraySize = arrayCount + 1;
                }
                m_projectileDataStructure.GetArrayElementAtIndex(arrayPosition + 1).managedReferenceValue = new SO_ProjectileRecursive.ProjectileData();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(20f * (arrayPosition + 1), false);
            EditorGUILayout.BeginVertical();
            SerializedProperty SubProjectileCount = dataToShow.FindPropertyRelative(nameof (SubProjectileCount));
            SubProjectileCount.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Sub-Projectile Count", (int)SubProjectileCount.uintValue), 1, uint.MaxValue);
            SerializedProperty SubProjectileSpawnTime = dataToShow.FindPropertyRelative(nameof(SubProjectileSpawnTime));
            SubProjectileSpawnTime.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Sub-Projectile Spawn Time", SubProjectileSpawnTime.floatValue), 0, SubProjectileSpawnTime.floatValue);
            
        }
        else
        {
            if( arrayPosition + 1 < arrayCount && m_projectileDataStructure.GetArrayElementAtIndex(arrayPosition + 1).managedReferenceValue != null)
            {
                m_projectileDataStructure.GetArrayElementAtIndex(arrayPosition + 1).managedReferenceValue = null;

                m_projectileDataStructure.arraySize = arrayPosition;
            }

        }
    }
    public override void OnInspectorGUI()
    {
        if(m_projectileDataStructure.arraySize > 0)
        {
            serializedObject.Update();
            for (int i = 0; i < m_projectileDataStructure.arraySize; ++i)
            {
                SerializedProperty currentProjectileDataToSerialize = m_projectileDataStructure.GetArrayElementAtIndex(i);
                ShowFieldsForData(currentProjectileDataToSerialize, i, m_projectileDataStructure.arraySize);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
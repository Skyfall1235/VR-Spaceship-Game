using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Project VRS/New Projectile Data")]
[System.Serializable]
public class SO_ProjectileData : ScriptableObject, ISerializationCallbackReceiver
{
    

   [HideInInspector][SerializeReference] ProjectileData m_subProjectileData = null;
   public ProjectileData SubProjectileData { get {  return m_subProjectileData; } internal set { m_subProjectileData = value; } }
    

    ProjectileData m_rootProjectileData = new ProjectileData();
    public ProjectileData RootProjectileData { get {  return m_rootProjectileData; } }
    [SerializeReference] List<ProjectileData> m_projectileDataStructure = new List<ProjectileData>();

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

    public uint CalculateDepth()
    {
        uint depth = 0;
        for(ProjectileData currentData = RootProjectileData; currentData.SubProjectileData != null; currentData = currentData.SubProjectileData)
        {
            depth++;
        }
        return depth;
    }

    public ProjectileData GetDataAtDepth(uint desiredDepth)
    {
        ProjectileData currentData = RootProjectileData;
        for (uint remainingDepth = desiredDepth; remainingDepth >= 0; remainingDepth--)
        {
            if (currentData != null)
            {
                if (remainingDepth <= 0)
                {
                    return currentData;
                }
                else
                {
                    currentData = currentData.SubProjectileData;
                }
            }
            else
            {
                return null;
            }
        }
        throw new System.IndexOutOfRangeException();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SO_ProjectileData))]
public class SO_ProjectileRecursiveEditor : Editor
{
    SerializedProperty m_projectileDataStructure;

    private void OnEnable()
    {
        m_projectileDataStructure = serializedObject.FindProperty(nameof(m_projectileDataStructure));
    }
    void ShowFieldsForData(SerializedProperty dataToShow, int arrayPosition, int arrayCount)
    {
        if(dataToShow.managedReferenceValue == null)
        {
            return;
        }
        if(arrayPosition <= 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
        }
        SerializedProperty m_speed = dataToShow.FindPropertyRelative(nameof(m_speed));
        m_speed.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Speed", m_speed.floatValue), 0, float.MaxValue);
        SerializedProperty m_damage = dataToShow.FindPropertyRelative(nameof(m_damage));
        m_damage.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Damage", (int)m_damage.uintValue), 0, uint.MaxValue);
        SerializedProperty m_armorPenetration = dataToShow.FindPropertyRelative(nameof(m_armorPenetration));
        m_armorPenetration.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Armor Penetration", (int)m_armorPenetration.uintValue), 0, uint.MaxValue);
        SerializedProperty m_lifetime = dataToShow.FindPropertyRelative(nameof(m_lifetime));
        m_lifetime.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Lifetime", m_lifetime.floatValue), 0, float.MaxValue);
        SerializedProperty m_projectileCount = dataToShow.FindPropertyRelative(nameof(m_projectileCount));
        m_projectileCount.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Projectile Count", (int)m_projectileCount.uintValue), 1, uint.MaxValue);
        SerializedProperty m_spread = dataToShow.FindPropertyRelative(nameof(m_spread));
        Vector2 incomingSpread = EditorGUILayout.Vector2Field("Spread", m_spread.vector2Value);
        m_spread.vector2Value = new Vector2(Mathf.Clamp(incomingSpread.x, 0, float.MaxValue), Mathf.Clamp(incomingSpread.y, 0, float.MaxValue));
        SerializedProperty m_prefab = dataToShow.FindPropertyRelative(nameof(m_prefab));
        m_prefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", m_prefab.objectReferenceValue, typeof(GameObject), false) as GameObject;
        SerializedProperty m_spawnSubProjectiles = dataToShow.FindPropertyRelative(nameof(m_spawnSubProjectiles));
        m_spawnSubProjectiles.boolValue = EditorGUILayout.Toggle("Spawn Sub-Projectiles?", m_spawnSubProjectiles.boolValue);
        if(arrayPosition < arrayCount)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        if (m_spawnSubProjectiles.boolValue)
        {
            if(arrayPosition + 1 >= arrayCount)
            {
                if(arrayPosition +1 >= arrayCount)
                {
                    m_projectileDataStructure.arraySize = arrayCount + 1;
                }
                m_projectileDataStructure.GetArrayElementAtIndex(arrayPosition + 1).managedReferenceValue = new ProjectileData();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(20f * (arrayPosition + 1), false);
            EditorGUILayout.BeginVertical();
            SerializedProperty m_subProjectileSpawnTime = dataToShow.FindPropertyRelative(nameof(m_subProjectileSpawnTime));
            m_subProjectileSpawnTime.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Sub-Projectile Spawn Time", m_subProjectileSpawnTime.floatValue), 0, m_lifetime.floatValue);
            
        }
        else
        {
            if(arrayPosition < arrayCount - 1)
            {
                m_projectileDataStructure.GetArrayElementAtIndex(arrayPosition + 1).managedReferenceValue = null;
                m_projectileDataStructure.DeleteArrayElementAtIndex(arrayPosition + 1);
                m_projectileDataStructure.arraySize = arrayCount;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(20f * (arrayPosition + 1), false);
                EditorGUILayout.BeginVertical();
            }

        }
    }
    public override void OnInspectorGUI()
    {
        if(m_projectileDataStructure.arraySize > 0)
        {
            serializedObject.Update();
            int currentArraySize = m_projectileDataStructure.arraySize;
            for (int i = 0; i < currentArraySize;  ++i)
            {
                SerializedProperty currentProjectileDataToSerialize = m_projectileDataStructure.GetArrayElementAtIndex(i);
                if(currentProjectileDataToSerialize.managedReferenceValue as ProjectileData != null)
                {
                    ShowFieldsForData(currentProjectileDataToSerialize, i, m_projectileDataStructure.arraySize);
                }
                currentArraySize = m_projectileDataStructure.arraySize;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
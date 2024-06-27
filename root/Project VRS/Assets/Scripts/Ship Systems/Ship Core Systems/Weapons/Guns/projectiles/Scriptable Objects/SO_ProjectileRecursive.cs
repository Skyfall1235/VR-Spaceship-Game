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

        [SerializeField] float m_speed = 0;
        public float Speed { get { return m_speed; } }

        [SerializeField] uint m_damage  = 0;
        public float Damage {  get { return m_damage; } }

        [SerializeField] public uint m_armorPenetration = 0;
        public uint ArmorPenetration { get {  return m_armorPenetration; } }

        [SerializeField] float m_lifetime = 0;
        public float Lifetime { get { return m_lifetime; } }

        [SerializeField] Vector2 m_spread = new Vector2(0,0);
        public Vector2 Spread { get { return m_spread; } }

        [SerializeField] GameObject m_prefab = null;
        public GameObject Prefab { get { return m_prefab; } }

        [SerializeField][HideInInspector] bool m_spawnSubProjectiles = false;
        public bool SpawnSubProjectiles { get { return m_spawnSubProjectiles; } }

        [SerializeField][HideInInspector] uint m_subProjectileCount = 0;
        public uint? SubProjectileCount
        {
            get
            {
                if (m_spawnSubProjectiles)
                {
                    return m_subProjectileCount;
                }
                return null;
            }
        }

        [SerializeField][HideInInspector] float m_subProjectileSpawnTime = 0;
        public float? SubProjectileSpawnTime
        {
            get 
            {
                if (m_spawnSubProjectiles)
                {
                    return m_subProjectileSpawnTime;
                }
                return null;
            }
        }

        [HideInInspector][SerializeReference] ProjectileData m_subProjectileData = null;
        public ProjectileData SubProjectileData { get {  return m_subProjectileData; } internal set { m_subProjectileData = value; } }
    }
    ProjectileData m_rootProjectileData = new ProjectileData();
    public ProjectileData RootProjectileData { get {  return m_rootProjectileData; } }
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
        SerializedProperty m_speed = dataToShow.FindPropertyRelative(nameof(m_speed));
        m_speed.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Speed", m_speed.floatValue), 0, float.MaxValue);
        SerializedProperty m_damage = dataToShow.FindPropertyRelative(nameof(m_damage));
        m_damage.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Damage", (int)m_damage.uintValue), 0, uint.MaxValue);
        SerializedProperty m_armorPenetration = dataToShow.FindPropertyRelative(nameof(m_armorPenetration));
        m_armorPenetration.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Armor Penetration", (int)m_armorPenetration.uintValue), 0, uint.MaxValue);
        SerializedProperty m_lifetime = dataToShow.FindPropertyRelative(nameof(m_lifetime));
        m_lifetime.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Lifetime", m_lifetime.floatValue), 0, float.MaxValue);
        SerializedProperty m_spread = dataToShow.FindPropertyRelative(nameof(m_spread));
        Vector2 incomingSpread = EditorGUILayout.Vector2Field("Spread", m_spread.vector2Value);
        m_spread.vector2Value = new Vector2(Mathf.Clamp(incomingSpread.x, 0, float.MaxValue), Mathf.Clamp(incomingSpread.y, 0, float.MaxValue));
        SerializedProperty m_prefab = dataToShow.FindPropertyRelative(nameof(m_prefab));
        m_prefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", m_prefab.objectReferenceValue, typeof(GameObject), false) as GameObject;
        SerializedProperty m_spawnSubProjectiles = dataToShow.FindPropertyRelative(nameof(m_spawnSubProjectiles));
        m_spawnSubProjectiles.boolValue = EditorGUILayout.Toggle("Spawn Sub-Projectiles?", m_spawnSubProjectiles.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        if (m_spawnSubProjectiles.boolValue)
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
            SerializedProperty m_subProjectileCount = dataToShow.FindPropertyRelative(nameof (m_subProjectileCount));
            m_subProjectileCount.uintValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Sub-Projectile Count", (int)m_subProjectileCount.uintValue), 1, uint.MaxValue);
            SerializedProperty m_subProjectileSpawnTime = dataToShow.FindPropertyRelative(nameof(m_subProjectileSpawnTime));
            m_subProjectileSpawnTime.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Sub-Projectile Spawn Time", m_subProjectileSpawnTime.floatValue), 0, m_lifetime.floatValue);
            
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
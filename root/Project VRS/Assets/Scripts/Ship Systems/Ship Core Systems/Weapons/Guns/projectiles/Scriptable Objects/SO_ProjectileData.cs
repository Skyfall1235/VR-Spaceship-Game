using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Project VRS/New Projectile Data")]
[Serializable]
public class SO_ProjectileData : ScriptableObject
{
    [SerializeField] float m_projectileSpeed;
    [SerializeField] float m_projectileDestroyTime;
    [SerializeField] uint m_projectileDamage;
    [HideInInspector][SerializeField] bool m_spawnSubProjectiles;
    [HideInInspector][SerializeField] uint m_subProjectileCount;
    public uint? SubProjectileCount
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileCount;
            }
            else
            {
                return null;
            }
        }
    }
    [HideInInspector][SerializeField] float m_subProjectileSpawnTime;
    public float? SubProjectileSpawnTime
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileSpawnTime;
            }
            else
            {
                return null;
            }
        }
    }
    [HideInInspector][SerializeField] float m_subProjectileSpeed;
    public float? SubProjectileSpeed
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileSpeed;
            }
            else
            {
                return null;
            }
        }
    }
    [HideInInspector][SerializeField] uint m_subProjectileDamage;
    public uint? SubProjectileDamage
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileDamage;
            }
            else
            {
                return null;
            }
        }
    }
    [HideInInspector][SerializeField] Vector2 m_subProjectileSpread;
    public Vector2? SubProjectileSpread
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileSpread;
            }
            else
            {
                return null;
            }
        }
    }
    [HideInInspector][SerializeField] float m_subProjectileDestroyTime;
    public float? SubProjectileDestroyTime
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileDestroyTime;
            }
            else
            {
                return null;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_ProjectileData))]
public class SO_ProjectileEditor : Editor
{
    SerializedProperty m_spawnSubProjectiles;
    SerializedProperty m_subProjectileCount;
    SerializedProperty m_subProjectileSpawnTime;
    SerializedProperty m_subProjectileSpeed;
    SerializedProperty m_subProjectileDamage;
    SerializedProperty m_subProjectileSpread;
    SerializedProperty m_subProjectileDestroyTime;
    SerializedProperty m_projectileDestroyTime;
    private void OnEnable()
    {
        m_spawnSubProjectiles = serializedObject.FindProperty(nameof(m_spawnSubProjectiles));
        m_subProjectileCount = serializedObject.FindProperty(nameof(m_subProjectileCount));
        m_subProjectileSpawnTime = serializedObject.FindProperty(nameof(m_subProjectileSpawnTime));
        m_subProjectileSpeed = serializedObject.FindProperty(nameof(m_subProjectileSpeed));
        m_subProjectileDamage = serializedObject.FindProperty(nameof(m_subProjectileDamage));
        m_subProjectileSpread = serializedObject.FindProperty(nameof(m_subProjectileSpread));
        m_subProjectileDestroyTime = serializedObject.FindProperty(nameof(m_subProjectileDestroyTime));
        m_projectileDestroyTime = serializedObject.FindProperty(nameof(m_projectileDestroyTime));
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        uint newSubProjectileCount = 0;
        float newSubProjectileSpawnTime = 0;
        float newSubProjectileSpeed = 0;
        float newSubProjectileDestroyTime = 0;
        uint newSubProjectileDamage = 0;
        Vector2 newSubProjectileSpread = Vector2.zero;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sub Projectiles");
        bool newSpawnProjectile = EditorGUILayout.Toggle(m_spawnSubProjectiles.boolValue);
        EditorGUILayout.EndHorizontal();
        if (m_spawnSubProjectiles.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(0.1f);
            EditorGUILayout.BeginVertical();
            newSubProjectileCount = (uint)Mathf.Clamp(EditorGUILayout.IntField("Sub Projectile Count", (int)m_subProjectileCount.uintValue), 0, uint.MaxValue);
            newSubProjectileSpawnTime = EditorGUILayout.FloatField("Sub Projectile Spawn Time", m_subProjectileSpawnTime.floatValue);
            newSubProjectileSpeed = EditorGUILayout.FloatField("Sub Projectile Speed", m_subProjectileSpeed.floatValue);
            newSubProjectileDamage = (uint)Mathf.Clamp(EditorGUILayout.IntField("Sub Projectile Damage", (int)m_subProjectileDamage.uintValue), 0, uint.MaxValue);
            newSubProjectileSpread = EditorGUILayout.Vector2Field("Spread", m_subProjectileSpread.vector2Value);
            newSubProjectileDestroyTime = EditorGUILayout.FloatField("Sub Projectile Destroy Time", m_subProjectileDestroyTime.floatValue);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed Scriptable Object");
            
            if (m_spawnSubProjectiles.boolValue)
            {
                m_subProjectileCount.uintValue = newSubProjectileCount;
                m_subProjectileSpawnTime.floatValue = Mathf.Clamp(newSubProjectileSpawnTime, 0, m_projectileDestroyTime.floatValue);
                m_subProjectileSpeed.floatValue = Mathf.Clamp(newSubProjectileSpeed, 0, Mathf.Infinity);
                m_subProjectileDamage.uintValue = newSubProjectileDamage;
                m_subProjectileSpread.vector2Value = newSubProjectileSpread;
                m_subProjectileDestroyTime.floatValue = Mathf.Clamp(newSubProjectileDestroyTime, 0, Mathf.Infinity);
            }
            m_spawnSubProjectiles.boolValue = newSpawnProjectile;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;

[CreateAssetMenu(menuName = "Project VRS/New Weapon Data")]
[Serializable]
public class SO_WeaponData : ScriptableObject
{
    #region Data Structures
    public enum ReloadMode
    {
        /// <summary>
        /// The weapon heat up when firing and cools off when not firing
        /// </summary>
        Heat,
        /// <summary>
        /// the weapon can fire a certain amount of times before having to reload
        /// </summary>
        Ammo,
        /// <summary>
        /// The weapon does not reload
        /// </summary>
        None
    }
    #endregion

    [SerializeField] string m_weaponName;
    public string WeaponName { get { return m_weaponName; } }

    [Header("Projectile and FX")]


    /// <summary>
    /// Whether the weapon uses projectiles
    /// </summary>
    [SerializeField][HideInInspector] bool m_useProjectile;
    public bool UseProjectile { get { return m_useProjectile; }}

    /// <summary>
    /// The data for any projectile fired
    /// </summary>
    [SerializeField][HideInInspector] SO_ProjectileData m_projectileData;
    public SO_ProjectileData WeaponProjectileData {  get { return m_projectileData; }}

    /// <summary>
    /// Particle system assciated with this turret.
    /// </summary>
    [SerializeField]
    [Tooltip("Particle system used for visual effects when the turret fires.")]
    private ParticleSystem m_weaponParticleSystem;
    public ParticleSystem WeaponParticleSystem
    {
        get => m_weaponParticleSystem;
    }

    /// <summary>
    /// The required minimum hardpoint size for this weapon.
    /// </summary>
    [SerializeField]
    [Tooltip("Specifies the minimum hardpoint size needed to house this weapon.")]
    public WeaponSize m_requiredHardpointSize;
    public WeaponSize RequiredHardpointSize
    {
        get => m_requiredHardpointSize;
    }

    /// <summary>
    /// Array of Audio clips for the SFX of the turret to use when firing.
    /// </summary>
    [SerializeField]
    [Tooltip("Array of audio clips played when the turret fires.")]
    private AudioClip[] m_weaponFireAudioClip;
    public AudioClip[] WeaponFireAudioClip
    {
        get => m_weaponFireAudioClip;
    }

    /// <summary>
    /// Array of audio clips for the weapon to make when doing actions.
    /// </summary>
    [SerializeField]
    [Tooltip("Array of audio clips played for various turret events.")]
    private AudioClip[] m_weaponEventSFX;
    public AudioClip[] WeaponEventSFX
    {
        get => m_weaponEventSFX;
    }

    /// <summary>
    /// The size of hardpoint required for the turret
    /// </summary>
    public enum WeaponSize
    {
        Small = 1,
        Medium = 2,
        Large = 3,
        XLarge = 4,
        XXLarge = 5
    }

    /// <summary>
    /// The minimum time between firing of the weapon
    /// </summary>
    [SerializeField][HideInInspector]
    [Tooltip("The minimum time between firing of the weapon")]
    private float m_minimumTimeBetweenFiring;
    public float MinimumTimeBetweenFiring
    {
        get => m_minimumTimeBetweenFiring;
    }

    /// <summary>
    /// Whether the weapon uses a magazine or not
    /// </summary>
    [Tooltip("Whether the weapon uses a magazine or not")]
    [HideInInspector] [SerializeField]
    bool m_usesMag = true;
    public bool UsesMag
    {
        get => m_usesMag;
    }

    /// <summary>
    /// The amount of time it takes to reload a weapon
    /// </summary>
    [Tooltip("The amount of time it takes to reload a weapon")]
    [SerializeField][HideInInspector]
    float m_reloadTime = 0;
    public float? ReloadTime
    {
        get
        {
            if (UsesMag)
            {
                return m_reloadTime;
            }
            else
            {
                return null;
            }
        }
        private set
        {
            if(UsesMag && value != null)
            {
                m_reloadTime = Mathf.Clamp((float)value, 0, float.MaxValue);
            }
        }
    }

    /// <summary>
    /// The total capacity of the magazine
    /// </summary>
    [Tooltip("The amount of time it takes to reload a weapon")]
    [SerializeField][HideInInspector]
    uint m_magazineCapacity = 0;
    public uint? MagazineCapacity
    {
        get
        {
            if (UsesMag)
            {
                return m_magazineCapacity;
            }
            else
            {
                return null;
            }
        }
        private set
        {
            if (UsesMag)
            {
                m_magazineCapacity = (uint)value;
            }
        }
    }

    [SerializeField] [SerializedTypeRestrictionAttribute(type = typeof(BC_FireType))] SerializedType m_fireType;
    public Type WeaponFiringMode
    {
        get => m_fireType.Value;
    }
    /// <summary>
    /// Whether the weapon is in charge of firing itself or takes commands from the wepon manager
    /// </summary>
    [SerializeField] bool m_autoFiring;
    public bool AutoFiring
    {
        get
        {
            return m_autoFiring;
        }
    }

    /// <summary>
    /// Whether the weapon should use the spread values
    /// </summary>
    [SerializeField][HideInInspector] bool m_useSpread;
    public bool UseSpread
    {
        get
        {
            return m_useSpread;
        }
    }
    [SerializeField][HideInInspector]Vector2 m_spreadValues;
    public Vector2? SpreadValues
    {
        get
        {
            if (UseSpread)
            {
                return (Vector2?)m_spreadValues;
            }
            else
            {
                return null;
            }
        }
    }


    private void OnValidate()
    {
        if(m_weaponName == "")
        {
            m_weaponName = name;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_WeaponData))]
class SO_WeaponData_Editor : Editor
{
    SerializedProperty m_useProjectile;
    SerializedProperty m_projectileData;
    SerializedProperty m_magazineCapacity;
    SerializedProperty m_reloadTime;
    SerializedProperty m_minimumTimeBetweenFiring;
    SerializedProperty m_usesMag;
    SerializedProperty m_useSpread;
    SerializedProperty m_spreadValues;
    private void OnEnable()
    {
        m_useProjectile = serializedObject.FindProperty(nameof(m_useProjectile));
        m_projectileData = serializedObject.FindProperty(nameof(m_projectileData));
        m_magazineCapacity = serializedObject.FindProperty(nameof(m_magazineCapacity));
        m_reloadTime = serializedObject.FindProperty(nameof(m_reloadTime));
        m_minimumTimeBetweenFiring = serializedObject.FindProperty(nameof(m_minimumTimeBetweenFiring));
        m_usesMag = serializedObject.FindProperty(nameof(m_usesMag));
        m_useSpread = serializedObject.FindProperty(nameof(m_useSpread));
        m_spreadValues = serializedObject.FindProperty(nameof(m_spreadValues));
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        float newMinimumTimeBetweenFiring = EditorGUILayout.FloatField("Minimum Time Between Shots", m_minimumTimeBetweenFiring.floatValue);

        //Initial variables to save values to
        SO_ProjectileData newProjectileData = null;
        uint newMagazineCapacityValue = 0;
        float newReloadTime = 0;
        Vector2 newSpreadValues = new Vector2(0, 0);

        //checkbox and variables for using a projectile
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Use Projectile");
            bool newUseProjectile = EditorGUILayout.Toggle(m_useProjectile.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(.1f);
            EditorGUILayout.BeginVertical();
            if (m_useProjectile.boolValue)
            {
                newProjectileData = (SO_ProjectileData)EditorGUILayout.ObjectField("Projectile Data", m_projectileData.objectReferenceValue, typeof(SO_ProjectileData), true);
            }
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //checkbox and variables for using a magazine
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Magazine");
            bool newUsesMag = EditorGUILayout.Toggle(m_usesMag.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(.1f);
            EditorGUILayout.BeginVertical();
                if (m_usesMag.boolValue)
                {
                    newReloadTime = EditorGUILayout.FloatField("Reload Time", m_reloadTime.floatValue);
                    newMagazineCapacityValue = (uint)Mathf.Clamp(EditorGUILayout.IntField("Magazine Capacity", m_magazineCapacity.intValue), 0, uint.MaxValue);
                }
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //checkbox and variables for spread
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spread");
            bool newUseSpred = EditorGUILayout.Toggle(m_useSpread.boolValue);
        EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(.1f);
            EditorGUILayout.BeginVertical();
                if (m_useSpread.boolValue)
                {
                    newSpreadValues = EditorGUILayout.Vector2Field("Spread Max Values", m_spreadValues.vector2Value);
                }
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //Update our object
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Scriptable Object");
            if(m_useProjectile.boolValue)
            {
                m_projectileData.objectReferenceValue = newProjectileData;
            }
            m_useProjectile.boolValue = newUseProjectile;
            m_minimumTimeBetweenFiring.floatValue = Mathf.Clamp(newMinimumTimeBetweenFiring, 0, float.MaxValue);
            if (m_usesMag.boolValue)
            {
                m_magazineCapacity.uintValue = newMagazineCapacityValue;
                m_reloadTime.floatValue = Mathf.Clamp(newReloadTime, 0, float.MaxValue);
            }
            m_usesMag.boolValue = newUsesMag;
            if(m_useSpread.boolValue)
            {
                m_spreadValues.vector2Value = newSpreadValues;
            }
            m_useSpread.boolValue = newUseSpred;
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif

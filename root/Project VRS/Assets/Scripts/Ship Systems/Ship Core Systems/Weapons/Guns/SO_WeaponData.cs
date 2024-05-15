using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "New Weapon Data")]
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

    public enum FiringMode
    {
        /// <summary>
        /// Fires repeatedly until the command is given to stop or the weapon needs to reload
        /// </summary>
        Auto,
        /// <summary>
        /// Fires once per command
        /// </summary>
        SemiAuto,
        /// <summary>
        /// holds a singular fire until told to stop
        /// </summary>
        Beam
    }
    #endregion

    [Header("Projectile and FX")]

    [SerializeField]
    [Tooltip("The speed of the projectiles fired by this turret. This value might be overridden based on the weapon reference.")]
    private float m_projectileSpeed;
    public float ProjectileSpeed
    {
        get => m_projectileSpeed;
    }

    /// <summary>
    /// gameobject reference to the bullet prefab.
    /// </summary>
    [SerializeField]
    [Tooltip("Prefab of the game object used as the projectile fired by this turret.")]
    private GameObject m_prefabBullet;
    public GameObject PrefabBullet
    {
        get => m_prefabBullet;
    }

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
    [SerializeField] FiringMode m_firingMode = FiringMode.SemiAuto;
    public FiringMode WeaponFiringMode
    {
        get => m_firingMode;
    }

    [SerializeField] bool m_autoFiring;
    public bool AutoFiring
    {
        get
        {
            return m_autoFiring;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_WeaponData))]
public class SO_WeaponData_Editor : Editor
{
    SerializedProperty m_magazineCapacity;
    SerializedProperty m_reloadTime;
    SerializedProperty m_minimumTimeBetweenFiring;
    SerializedProperty m_usesMag;
    private void OnEnable()
    {
        m_magazineCapacity = serializedObject.FindProperty(nameof(m_magazineCapacity));
        m_reloadTime = serializedObject.FindProperty(nameof(m_reloadTime));
        m_minimumTimeBetweenFiring = serializedObject.FindProperty(nameof(m_minimumTimeBetweenFiring));
        m_usesMag = serializedObject.FindProperty(nameof(m_usesMag));
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        SO_WeaponData scriptToDisplayDataFor = (SO_WeaponData)target;
        EditorGUI.BeginChangeCheck();
        int newMagazineCapacityValue = 0;
        float newReloadTime = 0;
        float newMinimumTimeBetweenFiring = EditorGUILayout.FloatField("Minimum Time Between Shots", m_minimumTimeBetweenFiring.floatValue);
        bool newUsesMag = EditorGUILayout.Toggle("Uses a Magazine", m_usesMag.boolValue);
        if (scriptToDisplayDataFor.UsesMag)
        {
            newReloadTime = EditorGUILayout.FloatField("Reload Time", m_reloadTime.floatValue);
            newMagazineCapacityValue = EditorGUILayout.IntField("Magazine Capacity", m_magazineCapacity.intValue);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Scriptable Object");
            m_usesMag.boolValue = newUsesMag;
            m_minimumTimeBetweenFiring.floatValue = Mathf.Clamp(newMinimumTimeBetweenFiring, 0 , float.MaxValue);
            m_magazineCapacity.intValue = (int)Mathf.Clamp(newMagazineCapacityValue, 0, uint.MaxValue);
            m_reloadTime.floatValue = Mathf.Clamp(newReloadTime, 0, float.MaxValue);
            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif

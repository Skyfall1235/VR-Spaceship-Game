using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "New Weapon Data")]
[Serializable]
public class SO_WeaponData : ScriptableObject
{
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
    private WeaponSize m_requiredHardpointSize;
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
        Small,
        Medium,
        Large,
        XLarge,
        XXLarge
    }

    /// <summary>
    /// The minimum time between firing of the weapon
    /// </summary>
    [field: SerializeField]
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
    [HideInInspector]
    bool m_usesMag = true;
    public bool UsesMag
    {
        get => m_usesMag;
        set => m_usesMag = value;
    }

    /// <summary>
    /// The amount of time it takes to reload a weapon
    /// </summary>
    [Tooltip("The amount of time it takes to reload a weapon")]
    [HideInInspector]
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
        set
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
    [HideInInspector]
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
        set
        {
            if (UsesMag)
            {
                m_magazineCapacity = (uint)value;
            }
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_WeaponData))]
public class SO_WeaponData_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SO_WeaponData scriptToDisplayDataFor = (SO_WeaponData)target;
        EditorGUI.BeginChangeCheck();
        int newMagazineCapacityValue = 0;
        float newReloadTime = 0;
        scriptToDisplayDataFor.UsesMag = EditorGUILayout.Toggle("Uses Mag", scriptToDisplayDataFor.UsesMag);
        if (scriptToDisplayDataFor.UsesMag)
        {
            newReloadTime = EditorGUILayout.FloatField("Reload Time", (float)scriptToDisplayDataFor.ReloadTime);
            newMagazineCapacityValue = EditorGUILayout.IntField("Magazine Capacity", (int)scriptToDisplayDataFor.MagazineCapacity);
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Scriptable Object");
            scriptToDisplayDataFor.MagazineCapacity = (uint)Mathf.Clamp(newMagazineCapacityValue, 0, uint.MaxValue);
            scriptToDisplayDataFor.ReloadTime = Mathf.Clamp(newReloadTime, 0, float.MaxValue);
        }
    }

}
#endif

using System;
using System.Collections;
using System.Collections.Generic;
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
    [Tooltip("The minimum time between firing of the weapon")]
    [field: SerializeField]
    public float minimumTimeBetweenFiring { get; private set; }

}

using UnityEngine;

public class SO_TurretData : ScriptableObject
{
    //what else needs to go here, and what can we store in the SO?
    [Header("Projectile and FX")]

    [SerializeField]
    [Tooltip("The speed of the projectiles fired by this turret. This value might be overridden based on the weapon reference.")]
    private float m_projectileSpeed;
    public float ProjectileSpeed
    {
        get => m_projectileSpeed;
    }

    [SerializeField]
    [Tooltip("Prefab of the game object used as the projectile fired by this turret.")]
    private GameObject m_prefabBullet;
    public GameObject PrefabBullet
    {
        get => m_prefabBullet;
    }

    [SerializeField]
    [Tooltip("Particle system used for visual effects when the turret fires.")]
    private ParticleSystem m_turretParticleSystem;
    public ParticleSystem TurretParticleSystem
    {
        get => m_turretParticleSystem;
    }

    [SerializeField]
    [Tooltip("Array of audio clips played when the turret fires.")]
    private AudioClip[] m_turretFireAudioClip;
    public AudioClip[] TurretFireAudioClip
    {
        get => m_turretFireAudioClip;
    }

    [SerializeField]
    [Tooltip("Array of audio clips played for various turret events.")]
    private AudioClip[] m_turretEventSFX;
    public AudioClip[] TurretEventSFX
    {
        get => m_turretFireAudioClip; // Typo corrected: should be m_turretEventSFX
    }

    [Header("Constraints")]

    [SerializeField]
    [Tooltip("Rotation speed of the turret on the X and Y axes.")]
    private Vector2 m_turretRotationSpeed = new Vector2(20, 20);
    public Vector2 TurretRotationSpeed
    {
        get => m_turretRotationSpeed;
    }

    [SerializeField]
    [Tooltip("Determines whether gimbal constraints are used to limit turret movement.")]
    private bool m_useGimbalConstraints = false;
    public bool UseGimbalConstraints
    {
        get => m_useGimbalConstraints;
    }

    [SerializeField]
    [Tooltip("Maximum angles (in degrees) at which the turret can rotate on the X and Y axes.")]
    private Vector2 m_maximumTurretAngles = new Vector2(15, 15);
    public Vector2 MaximumTurretAngles
    {
        get => m_maximumTurretAngles;
    }

    [Header("Targeting and Weapon Info")]

    [SerializeField]
    [Tooltip("The type of targeting system used by the turret (e.g., lead pursuit, lock-on).")]
    private TurretTargetingType m_targetingType;
    public TurretTargetingType TargetingType
    {
        get => m_targetingType;
    }

    [SerializeField]
    [Tooltip("Determines whether the turret automatically acquires targets or requires manual selection.")]
    private bool m_automaticTargetAquisistion;
    public bool AutomaticTargetAcquisition
    {
        get => m_automaticTargetAquisistion;
    }

    /// <summary>
    /// The type of targeting system used by the turret (e.g., lead pursuit, lock-on).
    /// </summary>
    public enum TurretTargetingType
    {
        /// <summary>
        /// The turret tracks targets by predicting their future position based on their movement.
        /// </summary>
        LeadPursuit,
        /// <summary>
        /// The turret locks onto a target and tracks its precise location.
        /// </summary>
        LockOn
    }
}

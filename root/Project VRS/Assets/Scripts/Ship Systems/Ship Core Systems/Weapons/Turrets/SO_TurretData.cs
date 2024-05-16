using System;
using UnityEngine;

[CreateAssetMenu(menuName = "New Turret Data")]
[Serializable]
public class SO_TurretData : ScriptableObject
{
    [Header("Constraints")]

    [SerializeField]
    [Tooltip("Rotation speed of the turret on the X and Y axes.")]
    private Vector2 m_turretRotationSpeed = new Vector2(20, 20);
    public Vector2 TurretRotationSpeed
    {
        get => m_turretRotationSpeed;
    }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Tooltip("Determines whether gimbal constraints are used to limit turret movement.")]
    private bool m_isGimballed = false;
    public bool IsGimballed
    {
        get => m_isGimballed;
    }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum angles (in degrees) at which the turret can rotate on the X and Y axes.")]
    private Vector2 m_constraintsOfXTurretAngles = new Vector2(15, 15);
    public Vector2 ConstraintsOfXTurretAngles
    {
        get => m_constraintsOfXTurretAngles;
    }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum angles (in degrees) at which the turret can rotate on the X and Y axes.")]
    private Vector2 m_constraintsOfYTurretAngles = new Vector2(15, 15);
    public Vector2 ConstraintsOfYTurretAngles
    {
        get => m_constraintsOfYTurretAngles;
    }

    [Header("Targeting and Weapon Info")]

    ///
    [SerializeField]
    [Tooltip("The type of targeting system used by the turret (e.g., lead pursuit, lock-on).")]
    private TurretTargetingType m_targetingType;
    public TurretTargetingType TargetingType
    {
        get => m_targetingType;
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

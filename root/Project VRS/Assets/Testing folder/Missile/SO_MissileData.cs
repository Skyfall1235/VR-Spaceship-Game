using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Simple Missile")]
[Serializable]
public class SO_MissileData : ScriptableObject
{
    /// <summary>
    /// Stores properties related to the launch of the missile.
    /// </summary>
    public LaunchProperties launchProperties = new LaunchProperties();

    /// <summary>
    /// A list containing information about the different stages the missile might have.
    /// </summary>
    public List<Stage> stages = new List<Stage>();

    /// <summary>
    /// Defines properties regarding interception behavior.
    /// </summary>
    public InterceptProperties interceptProperties = new InterceptProperties();

    [Serializable]
    public enum MotorType
    {
        /// <summary>
        /// The motor maintains a constant speed.
        /// </summary>
        ConstantSpeed,

        /// <summary>
        /// The motor accelerates over time.
        /// </summary>
        Acceleration,

        /// <summary>
        /// No motor is used in this stage.
        /// </summary>
        None
    }

    [Serializable]
    public enum LaunchMode
    {
        /// <summary>
        /// Launch the missile vertically upwards.
        /// </summary>
        Vertical,

        /// <summary>
        /// Launch the missile horizontally.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Launch the missile in the forward direction of the launching entity.
        /// </summary>
        Forward,

        /// <summary>
        /// Use a custom launch direction.
        /// </summary>
        Custom
    }

    [Serializable]
    public enum TargetType
    {
        /// <summary>
        /// The target is stationary and doesn't move.
        /// </summary>
        Stationary,

        /// <summary>
        /// The target is moving and its position might change.
        /// </summary>
        Dynamic
    }


    [Serializable]
    public struct LaunchProperties
    {
        /// <summary>
        /// Specifies the launch direction (Vertical, Horizontal, Forward, or Custom).
        /// </summary>
        public LaunchMode LaunchMode;

        /// <summary>
        /// Time the missile will track its target before launching (in seconds).
        /// </summary>
        public float TrackingTime;

        /// <summary>
        /// Initial speed of the missile upon launch (in meters per second).
        /// </summary>
        public float LaunchSpeed;

        /// <summary>
        /// Delay in seconds before the missile launches after being armed.
        /// </summary>
        public float LaunchDelay;
    }

    [Serializable]
    public struct Stage
    {
        /// <summary>
        /// A name or identifier for the specific stage (e.g., "Booster", "Second Stage").
        /// </summary>
        public string StageName;

        /// <summary>
        /// Type of motor used in this stage (ConstantSpeed, Acceleration, or None).
        /// </summary>
        public MotorType MotorType;

        /// <summary>
        /// Maximum duration (in seconds) for this stage to operate or burn.
        /// </summary>
        public float LimitTime;

        /// <summary>
        /// Speed of this stage (in meters per second).
        /// </summary>
        public float Speed;

        /// <summary>
        /// Whether this stage uses guidance for targeting (true or false).
        /// </summary>
        public bool UseGuidance;
    }

    [Serializable]
    public struct InterceptProperties
    {
        /// <summary>
        /// Effective range within which the missile considers a successful interception (in meters).
        /// </summary>
        public float HitRange;

        /// <summary>
        /// Controls whether guidance stops after the missile intercepts a target (true or false).
        /// </summary>
        public bool StopGuidanceAfterIntercept;

        /// <summary>
        /// Determines whether the missile self-destructs on hitting a target (true or false).
        /// </summary>
        public bool DestroyMissileOnHit;
    }
}

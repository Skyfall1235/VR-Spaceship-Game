using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BC_ShipInputHandler))]
public class BC_ShipMovementController : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The input handler of the entire ship
    /// </summary>
    [SerializeField]
    [Tooltip("Reference to the script handling ship input, such as joystick or keyboard input.")]
    protected BC_ShipInputHandler m_shipInputHandler;

    /// <summary>
    /// the rigidbody component of the entire ship
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum acceleration speed the ship can reach (0-100, higher values mean faster acceleration).")]
    protected Rigidbody m_shipRigidbody;

    /// <summary>
    /// maximum acceleration force that the ship can use
    /// </summary>
    [Range(0f, 100f)]
    [SerializeField]
    [Tooltip("Maximum acceleration speed the ship can reach (0-100, higher values mean faster acceleration).")]
    protected float m_maxAcceleration = 10f;

    /// <summary>
    /// maximum decceleration force that the ship can use
    /// </summary>
    [Range(-100f, 0f)]
    [SerializeField]
    [Tooltip("Maximum deceleration speed the ship can reach (-100-0, higher negative values mean faster deceleration).")]
    protected float m_maxDeceleration = -10f;

    /// <summary>
    /// maxmimum breaking value for the rigidbody
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum value for applying brakes (higher values mean stronger braking)")]
    protected float m_MaxBreakValue = 5f;

    /// <summary>
    /// maximum amount of torque that can be applied to the rigidbody
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum rotation speed the ship can achieve")]
    protected float m_maxRateOfPitch = 10f;

    /// <summary>
    /// maximum amount of torque that can be applied to the rigidbody
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum rotation speed the ship can achieve")]
    protected float m_maxRateOfRoll = 10f;

    /// <summary>
    /// maximum amount of torque that can be applied to the rigidbody
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum rotation speed the ship can achieve")]
    protected float m_maxRateOfYaw = 10f;

    /// <summary>
    /// the maximum amount of force that can be applied to strafe
    /// </summary>
    [SerializeField]
    protected float m_maxStrafeSpeed
    {
        get
        {
            return m_maxAcceleration / 2;
        }
    }

    /// <summary>
    /// The maximum speed of the ship.
    /// </summary>
    [SerializeField]
    protected float m_maxSpeed = 100f;

    /// <summary>
    /// Event raised when the ship's transform changes.
    /// </summary>
    [Serializable]
    public class TransformChangeEvent : UnityEvent<float> { }

    /// <summary>
    /// Event raised when the ship's rotation changes.
    /// </summary>
    [SerializeField]
    [Tooltip("Events to trigger when the ships rotational value changes")]
    protected TransformChangeEvent m_OnRotationChangeEvent = new TransformChangeEvent();

    /// <summary>
    /// Event raised when the ship's linear movement changes.
    /// </summary>
    [SerializeField]
    [Tooltip("Events to trigger when the linear movement value changes")]
    public TransformChangeEvent m_OnVelocityChangeEvent = new TransformChangeEvent();

    /// <summary>
    /// Gets the event raised when the ship's rotation changes.
    /// </summary>
    public TransformChangeEvent OnRotationChangeEvent => m_OnRotationChangeEvent;

    /// <summary>
    /// Gets the event raised when the ship's linear movement changes.
    /// </summary>
    public TransformChangeEvent OnVelocityChangeEvent => m_OnVelocityChangeEvent;

    /// <summary>
    /// Event raised when ship joystick input is updated.
    /// </summary>
    public UnityEvent<ShipJoystickInput> OnUpdateInputs = new UnityEvent<ShipJoystickInput>();

    #endregion

    private void OnValidate()
    {
        if (m_maxDeceleration + 0.1f >= m_maxAcceleration)
        {
            m_maxDeceleration = m_maxAcceleration - 0.1f;
        }
    }

    #region Application of Movement

    /// <summary>
    /// Applies forward thrust to the ship based on the Z component of the provided vector.
    /// Raises the `m_OnVelocityChangeEvent` event with the applied thrust value.
    /// </summary>
    /// <param name="forwardVector">The direction and strength of the desired thrust (Z component determines thrust).</param>
    internal void ApplyLinearMotionValue(Vector3 forwardVector)
    {
        float appliedThrustValue = forwardVector.z;
        m_shipRigidbody.AddRelativeForce(forwardVector, ForceMode.Acceleration);
        m_OnVelocityChangeEvent.Invoke(appliedThrustValue);
    }

    /// <summary>
    /// Applies rotation to the ship around the specified axis with the given speed.
    /// Raises the `m_OnRotationChangeEvent` event with the applied rotation speed.
    /// </summary>
    /// <param name="rotationAxisWithMappedSpeed">The axis and mapped speed for the desired rotation.</param>
    /// <param name="newRotationSpeed">The final rotation speed to apply.</param>
    internal void ApplyRotationOnAxis(Vector3 rotationAxisWithMappedSpeed, float newRotationSpeed)
    {
        m_shipRigidbody.AddRelativeTorque(rotationAxisWithMappedSpeed, ForceMode.Acceleration);
        m_OnRotationChangeEvent.Invoke(newRotationSpeed);
    }

    /// <summary>
    /// Applies up, down, left, and right strafing movement to the ship based on the X component of the provided vector and an elvation speed.
    /// Raises the `m_OnVelocityChangeEvent` event with the applied strafe value.
    /// </summary>
    /// <param name="strafeVector">The direction and strength of the desired strafe (X component determines strafe).</param>
    internal void ApplyStrafe(Vector3 strafeVector)
    {
        float appliedStrafeValue = strafeVector.x;
        m_shipRigidbody.AddRelativeForce(strafeVector, ForceMode.Acceleration);
        m_OnVelocityChangeEvent.Invoke(appliedStrafeValue);
    }

    internal void ApplyElavation(float elavationSpeed)
    {
        Vector3 elavation = transform.up * elavationSpeed;
        m_shipRigidbody.AddRelativeForce(elavation, ForceMode.Acceleration);
        m_OnVelocityChangeEvent.Invoke(elavationSpeed);
    }

    #endregion

    #region Math & Returns

    //possibily an optimisation to change this to setting an inverse force instead of a clamp
    internal void ClampVelocityToMaxSpeed()
    {
        if (m_shipRigidbody.velocity.magnitude > m_maxSpeed)
        {
            m_shipRigidbody.velocity = Vector3.ClampMagnitude(m_shipRigidbody.velocity, m_maxSpeed);
        }
    }
    #endregion
}

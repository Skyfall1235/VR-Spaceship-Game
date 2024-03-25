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
    /// maximum amount of torque that can be allied to the rigidbody
    /// </summary>
    [SerializeField]
    [Tooltip("Maximum rotation speed the ship can achieve")]
    protected float m_maxRateOfTurn = 10f;

    /// <summary>
    /// the toggle between strafing and yaw rotation
    /// </summary>
    [SerializeField]
    [Tooltip("Toggle option for strafing movement")]
    protected bool m_toggleStrafe;

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
    /// 
    /// </summary>
    [SerializeField]
    protected float m_maxSpeed = 100f;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TransformChangeEvent : UnityEvent<float> { }

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Tooltip("Events to trigger when the ships rotational value changes")]
    protected TransformChangeEvent m_OnRotationChangeEvent = new TransformChangeEvent();

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    [Tooltip("Events to trigger when the linear movement value changes")]
    public TransformChangeEvent m_OnVelocityChangeEvent = new TransformChangeEvent();

    public TransformChangeEvent OnRotationChangeEvent => m_OnRotationChangeEvent;
    public TransformChangeEvent OnVelocityChangeEvent => m_OnVelocityChangeEvent;

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
    /// Applies strafing movement to the ship based on the X component of the provided vector.
    /// Raises the `m_OnVelocityChangeEvent` event with the applied strafe value.
    /// </summary>
    /// <param name="strafeVector">The direction and strength of the desired strafe (X component determines strafe).</param>
    internal void ApplyStrafe(Vector3 strafeVector)
    {
        float appliedStrafeValue = strafeVector.x;
        m_shipRigidbody.AddRelativeForce(strafeVector, ForceMode.Acceleration);
        m_OnVelocityChangeEvent.Invoke(appliedStrafeValue);
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

    /// <summary>
    /// a method to find the cross product of a Vector2 and Vector3.Up
    /// </summary>
    /// <param name="inputVal">is the Vector2 we wish to get the cross product with</param>
    /// <returns>the cross product of a Vector2 and Vector3.Up</returns>
    internal Vector3 FindAxisForRotation(Vector2 inputVal)
    {
        Vector3 convertedInputVal = new Vector3(inputVal.x, 0f, inputVal.y);
        Vector3 crossAxis = Vector3.Cross(convertedInputVal, Vector3.up);
        return crossAxis;
    }

    /// <summary>
    /// Remaps a value from one range to another, potentially scaling and clamping it within the new range.
    /// </summary>
    /// <param name="value">The input value to be remapped.</param>
    /// <param name="originalMin">The minimum value of the original range.</param>
    /// <param name="originalMax">The maximum value of the original range.</param>
    /// <param name="newMin">The minimum value of the desired new range.</param>
    /// <param name="newMax">The maximum value of the desired new range.</param>
    /// <returns>The remapped value within the new range.</returns>
    internal float Remap(float value, float originalMin, float originalMax, float newMin, float newMax)
    {
        // Normalize value to the range [0, 1]
        float normalizedValue = (value - originalMin) / (originalMax - originalMin);

        // Rescale normalized value to the new range
        float rescaledValue = normalizedValue * (newMax - newMin) + newMin;
        return rescaledValue;
    }

    #endregion
}

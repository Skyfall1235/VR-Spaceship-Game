using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(ShipInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class ShipMovementController : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The input handler of the entire ship
    /// </summary>
    [SerializeField] private ShipInputHandler m_shipInputHandler;

    /// <summary>
    /// the rigidbody component of the entire ship
    /// </summary>
    [SerializeField] private Rigidbody m_shipRigidbody;

    [Range(0f, 10f)]
    public float maxAcceleration = 10f;
    [Range(0f, 10f)]
    public float maxDeceleration = 5f;

    public float maxRateOfTurn = 10f;

    public float maxStrafeSpeed
    {
        get
        {
            return maxAcceleration / 2;
        }
    }

    [Range(0f, 1000f)]
    public const float maxSpeed = 100f;
    const float lowerJoystickInputBounds = -2f;
    const float upperJoystickInputBounds = 2f;

    //continously applied vectors that change upon setting values for the inputs
    private Vector3 savedForwardVector;

    private Vector3 savedRotationAxis;
    private float savedRotationSpeed;

    private Vector3 savedStrafeVector;





    [Serializable]
    public class TransformChangeEvent : UnityEvent<float> { }

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's y value changes")]
    TransformChangeEvent m_OnRotationChangeEvent = new TransformChangeEvent();

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's x value changes")]
    TransformChangeEvent m_OnVelocityChangeEvent = new TransformChangeEvent();

    public TransformChangeEvent OnRotationChangeEvent => m_OnRotationChangeEvent;
    public TransformChangeEvent OnVelocityChangeEvent => m_OnVelocityChangeEvent;


    [SerializeField] Logger logger;


    private void FixedUpdate()
    {
        
    }








    #endregion

    #region Application of Motion

    public void UpdateLinearMotionVector(float rawThrottleInput)
    {
        float appliedThrustValue = Remap(rawThrottleInput,        // Value to modify
                                        lowerJoystickInputBounds, // Lower original bound
                                        upperJoystickInputBounds, // Upper original bound
                                        maxDeceleration,          // Lower new bound
                                        maxAcceleration);         // Upper new bound

        Vector3 forwardVector = new Vector3(appliedThrustValue, 0f, 0f);
        //save the value
        savedForwardVector = forwardVector;

    }

    public void UpdateRotationOnAxis(Vector3 axis, float rotationSpeed)
    {
        //ADD-RELATIVE-TORQUE ROTATES THE BODY CLOCKWISE AROUND THE AXIS (if it is counter clockwise your visualization is upside down)

        float newRotationSpeed = Remap(rotationSpeed,            // Value to modify
                                       lowerJoystickInputBounds, // Lower original bound
                                       upperJoystickInputBounds, // Upper original bound
                                       0f,                       // Lower new bound
                                       maxRateOfTurn);           // Upper new bound

        //since the axis is a normalized vector we can just multiply it
        Vector3 rotationAxisWithMappedSpeed = axis * newRotationSpeed;
        //save the values
        savedRotationAxis = rotationAxisWithMappedSpeed;
        savedRotationSpeed = newRotationSpeed;
    }

    public void UpdateStrafeVector(float rawYawInput)
    {
        float appliedYawValue = Remap(rawYawInput,              // Value to modify  
                                      lowerJoystickInputBounds, // Lower original bound
                                      upperJoystickInputBounds, // Upper original bound
                                      0f,                       // Lower new bound
                                      maxStrafeSpeed);          // Upper new bound

        //save the values
        //NOT FINISHED
        //savedStrafeVector = ;

    }

    #endregion

    #region Internal execution

    void ApplyLinearMotionValue(Vector3 forwardVector, float appliedThrustValue)
    {
        m_shipRigidbody.AddRelativeForce(forwardVector, ForceMode.Force);
        m_OnVelocityChangeEvent.Invoke(appliedThrustValue);
    }

    void ApplyRotationOnAxis(Vector3 rotationAxisWithMappedSpeed, float newRotationSpeed)
    {
        m_shipRigidbody.AddRelativeTorque(rotationAxisWithMappedSpeed, ForceMode.Force);
        m_OnRotationChangeEvent.Invoke(newRotationSpeed);
    }

    void ApplyStrafe(Vector3 strafeVector, float appliedStrafeValue)
    {
        m_shipRigidbody.AddRelativeForce(strafeVector, ForceMode.Force);
        m_OnVelocityChangeEvent.Invoke(appliedStrafeValue);
    }

    void ClampVelocityToMaxSpeed()
    {
        if (m_shipRigidbody.velocity.magnitude > maxSpeed)
        {
            m_shipRigidbody.velocity = Vector3.ClampMagnitude(m_shipRigidbody.velocity, maxSpeed);
        }
    }

    #endregion

    #region Math

    //calculate the rotation axis for a turn
    private Vector3 FindAxisForRotation(Vector2 axis, Transform localTransform)
    {
        Vector3 crossAxis = Vector3.Cross(axis, localTransform.up);
        return crossAxis.normalized;
    }

    private Vector3 FindYawDirection(Vector2 joystickInput)
    {
        //NOT FINISHED
        return Vector3.zero;
    }

    public static float Remap(float value, float originalMin, float originalMax, float newMin, float newMax)
    {
        // Normalize value to the range [0, 1]
        float normalizedValue = (value - originalMin) / (originalMax - originalMin);

        // Rescale normalized value to the new range
        float rescaledValue = normalizedValue * (newMax - newMin) + newMin;
        return rescaledValue;
    }

    #endregion

    #region Monobehavior

    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.magenta;

        // Get the input vector (assuming it's a Vector2)
        //Vector2 inputVector;

        // Calculate the desired direction in world space based on transform.forward
        //Vector3 desiredDirection = transform.forward + new Vector3(inputVector.y, inputVector.x, 0f); // Set Z to 0

        // Calculate the end point of the line in world space
        //Vector3 endPoint = transform.position + desiredDirection * 4f; // Adjust the scaling factor as needed

        // Draw the line
        //Gizmos.DrawLine(transform.position, endPoint);
    }

    private void OnValidate()
    {
        if(maxDeceleration + 0.1f >= maxAcceleration)
        {
            maxDeceleration = maxAcceleration - 0.1f;
        }
    }

    #endregion
}



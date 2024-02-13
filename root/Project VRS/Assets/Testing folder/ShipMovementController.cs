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
    public float maxDeceleration = -5f;

    public float MaxBreakValue = 5f;

    public float maxRateOfTurn = 10f;

    [Range(0f, 1f)]
    public float DampeningMultiplier = 0.75f;

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

    #region Monobehavior

    private void FixedUpdate()
    {
        //this will apply all current inputs. this does not SLOW DOWN or stop inputs.
        ApplyAllCurrentForces();
    }

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
        if (maxDeceleration + 0.1f >= maxAcceleration)
        {
            maxDeceleration = maxAcceleration - 0.1f;
        }
    }

    #endregion


    #endregion

    #region Application of Motion
    public void CallUpdateForLinearMotion()
    {
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        float secondaryJoystickYVal = currentInput.ThrustValue;

        SaveNewLinearMotionVector(secondaryJoystickYVal);
        //unlike the rest of the methods, brakes can be applied even if you are using thrust.
    }

    public void CallUpdateForRotation()
    {
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        float rotationSpeed = currentInput.PrimaryFlightStick.x; // this might be wrong, come back later
        Vector3 axisOfRotation = FindAxisForRotation(currentInput.PrimaryFlightStick, transform);//this might also be problematic

        if(currentInput.PrimaryFlightStick != Vector2.zero)
        {
            SaveNewRotationOnAxis(axisOfRotation, rotationSpeed);
        }
        else { DecelerateRotationalMovement(); }
        
    }

    public void CallUpdateForStrafe()
    {
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();

        if(currentInput.yawValue != 0)
        {
            SaveNewStrafeVector(currentInput.yawValue);
        }
        else { DecelerateStrafe(); }
        
    }

    private void ApplyAllCurrentForces()
    {
        ApplyLinearMotionValue(savedForwardVector);
        ApplyRotationOnAxis(savedRotationAxis, savedRotationSpeed);
        ApplyStrafe(savedStrafeVector);
        //if()
    }

    #endregion

    #region Application

    //this section is to help slow down the RB
    //we can add if statements to the call updates above to run these modules

    void BrakeLinearMovement()
    {
        //get the trigger value from the hand that is currently grabbing the joystick
        Vector3 currentLinearVector = savedForwardVector;
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();

        //get the current trigger value
        float triggerVal = currentInput.BreakValue;
        //set the consts
        const float minTriggerVal = 0f;
        const float maxTriggerVal = 1f;

        //remap the trigger value to be equal to the breakforce to be applied against the linear motion
        float mappedTriggerVal = Remap(triggerVal, minTriggerVal, maxTriggerVal, 0f, MaxBreakValue);

        //apply a force opposite of the current vector normalized, multiplied by the trigger val
        Vector3 BreakingVector = (-currentLinearVector.normalized * mappedTriggerVal);
        ApplyLinearMotionValue(BreakingVector);
    }

    //will be needed for when the throttle is at zero
    void DecelerateLinearMovement()
    {
        //determine inverted force needed on the fly based on the current linear velocity
        Vector3 currentInputVector = savedForwardVector;
        Vector3 invertedDampendVector  = -currentInputVector * DampeningMultiplier;
        ApplyLinearMotionValue(invertedDampendVector);
    }

    void DecelerateRotationalMovement()
    {
        //determine inverted torque needed on the fly based on the current torque
        Vector3 currentTorque = savedRotationAxis;
        float currentRotationalSpeed = savedRotationSpeed;

        //invert the vector3 and apply it when input is equal to zero, so it stops the rotation of whereever it is
        Vector3 invertedTorque = -currentTorque;
        ApplyRotationOnAxis(invertedTorque * DampeningMultiplier, -currentRotationalSpeed * DampeningMultiplier);//this might be problematic, but it think i just need to invert the rotational speed?
    }

    void DecelerateStrafe()
    {
        //determine inverted force needed on the fly based on the current strafe velocity
        Vector3 currentStrafe = savedStrafeVector;
        Vector3 invertedStrafe = -currentStrafe;
        ApplyStrafe(invertedStrafe * DampeningMultiplier);
    }

    #endregion

    #region Internal execution

    private void SaveNewLinearMotionVector(float rawThrottleInput)
    {
        float appliedThrustValue = Remap(rawThrottleInput,        // Value to modify
                                        lowerJoystickInputBounds, // Lower original bound
                                        upperJoystickInputBounds, // Upper original bound
                                        maxDeceleration,          // Lower new bound
                                        maxAcceleration);         // Upper new bound

        Vector3 forwardVector = new Vector3(0f, 0f, appliedThrustValue);
        //save the value
        savedForwardVector = forwardVector;
    }

    private void SaveNewRotationOnAxis(Vector3 axis, float rotationSpeed)
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

    //since the strafe is just the x value of the secondary joystick, it can just take a float
    private void SaveNewStrafeVector(float rawYawInput)
    {
        float appliedYawValue = Remap(rawYawInput,              // Value to modify  
                                      lowerJoystickInputBounds, // Lower original bound
                                      upperJoystickInputBounds, // Upper original bound
                                      0f,                       // Lower new bound
                                      maxStrafeSpeed);          // Upper new bound
        //set the vector for the strafe to the speed of the strafe value
        Vector3 strafeVector = new Vector3(appliedYawValue, 0f, 0f);
        //save the values
        savedStrafeVector = strafeVector;
    }

    ShipJoystickInput GrabCurrentShipControlInputs()
    {
        return m_shipInputHandler.CurrentShipJoystickInputs;
    }

    void ApplyLinearMotionValue(Vector3 forwardVector)
    {
        float appliedThrustValue = forwardVector.z;
        m_shipRigidbody.AddRelativeForce(forwardVector, ForceMode.Force);
        m_OnVelocityChangeEvent.Invoke(appliedThrustValue);
    }

    void ApplyRotationOnAxis(Vector3 rotationAxisWithMappedSpeed, float newRotationSpeed)
    {
        //float newRotationSpeed = rotationAxisWithMappedSpeed.
        m_shipRigidbody.AddRelativeTorque(rotationAxisWithMappedSpeed, ForceMode.Force);
        m_OnRotationChangeEvent.Invoke(newRotationSpeed);
    }

    void ApplyStrafe(Vector3 strafeVector)
    {
        float appliedStrafeValue = strafeVector.x;
        m_shipRigidbody.AddRelativeForce(strafeVector, ForceMode.Force);
        m_OnVelocityChangeEvent.Invoke(appliedStrafeValue);
    }

    //possibily an optimisattion to change this to setting an inverse force instead of a clamp
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
    private Vector3 FindAxisForRotation(Vector2 inputVal, Transform localTransform)
    {
        Vector3 crossAxis = Vector3.Cross(inputVal, localTransform.up);
        return crossAxis.normalized;
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

}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using System;
using UnityEngine.Events;


public class ShipMovementController : BC_ShipMovementController
{
    #region Variables
    //magic numbers for remapping the vector inputs
    const float m_lowerJoystickInputBounds = -1f;
    const float m_upperJoystickInputBounds = 1f;

    //continously applied vectors that change upon setting values for the inputs
    private Vector3 m_savedForwardVector;

    private Vector3 m_savedRotationAxis;
    private float m_savedRotationSpeed;

    private Vector3 m_savedStrafeVector;

    private Vector3 m_savedYawAxis;
    private float m_savedYawSpeed;

    private bool m_toggleYawForStrafe;

    [SerializeField] Logger m_logger;

    #endregion

    private void FixedUpdate()
    {
        //this will apply all current inputs
        ApplyAllCurrentForces();
    }

    #region Application of Motion

    private void ApplyAllCurrentForces()
    {
        //linear motion
        ApplyLinearMotionValue(m_savedForwardVector);

        //roll and pitch rotations
        ApplyRotationOnAxis(m_savedRotationAxis, m_savedRotationSpeed);

        if(m_toggleYawForStrafe == true) //true means we strafe, false means we rotate yaw
        {
            //apply strafe movement
            ApplyStrafe(m_savedStrafeVector);
        }
        else
        {
            //yaw rotations
            ApplyRotationOnAxis(m_savedYawAxis, m_savedYawSpeed);
        }

        //finally, prevent the speed from breaching the speed cap
        ClampVelocityToMaxSpeed();
    }

    public void CallUpdateForLinearMotion()
    {
        //get the current inputs
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        float secondaryJoystickYVal = currentInput.ThrustValue;

        SaveNewLinearMotionVector(secondaryJoystickYVal);
        //unlike the rest of the methods, brakes can be applied even if you are using thrust.
    }

    public void CallUpdateForRotation()
    {
        //get the current inputs
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        float rotationSpeed = Vector2.Distance(Vector2.zero, currentInput.PrimaryFlightStick);
        Vector3 axisOfRotation = FindAxisForRotation(currentInput.PrimaryFlightStick);
        //apply all rotations - pitch, roll, and yaw
        SaveNewRotationOnAxis(axisOfRotation, rotationSpeed, currentInput.yawValue);
    }

    public void CallUpdateForStrafe()
    {
        //get the current inputs
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        //save new strafe takes the raw yaw value and maps it, then saves it
        SaveNewStrafeVector(currentInput.yawValue);
    }

    #endregion

    #region Internal execution

    private void SaveNewLinearMotionVector(float rawThrottleInput)
    {
        //remap the joystick value
        float appliedThrustValue = Remap(rawThrottleInput,        // Value to modify
                                        m_lowerJoystickInputBounds, // Lower original bound
                                        m_upperJoystickInputBounds, // Upper original bound
                                        m_maxDeceleration,          // Lower new bound
                                        m_maxAcceleration);         // Upper new bound

        //create a forward vector of the speed we want to move
        Vector3 forwardVector = new Vector3(0f, 0f, appliedThrustValue);
        
        //save the value
        m_savedForwardVector = forwardVector;
    }

    private void SaveNewRotationOnAxis(Vector3 PitchAndRollAxis, float rotationSpeed, float rawYawInput)
    {
        //ADD-RELATIVE-TORQUE ROTATES THE BODY CLOCKWISE AROUND THE AXIS (if it is counter clockwise your visualization is upside down)

        //remap the joystick values
        float newRotationSpeed = Remap(rotationSpeed,              // Value to modify
                                       m_lowerJoystickInputBounds, // Lower original bound
                                       m_upperJoystickInputBounds, // Upper original bound
                                       0f,                         // Lower new bound
                                       m_maxRateOfTurn);           // Upper new bound

        float newYawRotationSpeed = Remap(rawYawInput,                // Value to modify
                                       m_lowerJoystickInputBounds, // Lower original bound
                                       m_upperJoystickInputBounds, // Upper original bound
                                       0f,                         // Lower new bound
                                       m_maxRateOfTurn);           // Upper new bound
    //Pitch and Roll
        //invert the axis. id why but it needed to happen, the math said this shouldnt be needed??
        Vector3 invertedVector = -PitchAndRollAxis;
        //since the axis is a normalized vector we can just multiply it to get our torque value
        Vector3 rotationAxisWithMappedSpeed = invertedVector * newRotationSpeed;
    //Yaw
        //determine the direction based on the torque roation
        Vector3 TorqueVector;
        if (rawYawInput > 0f) 
        { 
            TorqueVector = Vector3.up; 
        } 
        else if (rawYawInput < 0f)
        { 
            TorqueVector = Vector3.down; 
        }
        else { TorqueVector = Vector3.zero; }
        //set the vector to be the speed of the rotation
        Vector3 yawAxisWithMappedSpeed = TorqueVector * newRotationSpeed;
    //Saving
        //save Pitch and roll
        m_savedRotationAxis = rotationAxisWithMappedSpeed;
        m_savedRotationSpeed = newRotationSpeed;
        //save yaw
        m_savedYawAxis = yawAxisWithMappedSpeed;
        m_savedYawSpeed = newYawRotationSpeed;
    }

    private void SaveNewStrafeVector(float rawYawInput)
    {
        //remap the joystick value
        float appliedYawValue = Remap(rawYawInput,                // Value to modify  
                                      m_lowerJoystickInputBounds, // Lower original bound
                                      m_upperJoystickInputBounds, // Upper original bound
                                      -m_maxStrafeSpeed,          // Lower new bound
                                      m_maxStrafeSpeed);          // Upper new bound

        //set the vector for the strafe to the speed of the strafe value
        Vector3 strafeVector = new Vector3(appliedYawValue, 0f, 0f);

        //save the values
        m_savedStrafeVector = strafeVector;
    }

    ShipJoystickInput GrabCurrentShipControlInputs()
    {
        //attempt cast to inherited member with new variable
        if(m_shipInputHandler is PlayerShipInputHandler)
        {
            PlayerShipInputHandler convertedHandler = (PlayerShipInputHandler)m_shipInputHandler;
            return convertedHandler.CurrentShipJoystickInputs;
        }
        //if cast fails, notify with an error
        Debug.LogError("Unable to convert base class input handler to Player input handler\nReturning empty inputs");
        return new ShipJoystickInput();
    }

    #endregion
}



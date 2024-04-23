using UnityEngine;

public class IM_ShipMovementController : BC_ShipMovementController
{
    //magic numbers for remapping the vector inputs
    const float m_lowerJoystickInputBounds = -1f;
    const float m_upperJoystickInputBounds = 1f;

    //continously applied vectors that change upon setting values for the inputs
    protected Vector3 m_savedForwardVector;

    protected Vector3 m_savedRotationAxis;
    protected float m_savedRotationSpeed;

    protected Vector3 m_savedStrafeVector;

    protected Vector3 m_savedYawAxis;
    protected float m_savedYawSpeed;

    protected bool m_toggleYawForStrafe;

    protected float m_brakingForce;


    protected virtual void FixedUpdate()
    {
        //this will apply all current inputs
        ApplyAllCurrentForces();
    }

    private void ApplyAllCurrentForces()
    {
        //linear motion
        ApplyLinearMotionValue(m_savedForwardVector);

        //roll and pitch rotations
        ApplyRotationOnAxis(m_savedRotationAxis, m_savedRotationSpeed);

        if (m_toggleYawForStrafe == true) //true means we strafe, false means we rotate yaw
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

    protected void SaveNewLinearMotionVector(float rawThrottleInput)
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

    protected void SaveNewRotationOnAxis(Vector3 PitchAndRollAxis, float rotationSpeed, float rawYawInput)
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

    protected void SaveNewStrafeVector(float rawYawInput)
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
}

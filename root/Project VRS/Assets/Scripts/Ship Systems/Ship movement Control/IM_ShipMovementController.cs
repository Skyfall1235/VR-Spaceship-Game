using UnityEngine;

public class IM_ShipMovementController : BC_ShipMovementController
{
    //magic numbers for remapping the vector inputs
    const float m_lowerJoystickInputBounds = -1f;
    const float m_upperJoystickInputBounds = 1f;

    //continously applied vectors that change upon setting values for the inputs
    protected Vector3 m_savedForwardVector;

    protected Vector3 m_savedEulerOfComputedQuaterionAxis;
    protected float m_savedPitchRotationSpeed;

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

        //pitch rotations
        ApplyRotationOnAxis(m_savedEulerOfComputedQuaterionAxis, m_savedPitchRotationSpeed);

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

    protected void SaveNewRotationOnAxis(Vector3 newDirection, Rigidbody shipRB)
    {
        Vector3 currentDir = transform.forward;

        var x = Vector3.Cross(currentDir.normalized, newDirection.normalized);
        float theta = Mathf.Asin(x.magnitude);
        var w = x.normalized * theta / Time.fixedDeltaTime;
        var q = transform.rotation * shipRB.inertiaTensorRotation;
        var t = q * Vector3.Scale(shipRB.inertiaTensor, Quaternion.Inverse(q) * w);
        m_savedEulerOfComputedQuaterionAxis = (t - shipRB.angularVelocity).normalized;
        m_savedPitchRotationSpeed = (t - shipRB.angularVelocity).magnitude;
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

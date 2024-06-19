using UnityEngine;

public class ShipMovementController : IM_ShipMovementController
{
    #region Variables

    [SerializeField] Logger m_logger;

    #endregion

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    #region Called by unity Events
    public void CallUpdateForLinearMotion()
    {
        //get the current inputs
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        OnUpdateInputs.Invoke(currentInput);
        float secondaryJoystickYVal = currentInput.ThrustValue;

        SaveNewLinearMotionVector(secondaryJoystickYVal);
        //unlike the rest of the methods, brakes can be applied even if you are using thrust.
    }

    public void CallUpdateForRotation()
    {
        //get the current inputs
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        OnUpdateInputs.Invoke(currentInput);

        //TESTING
        //X, 0, Y : HANDLE RIGHT - DOWN AND LEFT (

        Quaternion newRotation = Quaternion.Euler(currentInput.PrimaryFlightStick.x, 0f, currentInput.PrimaryFlightStick.y) * transform.rotation;
        Vector3 newForward = newRotation * transform.forward;


        //apply all rotations - pitch, roll, and yaw
        SaveNewRotationOnAxis(newForward, m_shipRigidbody);
    }

    public void CallUpdateForStrafe()
    {
        //get the current inputs
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        OnUpdateInputs.Invoke(currentInput);

        //save new strafe takes the raw yaw value and maps it, then saves it
        SaveNewStrafeVector(currentInput.yawValue);
    }

    //THIS IS TEMP
    public void CallUpdateForBraking()
    {
        ShipJoystickInput currentInput = GrabCurrentShipControlInputs();
        OnUpdateInputs.Invoke(currentInput);
        m_shipRigidbody.drag = m_brakingForce * currentInput.BreakValue;
    }

    #endregion

    #region Internal execution

    ShipJoystickInput GrabCurrentShipControlInputs()
    {
        //attempt cast to inherited member with new variable
        
        if (m_shipInputHandler is PlayerShipInputHandler)
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Content.Interaction;

public class ShipInputHandler : MonoBehaviour
{
    #region Variables
    //reference to primary controls
    private XRJoystick m_primaryShipJoystick;
    public XRJoystick PrimaryShipJoystick
    {
        get => m_primaryShipJoystick;
    }

    private XRJoystick m_secondaryShipJoystick;
    public XRJoystick SecondaryShipJoystick
    {
        get => m_secondaryShipJoystick;
    }

    public bool useKeyboardControls = true;

    [SerializeField] Logger logger;

    //TEMP
    public float breakVal = 0;

    #endregion

    public ShipJoystickInput CurrentShipJoystickInputs
    {
        get
        {
            // Check for missing references and log a single, comprehensive message
            if (m_primaryShipJoystick == null || m_secondaryShipJoystick == null)
            {
                Debug.LogWarning("Either primary ship joystick or secondary ship joystick reference is missing.");
                return new();  // Return an empty struct to indicate an error
            }

            // Use a concise conditional expression for input selection
            return useKeyboardControls ? InputEncoder(m_primaryShipJoystick.value, m_secondaryShipJoystick.value, breakVal) : InputEncoder(KeyboardInputControls());
        }
    }

    private (Vector2, Vector2, float) KeyboardInputControls()
    {
        Vector2 rightHand = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        Vector2 leftHand = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float breakVal = Input.GetAxis("Jump");
        return (rightHand, leftHand, breakVal);
    }

    //we need a decoder to unformat the code into a reasonable format
    private ShipJoystickInput InputEncoder(Vector2 primaryInput, Vector2 secondaryInput, float breakVal)
    {
        //create and setup the ship input
        ShipJoystickInput newInput = new ShipJoystickInput(primaryInput, secondaryInput.x, secondaryInput.y, breakVal);
        return newInput;
    }
    //overload
    private ShipJoystickInput InputEncoder((Vector2, Vector2, float) keyboardInput)
    {
        Vector2 rightHand = keyboardInput.Item1;
        Vector2 leftHand = keyboardInput.Item2;
        float breakVal = keyboardInput.Item3;
        return InputEncoder(rightHand, leftHand, breakVal);
    }
}

public struct ShipJoystickInput
{
    public Vector2 PrimaryFlightStick; // x and y are the joystick movement values
    public float yawValue;
    public float ThrustValue;
    public float BreakValue;

    public ShipJoystickInput(Vector2 primaryFlightStick, float yawValue, float thrustValue, float breakValue)
    {
        this.PrimaryFlightStick = primaryFlightStick;
        this.yawValue = yawValue;
        this.ThrustValue = thrustValue;
        this.BreakValue = breakValue;
    }

    public ShipJoystickInput(Vector4 totalInput)
    {
        this.PrimaryFlightStick = new(totalInput.x, totalInput.y);
        this.yawValue = totalInput.z;
        this.ThrustValue = totalInput.w;
        this.BreakValue = 0f;
    }


    //might not be needed

    float ConvertToSignedRange(float value)
    {
        float convertedNum = (value * 2) - 1;
        return convertedNum;
    }
}

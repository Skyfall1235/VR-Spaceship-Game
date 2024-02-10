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
            return useKeyboardControls ? InputEncoder(m_primaryShipJoystick.value, m_secondaryShipJoystick.value) : InputEncoder(KeyboardInputControls());
        }
    }

    private (Vector2, Vector2) KeyboardInputControls()
    {
        Vector2 rightHand = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        Vector2 leftHand = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        return (rightHand, leftHand);
    }

    //we need a decoder to unformat the code into a reasonable format
    private ShipJoystickInput InputEncoder(Vector2 primaryInput, Vector2 secondaryInput)
    {
        //create and setup the ship input
        ShipJoystickInput newInput = new ShipJoystickInput(primaryInput, secondaryInput.x, secondaryInput.y);
        return newInput;
    }
    //overload
    private ShipJoystickInput InputEncoder((Vector2, Vector2) keyboardInput)
    {
        Vector2 rightHand = keyboardInput.Item1;
        Vector2 leftHand = keyboardInput.Item2;
        return InputEncoder(rightHand, leftHand);
    }
}

public struct ShipJoystickInput
{
    public Vector2 PrimaryFlightStick; // x and y are the joystick movement values
    public float yawValue;
    public float ThrustValue;

    public ShipJoystickInput(Vector2 primaryFlightStick, float yawValue, float thrustValue)
    {
        this.PrimaryFlightStick = primaryFlightStick;
        this.yawValue = yawValue;
        this.ThrustValue = thrustValue;
    }

    public ShipJoystickInput(Vector4 totalInput)
    {
        this.PrimaryFlightStick = new(totalInput.x, totalInput.y);
        this.yawValue = totalInput.z;
        this.ThrustValue = totalInput.w;
    }


    //might not be needed

    float ConvertToSignedRange(float value)
    {
        float convertedNum = (value * 2) - 1;
        return convertedNum;
    }
}

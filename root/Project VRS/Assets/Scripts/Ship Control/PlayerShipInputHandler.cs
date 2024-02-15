using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Content.Interaction;

public class PlayerShipInputHandler : BC_ShipInputHandler
{
    #region Variables

    //reference to primary controls
    [SerializeField] private XRJoystick m_primaryShipJoystick;
    public XRJoystick PrimaryShipJoystick
    {
        get => m_primaryShipJoystick;
    }

    [SerializeField] private XRJoystick m_secondaryShipJoystick;
    public XRJoystick SecondaryShipJoystick
    {
        get => m_secondaryShipJoystick;
    }

    //TEMP
    public float breakVal = 0;

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
            //return useKeyboardControls ? InputEncoder(m_primaryShipJoystick.value, m_secondaryShipJoystick.value, breakVal) : InputEncoder(KeyboardInputControls());
            return InputEncoder(m_primaryShipJoystick.value, m_secondaryShipJoystick.value, breakVal);
        }
    }

    private (Vector2, Vector2, float) KeyboardInputControls()
    {
        Vector2 rightHand = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        Vector2 leftHand = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float breakVal = Input.GetAxis("Jump");
        return (rightHand, leftHand, breakVal);
    }
}

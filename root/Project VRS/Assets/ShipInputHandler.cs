using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Content.Interaction;

public class ShipInputHandler : MonoBehaviour
{
    //xrs input coincides with the update method
    public Vector4 PlayerInputValue;//xand y are the joystick inputs, while z is the thrust amount

    //reference to primary controls
    public XRJoystick m_shipJoystick;
    public XRSlider m_shipThrottle;

    public bool useKeyboardControls = true;


    private void FixedUpdate()
    {
        KeyboardInputControls();//TEMP
    }




    public void UpdatePlayerInput()
    {
        if (useKeyboardControls)
        {
            KeyboardInputControls();
        }
        else JoystickAndThrottleControls();
    }

    private void JoystickAndThrottleControls()
    {
        var REPLACEWITHYAW = 0f;
        if (m_shipJoystick != null && m_shipThrottle != null)
        {
            Debug.LogWarning("YOU ARE USING THE JOYSTICK WITHOUT YAW"); //casn be removed after fixing :)
            Vector4 formattedInput = new(m_shipJoystick.value.x, m_shipJoystick.value.y, REPLACEWITHYAW, ConvertToSignedRange(m_shipThrottle.value)); //TEMP PLEASE FIX TO YAW VALUE OF JOYSTICK
            PlayerInputValue = formattedInput;
        }
        else Debug.LogWarning("joystick or throttle reference is missing");
    }

    public void KeyboardInputControls()
    {
        Vector4 formattedInput = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        PlayerInputValue = formattedInput.normalized;
    }

    //we need a decoder to unformat the code into a reasonable format
    public static ShipInput InputDecoder(Vector4 input)
    {
        ShipInput newInput = new();
        newInput.JoystickValue = input; //the conversion is implicit
        newInput.ThrustValue = input.w;
        return newInput;
    }

    private float ConvertToSignedRange(float value)
    {
        float convertedNum = (value * 2) - 1;
        return convertedNum;
    }

}

public struct ShipInput
{
    public Vector3 JoystickValue; // x and y are the joystick movement values, the z is the yaw of the joystick
    public float ThrustValue;
}

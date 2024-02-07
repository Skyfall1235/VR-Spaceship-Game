using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class ShipInputHandler : MonoBehaviour
{
    //xrs input coincides with the update method
    public Vector3 PlayerInputValue;//xand y are the joystick inputs, while z is the thrust amount

    //reference to primary controls
    public XRJoystick m_shipJoystick;
    public XRSlider m_shipThrottle;

    public void UpdatePlayerInput()
    {
        if (m_shipJoystick != null && m_shipThrottle != null)
        {
            Vector3 formattedInput = new(m_shipJoystick.value.x, m_shipJoystick.value.y, ConvertToSignedRange(m_shipThrottle.value));
            PlayerInputValue = formattedInput;
        }
        else Debug.LogWarning("joystick or throttle reference is missing");
    }

    private float ConvertToSignedRange(float value)
    {
        float convertedNum = (value * 2) - 1;
        return convertedNum;
    }

}

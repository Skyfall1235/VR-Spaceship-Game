using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BC_ShipInputHandler : MonoBehaviour
{
    /// <summary>
    /// Encodes joystick input into a ShipJoystickInput struct.
    /// </summary>
    /// <param name="primaryInput">Input vector from the primary joystick.</param>
    /// <param name="secondaryInput">Input vector from the secondary joystick.</param>
    /// <param name="breakVal">Value for applying brakes (0-1 range).</param>
    /// <returns>A ShipJoystickInput struct containing the encoded input values.</returns>
    internal ShipJoystickInput InputEncoder(Vector2 primaryInput, Vector2 secondaryInput, float breakVal)
    {
        //create and setup the ship input
        ShipJoystickInput newInput = new ShipJoystickInput(primaryInput, secondaryInput.x, secondaryInput.y, breakVal);
        return newInput;
    }

    /// <summary>
    /// Encodes keyboard input into a ShipJoystickInput struct.
    /// </summary>
    /// <param name="keyboardInput">A tuple containing keyboard movement, rotation, and break values.</param>
    /// <returns>A ShipJoystickInput struct containing the encoded input values.</returns>
    internal ShipJoystickInput InputEncoder((Vector2, Vector2, float) keyboardInput)
    {
        //map values to vector2s and then return the encoded version
        Vector2 rightHand = keyboardInput.Item1;
        Vector2 leftHand = keyboardInput.Item2;
        float breakVal = keyboardInput.Item3;
        return InputEncoder(rightHand, leftHand, breakVal);
    }
}

public struct ShipJoystickInput
{
    /// <summary>
    /// X and Y values of the main joystick for movement.
    /// </summary>
    public Vector2 PrimaryFlightStick;

    /// <summary>
    /// Single value for yaw rotation (left/right turning).
    /// </summary>
    public float yawValue;

    /// <summary>
    /// Value for applying thrust (forward/backward acceleration)
    /// </summary>
    public float ThrustValue;

    /// <summary>
    /// Value for applying brakes (deceleration).
    /// </summary>
    public float BreakValue;

    /// <summary>
    /// Constructor taking individual values for each input.
    /// </summary>
    /// <param name="primaryFlightStick">is a Vector2 that contains a -1 to 1 grid representing the value of the joystick</param>
    /// <param name="yawValue">is the X value of the secondary joystick</param>
    /// <param name="thrustValue">is the y value of the secondary joystick</param>
    /// <param name="breakValue">the trigger value of the secondary thruster</param>
    public ShipJoystickInput(Vector2 primaryFlightStick, float yawValue, float thrustValue, float breakValue)
    {
        this.PrimaryFlightStick = primaryFlightStick;
        this.yawValue = yawValue;
        this.ThrustValue = thrustValue;
        this.BreakValue = breakValue;
    }

    /// <summary>
    /// Constructor taking a single Vector4 containing primary controls, excluding breaking force
    /// </summary>
    /// <param name="totalInput"> is a Vector 4 that packages the primary flight stick, thrust, and yaw values into a single datum</param>
    public ShipJoystickInput(Vector4 totalInput)
    {
        this.PrimaryFlightStick = new(totalInput.x, totalInput.y);
        this.yawValue = totalInput.z;
        this.ThrustValue = totalInput.w;
        this.BreakValue = 0f;
    }
}

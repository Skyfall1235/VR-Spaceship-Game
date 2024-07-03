using UnityEngine;

public class BC_ShipInputHandler : MonoBehaviour
{
    /// <summary>
    /// Encodes raw input values directly into a ShipJoystickInput struct.
    /// </summary>
    /// <param name="primaryInput">The X and Y values for primary movement (likely from a joystick).</param>
    /// <param name="primaryYaw">The yaw rotation value for the primary input (likely Z-axis of a joystick).</param>
    /// <param name="secondaryInput">A Vector3 representing secondary input (if applicable). Assumed to contain X, Y for movement and potentially Z for yaw.</param>
    /// <param name="secondaryYaw">The yaw rotation value for the secondary input (if applicable).</param>
    /// <param name="breakVal">The value for applying brakes (deceleration) to the ship.</param>
    /// <returns>A ShipJoystickInput struct containing the encoded input values.</returns>
    internal ShipJoystickInput InputEncoder(Vector2 primaryInput, float primaryYaw, Vector3 secondaryInput, float secondaryYaw, float breakVal)
    {
        //create and setup the ship input
        ShipJoystickInput newInput = new ShipJoystickInput(primaryInput, primaryYaw, secondaryInput, secondaryYaw, breakVal);
        return newInput;
    }

    /// <summary>
    /// Encodes keyboard input into a ShipJoystickInput struct.
    /// </summary>
    /// <param name="keyboardInput">A tuple containing keyboard data for primary and secondary movement (X, Y), rotation (single value), and break value.</param>
    /// <returns>A ShipJoystickInput struct containing the encoded input values.</returns>
    internal ShipJoystickInput InputEncoder((Vector3, Vector3, float) keyboardInput)
    {
        // Extract values from the keyboard input tuple
        Vector3 primaryHand = keyboardInput.Item1;
        Vector3 secondaryHand = keyboardInput.Item2;
        float breakVal = keyboardInput.Item3;

        // Convert primary and secondary hand data to Vector2 for movement axes
        Vector2 primaryInput = primaryHand;
        Vector2 secondaryInput = secondaryHand;

        // Extract yaw rotation values from Z component of both hands
        float primaryYaw = primaryHand.z;
        float secondaryYaw = secondaryHand.z;

        // Recursively call the InputEncoder method with the extracted values
        return InputEncoder(primaryInput, primaryYaw, secondaryInput, secondaryYaw, breakVal);
    }
}

/// <summary>
/// Represents input data from a joystick for controlling a ship.
/// </summary>
public struct ShipJoystickInput
{
    /// <summary>
    /// X and Y values of the main joystick used for movement.
    /// </summary>
    public Vector2 PrimaryFlightStick;

    /// <summary>
    /// Single value for yaw rotation.
    /// </summary>
    public float PrimaryYawValue;

    /// <summary>
    /// X and Y values of a secondary joystick.
    /// </summary>
    public Vector2 SecondaryFlightStick;

    /// <summary>
    /// Single value for yaw rotation of the secondary joystick.
    /// </summary>
    public float SecondaryYawValue;

    /// <summary>
    /// Value for applying brakes (deceleration) to the ship.
    /// </summary>
    public float BreakValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipJoystickInput"/> struct with the specified primary flight stick values, yaw value, secondary flight stick values, and brake value.
    /// </summary>
    /// <param name="primaryFlightStick">The X and Y values of the main joystick for movement.</param>
    /// <param name="primaryyawValue">The single value for yaw rotation (turning left or right) using the primary joystick.</param>
    /// <param name="secondaryFlightStick">The X and Y values of a secondary joystick (if applicable).</param>
    /// <param name="secondaryyawValue">The single value for yaw rotation (turning left or right) using the secondary joystick (if applicable).</param>
    /// <param name="breakValue">The value for applying brakes (deceleration) to the ship.</param>
    public ShipJoystickInput(Vector2 primaryFlightStick, float primaryyawValue, Vector2 secondaryFlightStick, float secondaryyawValue, float breakValue)
    {
        PrimaryFlightStick = primaryFlightStick;
        PrimaryYawValue = primaryyawValue;
        SecondaryFlightStick = secondaryFlightStick;
        SecondaryYawValue = secondaryyawValue;
        BreakValue = breakValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipJoystickInput"/> struct with the specified primary and secondary flight stick directions as Vector3 (assuming Z is yaw), and brake value.
    /// </summary>
    /// <param name="primaryFlightStick">The X, Y, and Z (yaw) values of the main joystick for movement.</param>
    /// <param name="secondaryFlightStick">The X, Y, and Z (yaw) values of a secondary joystick (if applicable).</param>
    /// <param name="breakValue">The value for applying brakes (deceleration) to the ship.</param>
    public ShipJoystickInput(Vector3 primaryFlightStick, Vector3 secondaryFlightStick, float breakValue)
    {
        PrimaryFlightStick = primaryFlightStick;
        PrimaryYawValue = primaryFlightStick.z;
        SecondaryFlightStick = secondaryFlightStick;
        SecondaryYawValue = secondaryFlightStick.z;
        BreakValue = breakValue;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIForTesters : MonoBehaviour
{
    public Slider[] sliders; //pitch, roll, yaw
    public TextMeshProUGUI[] texts;//same as sliders
    public ShipMovementController controller;
    public NewXRJoystick lJS;
    public NewXRJoystick RJS;

    public void SetPitch()
    {
        texts[0].text = $"Pitch: " + sliders[0].value;
        controller.SetPitchSpeed(sliders[0].value);
    }
    public void SetRoll()
    {
        texts[1].text = $"Roll: " + sliders[1].value;
        controller.SetRollSpeed(sliders[1].value);
    }
    public void SetYaw()
    {
        texts[2].text = $"Yaw: " + sliders[2].value;
        controller.SetYawSpeed(sliders[2].value);
    }
    public void SetStickDeadzone()
    {
        texts[3].text = $"Stick Deadzones: " + sliders[3].value;
        lJS.SetStickDeadzone(sliders[3].value);
        RJS.SetStickDeadzone(sliders[3].value);
    }
    public void SetTwistDeadzone()
    {
        texts[4].text = $"Twist deadzones: " + sliders[4].value;
        lJS.SetTwistDeadzone(sliders[4].value);
        RJS.SetTwistDeadzone(sliders[4].value);
    }
}

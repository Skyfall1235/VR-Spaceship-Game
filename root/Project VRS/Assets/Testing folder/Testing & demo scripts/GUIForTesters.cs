using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIForTesters : MonoBehaviour
{
    public Slider[] sliders; //pitch, roll, yaw
    public TextMeshProUGUI[] texts;//same as sliders
    public ShipMovementController controller;

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
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayValue : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public JoystickWithYaw joystick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        displayText.text = $"JHyostick val - {joystick.JoystickValue} \nknob Val - {joystick.KnobValue}";
    }
}

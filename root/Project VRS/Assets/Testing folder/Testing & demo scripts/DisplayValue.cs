using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class DisplayValue : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public NewXRJoystick rightjoystick;
    public NewXRJoystick leftjoystick;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        displayText.text = $"Right Joystick - {rightjoystick.value}\n" +
                           $"Right Twist - {rightjoystick.value}\n" +
                           $"Left Joystick - {leftjoystick.value}\n" +
                           $"Left Twist - {leftjoystick.value}\n" +
                           $"Velocity - {rb.velocity.magnitude}m/s";
    }
}

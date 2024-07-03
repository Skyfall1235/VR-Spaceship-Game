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
        displayText.text = $"right Joystick val - {rightjoystick.value}\n" +
                           $"right twist val - {rightjoystick.TwistValue}\n" +
                           $"left Joystick val - {leftjoystick.value}\n" +
                           $"left twist val - {leftjoystick.TwistValue}\n" +
                           $"Velocity - {rb.velocity.magnitude}";
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class DisplayValue : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public XRJoystick joystick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        displayText.text = $"{joystick.value}";
    }
}

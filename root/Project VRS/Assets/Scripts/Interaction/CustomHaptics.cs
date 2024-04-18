using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomHaptics : MonoBehaviour
{
    public bool SendHapticsToController;
    public List<XRBaseController> registeredControllers = new List<XRBaseController>();

    public void RegisterInteractor()
    {

    }

    public void DeRegisterInteractor()
    {

    }
}

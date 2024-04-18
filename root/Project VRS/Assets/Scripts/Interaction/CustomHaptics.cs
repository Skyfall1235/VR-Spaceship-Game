using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractable))]
public class CustomHaptics : MonoBehaviour
{
    public bool SendHapticsToController;
    public List<XRBaseController> registeredControllers = new List<XRBaseController>();

    public bool RecieveHover, RecieveSelect, RecieveActivate;
    [Range(0f, 1f)]
    public float HoverHapticIntensity, SelectHapticIntensity, ActivateHapticIntensity;

    //we need to know waht controllers are interacting with this haptic

    private void Awake()
    {
        registeredControllers.Clear();
        XRBaseInteractable associatedInteractable = GetComponent<XRBaseInteractable>();
       
    }

    void SetupInteractionEvents(XRBaseInteractable XRinteractable)
    {
        if(RecieveHover)
        {
            //XRinteractable.hoverEntered.AddListener(HoverHapticIntensity, )
        }
        if (RecieveSelect)
        {
            
        }
        if (RecieveActivate)
        {

        }
    }


    public void RegisterInteractor()
    {

    }

    public void DeRegisterInteractor()
    {

    }

    private void ApplyHapticsToControllers(float intensity, float duration, bool loop)
    {

    }
}

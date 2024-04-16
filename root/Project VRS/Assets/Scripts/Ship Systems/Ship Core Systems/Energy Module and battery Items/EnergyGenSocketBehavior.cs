using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class EnergyGenSocketBehavior : MonoBehaviour
{
    [SerializeField]
    private FuelRodBehavior m_currentlyPluggedFuelRod;
    public FuelRodBehavior CurrentlyPluggedFuelRod
    {
        get => m_currentlyPluggedFuelRod;
    }

    public void RegisterFuelRod(SelectEnterEventArgs args)
    {
        XRBaseInteractable interactable = args.interactableObject  as XRBaseInteractable;
        GameObject interactableGo = interactable.gameObject;
        FuelRodBehavior behavior = interactableGo.GetComponent<FuelRodBehavior>();
        m_currentlyPluggedFuelRod = behavior;
        Debug.Log("registered an item");
        Debug.Log(interactable);
        Debug.Log(interactable);
        Debug.Log(behavior);
    }

    public void DeRegisterFuelRod(SelectExitEventArgs args)
    {
        m_currentlyPluggedFuelRod = null;
        Debug.Log("deregistered an item");
    }

}

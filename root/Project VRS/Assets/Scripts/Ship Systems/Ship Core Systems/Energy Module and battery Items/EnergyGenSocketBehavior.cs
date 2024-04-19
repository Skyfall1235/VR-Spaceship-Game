using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class EnergyGenSocketBehavior : MonoBehaviour
{
    XRSocketInteractor linkedSocket;

    [SerializeField]
    private FuelRodBehavior m_currentlyPluggedFuelRod;
    public FuelRodBehavior CurrentlyPluggedFuelRod
    {
        get => m_currentlyPluggedFuelRod;
    }

    private void Awake()
    {
        linkedSocket = GetComponent<XRSocketInteractor>();
        linkedSocket.selectEntered.AddListener(RegisterFuelRod);
        linkedSocket.selectExited.AddListener(DeRegisterFuelRod);
    }

    public void RegisterFuelRod(SelectEnterEventArgs args)
    {
        XRBaseInteractable interactable = args.interactableObject  as XRBaseInteractable;
        GameObject interactableGo = interactable.gameObject;
        FuelRodBehavior behavior = interactableGo.GetComponent<FuelRodBehavior>();
        m_currentlyPluggedFuelRod = behavior;
        Debug.Log($"registered an item : {interactable}");
    }

    public void DeRegisterFuelRod(SelectExitEventArgs args)
    {
        Debug.Log($"deregistered an item : {m_currentlyPluggedFuelRod}");
        m_currentlyPluggedFuelRod = null;
    }

}

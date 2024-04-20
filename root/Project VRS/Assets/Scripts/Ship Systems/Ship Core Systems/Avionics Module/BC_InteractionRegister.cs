using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BC_InteractionRegister : MonoBehaviour
{

    //this is for registering the interactor, NOT the interactable

    public XRBaseInteractor interactor;
    [SerializeField]
    private XRBaseInteractable associatedInteractable;

    protected void Awake()
    {
        
    }

    protected void RegisterInteractor(BaseInteractionEventArgs arg)
    {

    }

    protected void DeRegisterInteractor(BaseInteractionEventArgs arg)
    {

    }
}

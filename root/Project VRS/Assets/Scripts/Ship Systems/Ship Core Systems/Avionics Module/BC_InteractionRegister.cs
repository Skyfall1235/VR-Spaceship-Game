using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This class manages the registration of XRBaseInteractors with a game object.
/// It provides functionality to register, deregister, and check for the currently registered interactor.
/// </summary>
public class BC_InteractionRegister : MonoBehaviour
{
    //this is for registering the interactor, NOT the interactable
    [SerializeField]
    protected XRBaseInteractor m_registeredInteractor;
    public XRBaseInteractor RegisteredInteractor
    {
        get { return m_registeredInteractor; }
    }
    [SerializeField]
    protected XRBaseInteractable m_associatedInteractable;

    /// <summary>
    /// Registers an XRBaseInteractor with the current object.
    /// </summary>
    /// <param name="args">The arguments associated with the interaction event.</param>
    protected void RegisterInteractor(BaseInteractionEventArgs args)
    {
        m_registeredInteractor = args.interactorObject as XRBaseInteractor;
    }

    /// <summary>
    /// Deregisters the currently registered XRBaseInteractor.
    /// </summary>
    protected void DeRegisterInteractor(BaseInteractionEventArgs args)
    {
        m_registeredInteractor = null;
    }

    public bool HasRegistered()
    {
        if(m_registeredInteractor != null)
        {
            return true;
        }
        return false;
    }
}

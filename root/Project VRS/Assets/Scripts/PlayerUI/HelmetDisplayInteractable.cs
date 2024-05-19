using UnityEngine.XR.Interaction.Toolkit;

public class HelmetDisplayInteractable : BC_InteractionRegister
{
    private void Awake()
    {
        m_associatedInteractable = GetComponent<XRBaseInteractable>();
        m_associatedInteractable.selectEntered.AddListener(RegisterInteractor);
        m_associatedInteractable.selectExited.AddListener(DeRegisterInteractor);
    }
}

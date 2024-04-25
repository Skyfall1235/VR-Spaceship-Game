using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HelmetDisplayInteractable : BC_InteractionRegister
{
    private void Awake()
    {
        m_associatedInteractable = GetComponent<XRBaseInteractable>();
        //RegisterListeners(m_associatedInteractable.selectEntered, m_associatedInteractable.selectExited);
    }
}

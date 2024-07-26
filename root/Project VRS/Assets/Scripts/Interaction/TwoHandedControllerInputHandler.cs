using log4net.Repository.Hierarchy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TwoHandedControllerInputHandler : MonoBehaviour, IinteractorRegister
{
    [Header("Interactables")]

    [SerializeField] private XRBaseInteractable m_primaryInteractable;
    [SerializeField] private XRBaseInteractable m_secondaryInteractable;

    [SerializeField] [HideInInspector] private ActionBasedController m_primaryController;
    [SerializeField] private ValueProperties m_primaryValueProperties;
    public ValueProperties PrimaryValuesProperties
    {
        get => m_primaryValueProperties;
    }

    [SerializeField][HideInInspector] private ActionBasedController m_secondaryController;
    [SerializeField] private ValueProperties m_secondaryValueProperties;
    public ValueProperties SecondaryValuesProperties
    {
        get => m_secondaryValueProperties;
    }
    [Header("Extra")]
    [SerializeField] CustomLogger logger;

    #region Registration of interactors

    public void RegisterPrimaryInteractorController(SelectEnterEventArgs e)
    {
        Debug.Log("registered primary");
        m_primaryController = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
        SetOrRemoveValueProperties(e, true, ref m_primaryController, ref m_primaryValueProperties);
    }

    public void UnregisterPrimaryInteractorController(SelectExitEventArgs e)
    {
        Debug.Log("registered primary");
        m_primaryController = null;
        SetOrRemoveValueProperties(e, false, ref m_primaryController, ref m_primaryValueProperties);
    }

    public void RegisterSecondaryInteractorController(SelectEnterEventArgs e)
    {
        Debug.Log("deregistered secondary");
        m_secondaryController = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
        SetOrRemoveValueProperties(e, true, ref m_secondaryController, ref m_secondaryValueProperties);
    }

    public void UnregisterSecondaryInteractorController(SelectExitEventArgs e)
    {
        Debug.Log("deregistered secondary");
        m_secondaryController = null;
        SetOrRemoveValueProperties(e, false, ref m_secondaryController, ref m_secondaryValueProperties);
    }

    private void Awake()
    {
        m_primaryValueProperties = new ValueProperties(gameObject, logger);
        m_secondaryValueProperties = new ValueProperties(gameObject, logger);

        //primary interactable
        m_primaryInteractable.selectEntered.AddListener(RegisterPrimaryInteractorController);
        m_primaryInteractable.selectExited.AddListener(UnregisterPrimaryInteractorController);
        //secondary interactable
        m_secondaryInteractable.selectEntered.AddListener(RegisterPrimaryInteractorController);
        m_secondaryInteractable.selectExited.AddListener(UnregisterSecondaryInteractorController);
    }

    #endregion

    

    public void SetOrRemoveValueProperties(BaseInteractionEventArgs e, bool setProperties, ref ActionBasedController controller, ref ValueProperties properties)
    {
        //XRBaseController grabbed = e.interactableObject.transform.GetComponent<XRBaseController>();
        //depending on where or not we are setting or removing the properties for the addtional input
        if (setProperties)
        {
            //set the interactor so we can reference it
            controller = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
            AdditionalInputForActionBasedController additionalInput = e.interactorObject.transform.parent.GetComponent<AdditionalInputForActionBasedController>();

            //if the interactor has additional inputs, register them
            if (additionalInput != null)
            {
                properties.SetProperties(additionalInput, ref properties);
            }
            else
            {
                Debug.LogError("additional input is null you dumb bitch");
            }
        }
        else
        {
            properties.RemoveProperties();
        }

    }
}

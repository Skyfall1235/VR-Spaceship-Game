using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

//needs an access point to the interactor through the interactable
[RequireComponent(typeof(XRBaseInteractable))]
public class InteractorToInputExposer : MonoBehaviour
{
    //register the interactor
    [SerializeField] private ActionBasedController m_interactor;

    //list all possible inputs
    //grip
    public bool select
    {
        get
        {
            if (m_interactor != null)
            {
                return RetrieveBoolFromAction(m_interactor.selectAction);
            }
            else
            {
                StateLackOfAction("Select Bool");
                return false;
            }
        }
    }
    public float selectValue
    {
        get
        {
            if (m_interactor != null)
            {
                return RetrieveFloatFromAction(m_interactor.selectActionValue);
            }
            else
            {
                StateLackOfAction("Select Value");
                return 0f;
            }
        }
    }

    //trigger
    public bool activate
    {
        get
        {
            if (m_interactor != null)
            {
                return RetrieveBoolFromAction(m_interactor.activateAction);
            }
            else
            {
                StateLackOfAction("Activate Bool");
                return false;
            }
        }
    }
    public float activateValue
    {
        get
        {
            if (m_interactor != null)
            {
                return RetrieveFloatFromAction(m_interactor.activateActionValue);
            }
            else
            {
                StateLackOfAction("Activate Value");
                return 0f;
            }
        }
    }

    //buttons

    //joystick val
    public Vector2 JoystickVal
    {
        get
        {
            if (m_interactor != null)
            {
                //i had to map the vector2 of the joystick somehow
                return RetrieveVector2FromAction(m_interactor.uiScrollAction);
            }
            else
            {
                StateLackOfAction("Joystick Value");
                return Vector2.zero;
            }
        }
    }

    //gameobject
    public GameObject InteractorGameObject;



    /// <summary>
    /// Registers the action based controller based on what selected the associated item
    /// </summary>
    /// <param name="e">the event args that contain the interactor and interactable associated with this unityevent</param>
    public void RegisterInteractorController(SelectEnterEventArgs e)
    {
        if (m_interactor == null)
        {
            m_interactor = GrabActionBasedController(e.interactorObject);
            InteractorGameObject = m_interactor.gameObject;
        }
    }

    /// <summary>
    ///  Unregisters the  action based controller
    /// </summary>
    public void UnregisterInteractorController()
    {
        if (m_interactor != null)
        {
            m_interactor = null;
            InteractorGameObject = null;
        }
    }

    private float RetrieveFloatFromAction(InputActionProperty actionProperty)
    {
        float actionValue = actionProperty.action.ReadValue<float>();
        return actionValue;
    }

    private bool RetrieveBoolFromAction(InputActionProperty actionProperty)
    {
        bool actionValue = actionProperty.action.ReadValue<bool>();
        return actionValue;
    }

    private Vector2 RetrieveVector2FromAction(InputActionProperty actionProperty)
    {
        Vector2 actionValue = actionProperty.action.ReadValue<Vector2>();
        return actionValue;
    }

    //private T RetrieveValueFromAction<T>(InputActionProperty actionProperty) where T : struct
    //{
    //    T actionValue = actionProperty.action.ReadValue<T>();
    //    return actionValue;
    //}

    private GameObject RetrieveGameObjectFromInteractor(IXRInteractor interactor)
    {
        GameObject interactorGO = interactor.transform.gameObject;
        return interactorGO;
    }

    private void StateLackOfAction(string nameOfAction)
    {
        Debug.LogWarning($"A method is trying to access the {nameOfAction} variable while the interactor is not registered.");
    }

    /// <summary>
    /// returns the action based controller from an interactor if it has one
    /// </summary>
    /// <param name="interactor">the interactor we want to search</param>
    /// <returns>The action based controller from an interactor if it has one</returns>
    private ActionBasedController GrabActionBasedController(IXRInteractor interactor)
    {
        GameObject interactorGO = RetrieveGameObjectFromInteractor(interactor);
        ActionBasedController ABController = interactorGO.GetComponent<ActionBasedController>();
        //if the action based controller is null, then no interactor will be saved
        return ABController;
    }

}

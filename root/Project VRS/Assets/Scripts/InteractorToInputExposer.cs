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
                return RetrieveValueFromAction<bool>(m_interactor.selectAction);
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
                return RetrieveValueFromAction<float>(m_interactor.selectActionValue);
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
                return RetrieveValueFromAction<bool>(m_interactor.activateAction);
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
                return RetrieveValueFromAction<float>(m_interactor.activateActionValue);
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
                return RetrieveValueFromAction<Vector2>(m_interactor.uiScrollAction);
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

    /// <summary>
    /// Retrieves a value of type `T` from a Unity `InputActionProperty`.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve. Must be a struct.</typeparam>
    /// <param name="actionProperty">The `InputActionProperty` object containing the value.</param>
    /// <returns>The value of type `T` retrieved from the `InputActionProperty`.</returns>
    private T RetrieveValueFromAction<T>(InputActionProperty actionProperty) where T : struct
    {
        // retrive the value from the action map
        T actionValue = actionProperty.action.ReadValue<T>();
        //return the value
        return actionValue;
    }

    /// <summary>
    /// Sometimes, we need the gameobject from the interactor
    /// </summary>
    /// <param name="interactor">is the interactor that is currently registered</param>
    /// <returns>the Game object currently interacting with the associated Interactable</returns>
    static private GameObject RetrieveGameObjectFromInteractor(IXRInteractor interactor)
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
    static public ActionBasedController GrabActionBasedController(IXRInteractor interactor)
    {
        GameObject interactorGO = RetrieveGameObjectFromInteractor(interactor);
        ActionBasedController ABController = interactorGO.GetComponent<ActionBasedController>();
        //if the action based controller is null, then no interactor will be saved
        return ABController;
    }

}

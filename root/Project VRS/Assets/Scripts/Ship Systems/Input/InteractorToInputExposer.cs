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

    #region Select
    //grip
    [Header("Select Action and Value")]
    [SerializeField]
    private bool m_select;
    public bool Select
    {
        get
        {
            if (m_interactor != null)
            {
                m_select = RetrieveValueFromAction<bool>(m_interactor.selectAction);
                return m_select;
            }
            else
            {
                StateLackOfAction("Select Bool");
                m_select = false;
                return false;
            }
        }
    }
    [SerializeField]
    private float m_selectValue;
    public float SelectValue
    {
        get
        {
            if (m_interactor != null)
            {
                m_selectValue = RetrieveValueFromAction<float>(m_interactor.selectActionValue);
                return m_selectValue;
            }
            else
            {
                StateLackOfAction("Select Value");
                m_selectValue = 0f;
                return 0f;
            }
        }
    }

    #endregion

    #region Activate

    [Header("Activate Action and Value")]
    //trigger
    [SerializeField]
    private bool m_activate;
    public bool Activate
    {
        get
        {
            if (m_interactor != null)
            {
                m_activate = RetrieveValueFromAction<bool>(m_interactor.activateAction);
                return m_activate;
            }
            else
            {
                StateLackOfAction("Activate Bool");
                m_activate = false;
                return false;
            }
        }
    }
    [SerializeField]
    private float m_activateValue;
    public float ActivateValue
    {
        get
        {
            if (m_interactor != null)
            {
                m_activateValue = RetrieveValueFromAction<float>(m_interactor.activateActionValue);
                return m_activateValue;
            }
            else
            {
                StateLackOfAction("Activate Value");
                m_activateValue = 0f;
                return 0f;
            }
        }
    }

    #endregion

    #region buttons and joystick

    [Header("Button and Joystick Action and Value")]
    //buttons?
    [SerializeField]
    private bool m_primaryButton;
    public bool PrimaryButton
    {
        get
        {
            if(m_interactor != null)
            {
                //return RetrieveValueFromAction<bool>(m_interactor.)
                return false;
            }
            m_primaryButton = false;
            return false;
        }
    }

    //joystick val

    [SerializeField]
    private bool m_joystickClick;
    public bool JoystickClick
    {
        get
        {
            if (m_interactor != null)
            {
                //m_joystickClick = RetrieveValueFromAction<bool>(m_interactor.)
                return false;
            }
            else
            {
                StateLackOfAction("Joystick Bool");
                m_joystickClick = false;
                return m_joystickClick;
            }
        }
    }

    [SerializeField]
    private Vector2 m_joystickVal;
    public Vector2 JoystickVal
    {
        get
        {
            if (m_interactor != null)
            {
                //i had to map the vector2 of the joystick somehow
                m_joystickVal = RetrieveValueFromAction<Vector2>(m_interactor.uiScrollAction);
                return m_joystickVal;
            }
            else
            {
                StateLackOfAction("Joystick Value");
                m_joystickVal = Vector2.zero;
                return Vector2.zero;
            }
        }
    }

    #endregion


    [Header("Interactor GameObject")]
    //gameobject
    [SerializeField]
    private GameObject m_interactorGameObject;

    public GameObject InteractorGameObject
    {
        get
        {
            return m_interactorGameObject;
        }
    }    

    #region Interactor Registration

    /// <summary>
    /// Registers the action based controller based on what selected the associated item
    /// </summary>
    /// <param name="e">the event args that contain the interactor and interactable associated with this unityevent</param>
    public void RegisterInteractorController(SelectEnterEventArgs e)
    {
        if (m_interactor == null)
        {
            m_interactor = GrabActionBasedController(e.interactorObject);
            m_interactorGameObject = m_interactor.gameObject;
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
            m_interactorGameObject = null;
        }
    }

    #endregion

    #region State retrival and warnings

    /// <summary>
    /// Retrieves a value of type T from a Unity InputActionProperty.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve. Must be a struct.</typeparam>
    /// <param name="actionProperty">The InputActionProperty object containing the value.</param>
    /// <returns>The value of type T retrieved from the InputActionProperty.</returns>
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

    static public AdditionalInputForActionBasedController GrabOptionalAdditionalInputfromController(IXRInteractor interactor)
    {
        GameObject interactorGO = RetrieveGameObjectFromInteractor(interactor);
        AdditionalInputForActionBasedController AdditionalInputController = interactorGO.GetComponent<AdditionalInputForActionBasedController>();
        //if the action based controller is null, then no interactor will be saved
        return AdditionalInputController;
    }

    #endregion

}

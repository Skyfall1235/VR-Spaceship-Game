using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerShipInputHandler : BC_ShipInputHandler, IinteractorRegister
{
    #region Joysticks

    //reference to primary controls
    [SerializeField] private NewXRJoystick m_primaryShipJoystick;
    public NewXRJoystick PrimaryShipJoystick => m_primaryShipJoystick;

    [SerializeField] private NewXRJoystick m_secondaryShipJoystick;
    public NewXRJoystick SecondaryShipJoystick => m_secondaryShipJoystick;

    [SerializeField] private ActionBasedController m_primaryJoystickInteractor;
    [SerializeField] private ActionBasedController m_secondaryJoystickInteractor;


    public UnityEvent InputRecieval = new UnityEvent();

    public bool useKeyboardControls = true;

    public ShipJoystickInput CurrentShipJoystickInputs
    {
        get
        {
            //if the controls are to be unresponsive, dont proceed
            if (!ControlsAreResponsive)
            {
                return new();
            }
            // Check for missing references and log a single, comprehensive message
            if (m_primaryShipJoystick == null || m_secondaryShipJoystick == null && !useKeyboardControls)
            {
                Debug.LogWarning("Either primary ship joystick or secondary ship joystick reference is missing.");
                return new();  // Return an empty struct to indicate an error
            }

            if (useKeyboardControls)
            {
                return InputEncoder(KeyboardInputControls());
            }
            //this can stay the same because we

            // There are only 2 possible reasons for why this should not be true
            // 1. Update on End select requests this input handlers value
            // 2. if the register interactor Controller event somehow gets called first.
            //therefore LEAVE THE DAMN RETURN AT THE BOTTOM ALONE
            if (m_secondaryJoystickInteractor != null)
            {
                //get the input action property and store its value
                InputActionProperty activateValueProperty = m_secondaryJoystickInteractor.activateActionValue;
                float activateValue = activateValueProperty.action.ReadValue<float>();

                //return the encoded joystick inputs with a break value if there is any
                InputRecieval.Invoke();
                return InputEncoder(m_primaryShipJoystick.value, m_primaryShipJoystick.TwistValue, m_secondaryShipJoystick.value, m_secondaryShipJoystick.TwistValue, activateValue);
            }
            Debug.Log("are we getting here?");
            InputRecieval.Invoke();
            return InputEncoder(m_primaryShipJoystick.value, m_primaryShipJoystick.TwistValue, m_secondaryShipJoystick.value, m_secondaryShipJoystick.TwistValue, 0);
        }
    }

    [SerializeField] CustomLogger logger;

    private void Awake()
    {
        //primary joystick
        m_primaryShipJoystick.selectEntered.AddListener(RegisterPrimaryInteractorController);
        m_primaryShipJoystick.selectExited.AddListener(UnregisterPrimaryInteractorController);
        //secondary joystick
        m_secondaryShipJoystick.selectEntered.AddListener(RegisterPrimaryInteractorController);
        m_secondaryShipJoystick.selectExited.AddListener(UnregisterSecondaryInteractorController);
    }

    #endregion

    public void RegisterPrimaryInteractorController(SelectEnterEventArgs e)
    {
        m_primaryJoystickInteractor = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
    }

    public void UnregisterPrimaryInteractorController(SelectExitEventArgs e)
    {
        m_primaryJoystickInteractor = null;
    }

    public void RegisterSecondaryInteractorController(SelectEnterEventArgs e)
    {
        m_secondaryJoystickInteractor = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
    }

    public void UnregisterSecondaryInteractorController(SelectExitEventArgs e)
    {
        m_secondaryJoystickInteractor = null;
    }

    #region Access Controls

    private (Vector3, Vector3, float) KeyboardInputControls()
    {
        Vector2 rightHand = Vector2.zero;//new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 
        Debug.Log(rightHand);
        Vector2 leftHand = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        Debug.Log(leftHand);
        float breakVal = Input.GetAxis("Jump");
        return (rightHand, leftHand, breakVal);
    }

    #endregion
}

public interface IinteractorRegister
{
    public void RegisterPrimaryInteractorController(SelectEnterEventArgs e);

    public void UnregisterPrimaryInteractorController(SelectExitEventArgs e);

    public void RegisterSecondaryInteractorController(SelectEnterEventArgs e);

    public void UnregisterSecondaryInteractorController(SelectExitEventArgs e);
}


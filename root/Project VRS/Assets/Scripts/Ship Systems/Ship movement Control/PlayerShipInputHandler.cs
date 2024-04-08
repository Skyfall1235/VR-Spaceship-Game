using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerShipInputHandler : BC_ShipInputHandler
{
    #region Variables

    //reference to primary controls
    [SerializeField] private XRJoystick m_primaryShipJoystick;
    public XRJoystick PrimaryShipJoystick
    {
        get => m_primaryShipJoystick;
    }

    [SerializeField] private XRJoystick m_secondaryShipJoystick;
    public XRJoystick SecondaryShipJoystick
    {
        get => m_secondaryShipJoystick;
    }

    [SerializeField] private ActionBasedController m_primaryJoystickInteractor;
    [SerializeField] private ActionBasedController m_secondaryJoystickInteractor;

    //TEMP
    public bool useKeyboardControls = true;

    [SerializeField] Logger logger;

    #endregion

    public ShipJoystickInput CurrentShipJoystickInputs
    {
        get
        {
            // Check for missing references and log a single, comprehensive message
            if (m_primaryShipJoystick == null || m_secondaryShipJoystick == null)
            {
                Debug.LogWarning("Either primary ship joystick or secondary ship joystick reference is missing.");
                return new();  // Return an empty struct to indicate an error
            }

            // There are only 2 possible reasons for why this should not be true
            // 1. Update on End select requests this input handlers value
            // 2. if the register interactor Controller event somehow gets called first.
            //therefore LEAVE THE DAMN RETURN AT THE BOTTOM ALONE
            if(m_secondaryJoystickInteractor != null)
            {
                //get the input action property and store its value
                InputActionProperty activateValueProperty = m_secondaryJoystickInteractor.activateActionValue;
                float activateValue = activateValueProperty.action.ReadValue<float>();

                //return the encoded joystick inputs with a break value if there is any
                return InputEncoder(m_primaryShipJoystick.value, m_secondaryShipJoystick.value, activateValue);
            }

            return InputEncoder(m_primaryShipJoystick.value, m_secondaryShipJoystick.value, 0);
        }
    }

    /// <summary>
    /// Registers the secondary joysticks action based controller based on what selected the joystick.
    /// this method does use <seealso cref="InteractorToInputExposer"/>'s <seealso cref="InteractorToInputExposer.GrabActionBasedController"/> to get the action map from the interactor.
    /// </summary>
    /// <param name="e">the event args that contain the interactor and interactable associated with this unityevent</param>
    /// 
    public void RegisterInteractorController(SelectEnterEventArgs e)
    {
        if(m_secondaryJoystickInteractor == null)
        {
            m_secondaryJoystickInteractor = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
        }
    }

    /// <summary>
    ///  Unregisters the secondary joysticks action based controller from what unselected the joystick
    /// </summary>
    public void UnregisterInteractorController()
    {
        if (m_secondaryJoystickInteractor != null)
        {
            m_secondaryJoystickInteractor = null;
        }
    }

    private (Vector2, Vector2, float) KeyboardInputControls()
    {
        Vector2 rightHand = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        Vector2 leftHand = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float breakVal = Input.GetAxis("Jump");
        return (rightHand, leftHand, breakVal);
    }
}

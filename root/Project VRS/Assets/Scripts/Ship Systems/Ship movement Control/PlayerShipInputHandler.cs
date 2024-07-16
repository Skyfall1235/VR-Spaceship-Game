using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerShipInputHandler : BC_ShipInputHandler
{
    #region Variables

    //reference to primary controls
    [SerializeField] private NewXRJoystick m_primaryShipJoystick;
    public NewXRJoystick PrimaryShipJoystick => m_primaryShipJoystick;


    [SerializeField] private NewXRJoystick m_secondaryShipJoystick;
    public NewXRJoystick SecondaryShipJoystick => m_secondaryShipJoystick;

    [SerializeField] private ActionBasedController m_primaryJoystickInteractor;
    [SerializeField] private ValueProperties m_primaryValueProperties;
    [SerializeField] private ActionBasedController m_secondaryJoystickInteractor;
    [SerializeField] private ValueProperties m_secondaryValueProperties;

    //TEMP
    public bool useKeyboardControls = true;

    [SerializeField] CustomLogger logger;

    #endregion

    private void Awake()
    {
        m_primaryValueProperties = new ValueProperties(gameObject, logger);
        m_secondaryValueProperties = new ValueProperties(gameObject, logger);
    }

    public ShipJoystickInput CurrentShipJoystickInputs
    {
        get
        {
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
                return InputEncoder(m_primaryShipJoystick.value, m_primaryShipJoystick.TwistValue, m_secondaryShipJoystick.value, m_secondaryShipJoystick.TwistValue, activateValue);
            }
            Debug.Log("are we getting here?");
            return InputEncoder(m_primaryShipJoystick.value, m_primaryShipJoystick.TwistValue, m_secondaryShipJoystick.value, m_secondaryShipJoystick.TwistValue, 0);
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
        SetOrRemoveValueProperties(e, true);
    }

    /// <summary>
    ///  Unregisters the secondary joysticks action based controller from what unselected the joystick
    /// </summary>
    public void UnregisterInteractorController(SelectExitEventArgs e)
    {
        SetOrRemoveValueProperties(e, false);
    }

    public void SetOrRemoveValueProperties(BaseInteractionEventArgs e, bool setProperties)
    {
        //this method is called when either joystick gets grabbed
        //Debug.Log(e.interactableObject.ToString());
        //depending on the ordinal, set the interactor as needed.
        NewXRJoystick grabbedJoystick = e.interactableObject.transform.GetComponent<NewXRJoystick>();
        //depending on where or not we are setting or removing the properties for the addtional input
        switch(setProperties)
        {
            //if we are setting the properties, we need to know waht joystick is being interacted with
            case true:
                switch (grabbedJoystick.Ordinal)
                {
                    case NewXRJoystick.JoystickOrdinal.Primary:

                        //set the interactor so we can reference it
                        m_primaryJoystickInteractor = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
                        AdditionalInputForActionBasedController additionalInput = e.interactorObject.transform.GetComponent<AdditionalInputForActionBasedController>();

                        //if the interactor has additional inputs, register them
                        if (additionalInput != null)
                        {
                            m_primaryValueProperties.SetProperties(additionalInput, ref m_primaryValueProperties);
                        }
                        break;

                    case NewXRJoystick.JoystickOrdinal.Secondary:

                        //set the interactor so we can reference it
                        m_secondaryJoystickInteractor = InteractorToInputExposer.GrabActionBasedController(e.interactorObject);
                        AdditionalInputForActionBasedController additionalInputSecondary = e.interactorObject.transform.GetComponent<AdditionalInputForActionBasedController>();

                        //if the interactor has additional inputs, register them
                        if (additionalInputSecondary != null)
                        {
                            m_secondaryValueProperties.SetProperties(additionalInputSecondary, ref m_secondaryValueProperties);
                        }
                        break;
                }
                break;
            //in the case of removal, just remove based on the joystick.
            case false:
                switch (grabbedJoystick.Ordinal)
                {
                    case NewXRJoystick.JoystickOrdinal.Primary:
                        m_primaryValueProperties.RemoveProperties();
                        break;
                    case NewXRJoystick.JoystickOrdinal.Secondary:
                        m_secondaryValueProperties.RemoveProperties();
                        break;
                }
            break;
        }
    }

    private (Vector3, Vector3, float) KeyboardInputControls()
    {
        Vector2 rightHand = Vector2.zero;//new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); 
        Debug.Log(rightHand);
        Vector2 leftHand = new(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
        Debug.Log(leftHand);
        float breakVal = Input.GetAxis("Jump");
        return (rightHand, leftHand, breakVal);
    }

}

/// <summary>
/// Represents properties for accessing input action values.
/// </summary>
[System.Serializable]
public class ValueProperties
{
    private CustomLogger logger;
    private GameObject scriptOwner;

    #region Input action properties

    /// <summary>
    /// Private property for storing the trigger press action.
    /// </summary>
    private InputActionProperty? m_triggerPressProperty;

    /// <summary>
    /// Private property for storing the trigger value action.
    /// </summary>
    private InputActionProperty? m_triggerValueProperty;

    /// <summary>
    /// Private property for storing the primary button press action.
    /// </summary>
    private InputActionProperty? m_primaryButtonPressProperty;

    /// <summary>
    /// Private property for storing the joystick press action (assumed to be left/right press, not touchpad).
    /// </summary>
    private InputActionProperty? m_joystickPressProperty;

    /// <summary>
    /// Private property for storing the joystick value action (assumed to provide X and Y values).
    /// </summary>
    private InputActionProperty? m_joystickValueProperty;

    #endregion

    /// <summary>
    /// Gets a value indicating whether the trigger button is currently pressed.
    /// </summary>
    /// <remarks>
    /// This property retrieves the value from the internally stored `m_triggerPressProperty` using the `InteractorToInputExposer.RetrieveValueFromAction` method.
    /// </remarks>
    public bool TriggerPressed { get => InteractorToInputExposer.RetrieveValueFromAction<bool>(m_triggerPressProperty != null ? (InputActionProperty)m_triggerValueProperty : throw ThrowErrorWithLog("TriggerPressed")); }

    /// <summary>
    /// Gets the current value of the trigger axis (usually a value between 0.0 and 1.0).
    /// </summary>
    /// <remarks>
    /// This property retrieves the value from the internally stored `m_triggerValueProperty` using the `InteractorToInputExposer.RetrieveValueFromAction` method.
    /// </remarks>
    public float TriggerValue { get => InteractorToInputExposer.RetrieveValueFromAction<float>(m_triggerValueProperty != null ? (InputActionProperty)m_triggerValueProperty : throw ThrowErrorWithLog("TriggerValue")); }

    /// <summary>
    /// Gets a value indicating whether the primary button is currently pressed.
    /// </summary>
    /// <remarks>
    /// This property retrieves the value from the internally stored `m_primaryButtonPressProperty` using the `InteractorToInputExposer.RetrieveValueFromAction` method.
    /// </remarks>
    public bool PrimaryButtonPressed { get => InteractorToInputExposer.RetrieveValueFromAction<bool>(m_primaryButtonPressProperty != null ? (InputActionProperty)m_primaryButtonPressProperty : throw ThrowErrorWithLog("PrimaryButtonPressed")); }

    /// <summary>
    /// Gets a value indicating whether the joystick is currently pressed, does not care about where on the touchpad.
    /// </summary>
    /// <remarks>
    /// This property retrieves the value from the internally stored `m_joystickPressProperty` using the `InteractorToInputExposer.RetrieveValueFromAction` method.
    /// </remarks>
    public bool TouchpadPressed { get => InteractorToInputExposer.RetrieveValueFromAction<bool>(m_joystickPressProperty != null ? (InputActionProperty)m_joystickPressProperty : throw ThrowErrorWithLog("TouchpadPressed")); }

    /// <summary>
    /// Gets the current X and Y values of the joystick.
    /// </summary>
    /// <remarks>
    /// This property retrieves the value from the internally stored `m_joystickValueProperty` using the `InteractorToInputExposer.RetrieveValueFromAction` method.
    /// </remarks>
    public Vector2 TouchpadValue { get => InteractorToInputExposer.RetrieveValueFromAction<Vector2>(m_joystickValueProperty != null ? (InputActionProperty)m_joystickValueProperty : throw ThrowErrorWithLog("TouchpadValue")); }

    #region public methods

    /// <summary>
    /// Sets the properties of a <see cref="ValueProperties"/> instance using the provided additional input configuration for an action-based controller.
    /// </summary>
    /// <param name="additionalInput">An object containing additional input configuration for an action-based controller.</param>
    /// <param name="valueProperties">A reference to the <see cref="ValueProperties"/> instance whose properties will be set.</param>
    public void SetProperties(AdditionalInputForActionBasedController additionalInput, ref ValueProperties valueProperties)
    {
        valueProperties.SetProperties(additionalInput.activateAction,
                                      additionalInput.activateActionValue,
                                      additionalInput.primaryButtonAction,
                                      additionalInput.uiPressAction,
                                      additionalInput.directionalAnchorRotationAction);
    }

    /// <summary>
    /// Sets the properties used to retrieve input action values.
    /// </summary>
    /// <param name="TriggerPressProperty">The property for retrieving the trigger press state.</param>
    /// <param name="TriggerValueProperty">The property for retrieving the trigger value.</param>
    /// <param name="PrimaryButtonPressProperty">The property for retrieving the primary button press state.</param>
    /// <param name="JoystickPressProperty">The property for retrieving the joystick press state (assumed to be left/right press).</param>
    /// <param name="JoystickValueProperty">The property for retrieving the X and Y values of the joystick.</param>
    public void SetProperties(InputActionProperty TriggerPressProperty,
                              InputActionProperty TriggerValueProperty,
                              InputActionProperty PrimaryButtonPressProperty,
                              InputActionProperty JoystickPressProperty,
                              InputActionProperty JoystickValueProperty)
    {
        m_triggerPressProperty = TriggerPressProperty;
        m_triggerValueProperty = TriggerValueProperty;
        m_primaryButtonPressProperty = PrimaryButtonPressProperty;
        m_joystickPressProperty = JoystickPressProperty;
        m_joystickValueProperty = JoystickValueProperty;
    }

    /// <summary>
    /// Clears the properties used to retrieve input action values.
    /// </summary>
    public void RemoveProperties()
    {
        m_triggerPressProperty = null;
        m_triggerValueProperty = null;
        m_primaryButtonPressProperty = null;
        m_joystickPressProperty = null;
        m_joystickValueProperty = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueProperties"/> class with the specified owner and optional input action properties.
    /// </summary>
    /// <param name="Owner">The GameObject that owns this instance.</param>
    /// <param name="triggerPressProperty">The property for retrieving the trigger press state (nullable).</param>
    /// <param name="triggerValueProperty">The property for retrieving the trigger value (nullable).</param>
    /// <param name="primaryButtonPressProperty">The property for retrieving the primary button press state (nullable).</param>
    /// <param name="joystickPressProperty">The property for retrieving the joystick press state (nullable, assumed left/right press).</param>
    /// <param name="joystickValueProperty">The property for retrieving the X and Y values of the joystick (nullable).</param>
    public ValueProperties(GameObject Owner,
                           CustomLogger logger,
                           InputActionProperty? triggerPressProperty = null,
                           InputActionProperty? triggerValueProperty = null,
                           InputActionProperty? primaryButtonPressProperty = null,
                           InputActionProperty? joystickPressProperty = null,
                           InputActionProperty? joystickValueProperty = null)
    {
        this.scriptOwner = Owner;
        this.logger = logger;
        this.m_triggerPressProperty = triggerPressProperty;
        this.m_triggerValueProperty = triggerValueProperty;
        this.m_primaryButtonPressProperty = primaryButtonPressProperty;
        this.m_joystickPressProperty = joystickPressProperty;
        this.m_joystickValueProperty = joystickValueProperty;
    }

    /// <summary>
    /// Throws a new System.Exception with a log message about a failed retrieval of a value.
    /// </summary>
    /// <param name="actionPropertyName">The name of the action property that failed retrieval.</param>
    /// <returns>A new System.Exception object.</returns>
    public System.Exception ThrowErrorWithLog(string actionPropertyName)
    {
        logger.Log($"Failed to retrieve trigger press value from {actionPropertyName}", CustomLogger.LogLevel.Error, CustomLogger.LogCategory.Player, this.scriptOwner);
        return new System.Exception();
    }

    #endregion
}

[System.Serializable]
public enum InteractorObject
{
    Head, LeftHand, RightHand
}

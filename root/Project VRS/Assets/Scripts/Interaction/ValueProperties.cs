using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Represents properties for accessing input action values.
/// </summary>
[System.Serializable]
public class ValueProperties
{
    private CustomLogger logger;
    private GameObject scriptOwner;
    public bool HasValues = false;

    #region Input action properties

    /// <summary>
    /// Private property for storing the trigger press action.
    /// </summary>
    [SerializeField] 
    private InputActionProperty? m_triggerPressProperty;

    public InputActionProperty? TriggerPressProperty => m_triggerPressProperty;

    /// <summary>
    /// Private property for storing the trigger value action.
    /// </summary>
    [SerializeField]
    private InputActionProperty? m_triggerValueProperty;

    public InputActionProperty? TriggerValueProperty => m_triggerValueProperty;

    /// <summary>
    /// Private property for storing the primary button press action.
    /// </summary>
    [SerializeField]
    private InputActionProperty? m_primaryButtonPressProperty;

    public InputActionProperty? PrimaryButtonPressProperty => m_primaryButtonPressProperty;

    /// <summary>
    /// Private property for storing the joystick press action (assumed to be left/right press, not touchpad).
    /// </summary>
    [SerializeField]
    private InputActionProperty? m_joystickPressProperty;

    public InputActionProperty? JoystickPressProperty => m_joystickPressProperty;

    /// <summary>
    /// Private property for storing the joystick value action (assumed to provide X and Y values).
    /// </summary>
    [SerializeField]
    private InputActionProperty? m_joystickValueProperty;

    public InputActionProperty? JoystickValueProperty => m_joystickValueProperty;

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
        Debug.Log("1st set property chain");
        valueProperties.SetProperties(additionalInput.activateAction,
                                      additionalInput.activateActionValue,
                                      additionalInput.PrimaryButtonAction,
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
        Debug.Log("2nd set property chain");
        HasValues = true;
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
        Debug.Log("remove properties");
        HasValues = false;
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

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

 public partial class AdditionalInputForActionBasedController : XRBaseController
{
    /// <inheritdoc />
    protected override void OnEnable()
    {
        m_PrimaryButtonAction.EnableDirectAction();
    }

    /// <inheritdoc />
    protected override void OnDisable()
    {
        m_PrimaryButtonAction.DisableDirectAction();
    }

    [SerializeField]
    InputActionProperty m_PrimaryButtonAction = new InputActionProperty(new InputAction("Primary Button", type: InputActionType.Button));
    public InputActionProperty primaryButtonAction
    {
        get => m_PrimaryButtonAction;
        set => SetInputActionProperty(ref m_PrimaryButtonAction, value);
    }

    /// <inheritdoc />
    protected override void UpdateInput(XRControllerState controllerState)
    {
        base.UpdateInput(controllerState);
        if (controllerState == null)
            return;

        controllerState.ResetFrameDependentStates();

        //primary button action
        var primaryButtonAction = m_PrimaryButtonAction.action;
        if (primaryButtonAction == null || primaryButtonAction.bindings.Count <= 0)
            primaryButtonAction = m_PrimaryButtonAction.action;
        controllerState.uiPressInteractionState.SetFrameState(IsPressed(m_PrimaryButtonAction.action), ReadValue(primaryButtonAction));
    }

    #region Access to actions and Modifications

    void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
    {
        if (Application.isPlaying)
            property.DisableDirectAction();

        property = value;

        if (Application.isPlaying && isActiveAndEnabled)
            property.EnableDirectAction();
    }

    /// <summary>
    /// Reads and returns the given action value.
    /// Unity automatically calls this method during <see cref="UpdateInput"/> to determine
    /// the amount or strength of the interaction state this frame.
    /// </summary>
    /// <param name="action">The action to read the value from.</param>
    /// <returns>Returns the action value. If the action is <see langword="null"/> returns the default <see langword="float"/> value (<c>0f</c>).</returns>
    /// <seealso cref="InteractionState.value"/>
    protected virtual float ReadValue(InputAction action)
    {
        if (action == null)
            return default;

        if (action.activeControl is AxisControl)
            return action.ReadValue<float>();

        if (action.activeControl is Vector2Control)
            return action.ReadValue<Vector2>().magnitude;

        return IsPressed(action) ? 1f : 0f;
    }

    /// <summary>
    /// Evaluates whether the given input action is considered performed.
    /// Unity automatically calls this method during <see cref="UpdateInput"/> to determine
    /// if the interaction state is active this frame.
    /// </summary>
    /// <param name="action">The input action to check.</param>
    /// <returns>Returns <see langword="true"/> when the input action is considered performed. Otherwise, returns <see langword="false"/>.</returns>
    /// <remarks>
    /// More accurately, this evaluates whether the action with a button-like interaction is performed.
    /// Depending on the interaction of the input action, the control driving the value of the input action
    /// may technically be pressed and though the interaction may be in progress, it may not yet be performed,
    /// such as for a Hold interaction. In that example, this method returns <see langword="false"/>.
    /// </remarks>
    /// <seealso cref="InteractionState.active"/>
    protected virtual bool IsPressed(InputAction action)
    {
        if (action == null)
            return false;

#if INPUT_SYSTEM_1_1_OR_NEWER || INPUT_SYSTEM_1_1_PREVIEW // 1.1.0-preview.2 or newer, including pre-release
                return action.phase == InputActionPhase.Performed;
#else
        if (action.activeControl is ButtonControl buttonControl)
            return buttonControl.isPressed;

        return action.triggered || action.phase == InputActionPhase.Performed;
#endif
    }

    static bool IsDisabledReferenceAction(InputActionProperty property) =>
            property.reference != null && property.reference.action != null && !property.reference.action.enabled;

    #endregion
}

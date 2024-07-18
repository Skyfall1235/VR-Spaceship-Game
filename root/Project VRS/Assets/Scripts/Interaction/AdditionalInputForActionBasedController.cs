using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class AdditionalInputForActionBasedController : ActionBasedController
{
    /// <inheritdoc />
    protected override void OnEnable()
    {
        m_primaryButtonAction.EnableDirectAction();
    }

    /// <inheritdoc />
    protected override void OnDisable()
    {
        m_primaryButtonAction.DisableDirectAction();
    }

    [SerializeField]
    InputActionProperty m_primaryButtonAction = new InputActionProperty(new InputAction("Primary Button", type: InputActionType.Button));
    public InputActionProperty PrimaryButtonAction
    {
        get => m_primaryButtonAction;
        set => SetInputActionProperty(ref m_primaryButtonAction, value);
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

    #endregion
}

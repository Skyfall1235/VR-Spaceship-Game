using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

 public partial class AdditionalInputForActionBasedController : ActionBasedController
{
    /// <inheritdoc />
    protected override void OnEnable()
    {
        base.OnEnable();
        m_PrimaryButtonAction.EnableDirectAction();
    }

    /// <inheritdoc />
    protected override void OnDisable()
    {
        base.OnDisable();
        m_PrimaryButtonAction.DisableDirectAction();
    }

    [SerializeField]
    InputActionProperty m_PrimaryButtonAction = new InputActionProperty(new InputAction("Primary Button", type: InputActionType.Button));
    public InputActionProperty primaryButtonAction
    {
        get => m_PrimaryButtonAction;
        set => SetInputActionProperty(ref m_PrimaryButtonAction, value);
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

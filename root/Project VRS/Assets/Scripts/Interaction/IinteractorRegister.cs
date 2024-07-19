using UnityEngine.XR.Interaction.Toolkit;

public interface IinteractorRegister
{
    public void RegisterPrimaryInteractorController(SelectEnterEventArgs e);

    public void UnregisterPrimaryInteractorController(SelectExitEventArgs e);

    public void RegisterSecondaryInteractorController(SelectEnterEventArgs e);

    public void UnregisterSecondaryInteractorController(SelectExitEventArgs e);
}

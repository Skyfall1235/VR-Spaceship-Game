using UnityEngine;
using UnityEngine.Events;

public class ControllerInputManager : MonoBehaviour, IControllerActionValues
{
    public void SetEvents(UnityEvent<ActionValues?> primary, UnityEvent<ActionValues?> secondary)
    {
        primary.AddListener(PrimaryUpdate);
        secondary.AddListener(SecondaryUpdate);
    }

    public void RemoveEvents(UnityEvent<ActionValues?> primary, UnityEvent<ActionValues?> secondary)
    {
        primary.RemoveListener(PrimaryUpdate);
        secondary.RemoveListener(SecondaryUpdate);
    }

    public virtual void PrimaryUpdate(ActionValues? primary)
    {

    }

    public virtual void SecondaryUpdate(ActionValues? secondary)
    {

    }
}

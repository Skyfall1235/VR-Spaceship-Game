using UnityEngine.Events;

/// <summary>
/// Interface that defines properties and methods related to healing
/// </summary>
public interface IHealable
{
    /// <summary>
    /// This property exposes a `Health.OnHealthChanged` delegate. 
    /// It allows subscribing to events triggered when the object heals.
    /// </summary>
    public UnityEvent<HealData, uint, uint> OnHeal { get; set; }

    /// <summary>
    /// Heals a health component using the HealData struct
    /// </summary>
    /// <param name="healData">The heal data to apply</param>
    public void Heal(HealData healData);
}
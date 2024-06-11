/// <summary>
/// Interface that defines properties and methods related to healing
/// </summary>
public interface IHealable
{
    /// <summary>
    /// This property exposes a `Health.OnHealthChanged` delegate. 
    /// It allows subscribing to events triggered when the object heals.
    /// </summary>
    public Health.OnHealthChangedEvent OnHeal { get; set; }

    /// <summary>
    /// Heals the health component by a given amount
    /// </summary>
    /// <param name="amountToHeal">The amount to heal a health component</param>
    public void Heal(uint amountToHeal);
}
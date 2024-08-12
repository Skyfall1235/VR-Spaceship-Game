using UnityEngine.Events;

/// <summary>
/// Interface that defines properties and methods related to handling damage
/// </summary>
public interface IDamagable
{
    /// <summary>
    /// This property exposes a `Health.OnHealthChangedEvent` delegate. 
    /// It allows subscribing to events triggered when the object takes damage.
    /// </summary>
    public UnityEvent<DamageData, uint, uint> OnDamage { get; set; }

    /// <summary>
    /// This method takes a `DamageData` object containing information about the damage and applies it to the object.
    /// You can optionally choose to ignore invulnerability and armor while applying damage.
    /// </summary>
    /// <param name="damageData">The DamageData object containing information about the damage.</param>
    /// <param name="ignoreInvulnerabilityAfterDamage">Optional flag to ignore invulnerability after damage (default: false).</param>
    /// <param name="ignoreArmor">Optional flag to ignore armor while applying damage (default: false).</param>
    public void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false);
}
using static Health;

/// <summary>
/// Interface that defines properties and methods related to handling damage and healing.
/// </summary>
public interface IDamagable
{

    /// <summary>
    /// This property exposes a `DamageData.OnHealEvent` delegate. 
    /// It allows subscribing to events triggered when the object heals.
    /// </summary>
    public DamageData.OnHealEvent OnHeal { get; set; }

    /// <summary>
    /// This property exposes a `DamageData.OnDamageEvent` delegate. 
    /// It allows subscribing to events triggered when the object takes damage.
    /// </summary>
    public DamageData.OnDamageEvent OnDamage { get; set; }

    /// <summary>
    /// This property exposes a `DamageData.OnHealthComponentInitialized` delegate. 
    /// It allows subscribing to events triggered when the health component of the object is initialized.
    /// </summary>
    public DamageData.OnHealthComponentInitialized OnHealthInitialized { get; set; }

    /// <summary>
    /// This method takes a `DamageData` object containing information about the damage and applies it to the object.
    /// You can optionally choose to ignore invulnerability and armor while applying damage.
    /// </summary>
    /// <param name="damageData">The DamageData object containing information about the damage.</param>
    /// <param name="ignoreInvulnerabilityAfterDamage">Optional flag to ignore invulnerability after damage (default: false).</param>
    /// <param name="ignoreArmor">Optional flag to ignore armor while applying damage (default: false).</param>
    public void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false);
}

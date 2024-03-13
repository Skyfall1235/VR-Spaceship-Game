/// <summary>
/// Defines a common set of methods for modules to accept and remove damage values.
/// </summary>
public interface ISystemDamage
{
    /// <summary>
    /// Applies damage to the system.
    /// </summary>
    /// <param name="damage">The base amount of damage applied to the module</param>
    public void TakeDamage(int damage);

    /// <summary>
    /// Applies a value to heal the module by.
    /// </summary>
    /// <param name="healAmount">the maximum amount of damage able to be healed by the module</param>
    public void HealModule(int healAmount);
}

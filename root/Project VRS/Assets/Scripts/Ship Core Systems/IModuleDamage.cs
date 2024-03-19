using System;
using UnityEngine.Events;

/// <summary>
/// Defines a common set of methods for modules to accept and remove damage values.
/// </summary>
public interface IModuleDamage
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

    /// <summary>
    /// This event is triggered when a core system within the game receives healing.
    /// </summary>
    /// <remarks>
    /// This event uses the `UnityEvent` class with two arguments:
    ///  - `healAmount`: An integer representing the amount of healing applied (positive value).
    ///  - `coreSystem`: A reference to the core system that received healing (of type SO_CoreSystem).
    /// </remarks>
    [Serializable]
    public class OnHealEvent : UnityEvent<int, SO_CoreModule> { }

    /// <summary>
    /// This event is triggered when a core system within the game takes damage.
    /// </summary>
    /// <remarks>
    /// This event uses the `UnityEvent` class with two arguments:
    ///  - `damageAmount`: An integer representing the amount of damage inflicted (positive value).
    ///  - `coreSystem`: A reference to the core system that took damage (of type SO_CoreSystem).
    /// </remarks>
    [Serializable]
    public class OnDamageEvent : UnityEvent<int, SO_CoreModule> { }
    
}

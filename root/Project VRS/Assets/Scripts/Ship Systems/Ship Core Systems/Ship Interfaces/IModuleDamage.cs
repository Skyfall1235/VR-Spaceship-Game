using System;
using UnityEngine.Events;

/// <summary>
/// Defines a common set of methods for modules to accept and remove damage values.
/// </summary>
public interface IModuleDamage : IDamageData
{
    /// <summary>
    /// This event is triggered when a core system within the game receives healing.
    /// </summary>
    /// <remarks>
    /// This event uses the `UnityEvent` class with two arguments:
    ///  - 
    ///  - `coreSystem`: A reference to the core system that received healing (of type SO_CoreSystem).
    /// </remarks>
    [Serializable]
    public class OnHealEvent : UnityEvent<HealModuleData, BC_CoreModule> { }

    /// <summary>
    /// This event is triggered when a core system within the game takes damage.
    /// </summary>
    /// <remarks>
    /// This event uses the `UnityEvent` class with two arguments:
    ///  - 
    ///  - `coreSystem`: A reference to the core system that took damage (of type SO_CoreSystem).
    /// </remarks>
    [Serializable]
    public class OnDamageEvent : UnityEvent<WeaponCollisionData, BC_CoreModule> { }


    public void TakeDamage(WeaponCollisionData damageData);


    public void HealModule(HealModuleData healData);

    
    
}

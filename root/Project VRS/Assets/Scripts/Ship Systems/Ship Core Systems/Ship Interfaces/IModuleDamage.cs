using System;
using UnityEngine.Events;

/// <summary>
/// Defines a common set of methods for modules to accept and remove damage values.
/// </summary>
public interface IModuleDamage : IDamageData
{

    /// <summary>
    /// Represents a custom event that is invoked when a healing action occurs.
    /// </summary>
    /// <remarks>
    /// Inherits from UnityEvent to provide a flexible way to subscribe to and trigger the event.
    /// Requires two arguments when invoked:
    ///   - HealModuleData: Contains information about the healing action.
    ///   - BC_CoreModule: reference to the affected module.
    /// </remarks>
    [Serializable]
    public class OnHealEvent : UnityEvent<HealModuleData, BC_CoreModule> { }


    /// <summary>
    /// Represents a custom event that is triggered when damage is taken.
    /// </summary>
    /// <remarks>
    /// Inherits from UnityEvent to allow for subscriptions and event triggering.
    /// Requires two arguments when invoked:
    ///   - WeaponCollisionData: Contains information about the damage event.
    ///   - BC_CoreModule: reference to the affected module.
    /// </remarks>
    [Serializable]
    public class OnDamageEvent : UnityEvent<WeaponCollisionData, BC_CoreModule> { }

    /// <summary>
    /// A custom Unity Event that is triggered when the Modules health Script announces it is dead, and gives the module that died
    /// </summary>
    [Serializable]
    public class OnDeathEvent : UnityEvent<BC_CoreModule> { }

    /// <summary>
    /// Applies healing to a module.
    /// </summary>
    /// <param name="healData">Contains information about the healing action.</param>
    /// <remarks>
    /// restores module health or state based on the healing data.
    /// triggers the OnHealEvent to notify other systems.
    /// </remarks>
    public void HealModule(HealModuleData healData);

    /// <summary>
    /// Handles damage being inflicted on a module.
    /// </summary>
    /// <param name="damageData">Contains information about the damage event.</param>
    /// <remarks>
    /// updates module health or state based on the damage received.
    /// Triggers the OnDamageEvent to notify other systems.
    /// </remarks>
    public void TakeDamage(WeaponCollisionData damageData);

}

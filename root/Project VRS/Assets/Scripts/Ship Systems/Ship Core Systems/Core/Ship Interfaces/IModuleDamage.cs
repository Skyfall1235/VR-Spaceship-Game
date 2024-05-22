using System;
using UnityEngine.Events;
using static ICoreModule;

/// <summary>
/// Defines a common set of methods for modules to accept and remove damage values.
/// </summary>
public interface IModuleDamage : IDamageData
{

    /// <summary>
    /// UnityEvent triggered when a core module receives healing.
    /// </summary>
    [Serializable]
    public class OnHealEvent : UnityEvent<HealModuleData, IHealthEvents, ModuleStateChangeType> { }

    /// <summary>
    /// UnityEvent triggered when a core module takes damage.
    /// </summary>
    [Serializable]
    public class OnDamageEvent : UnityEvent<WeaponCollisionData, IHealthEvents, ModuleStateChangeType> { }

    /// <summary>
    /// UnityEvent triggered when a core module is destroyed.
    /// </summary>
    [Serializable]
    public class OnDeathEvent : UnityEvent<IHealthEvents, ModuleStateChangeType> { }

    /// <summary>
    /// Applies healing to a module.
    /// </summary>
    /// <param name="healData">Contains information about the healing action.</param>
    /// <remarks>
    /// restores module health or state based on the healing data.
    /// triggers the OnHealEvent to notify other systems.
    /// </remarks>
    public void HealObject(HealModuleData healData);

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

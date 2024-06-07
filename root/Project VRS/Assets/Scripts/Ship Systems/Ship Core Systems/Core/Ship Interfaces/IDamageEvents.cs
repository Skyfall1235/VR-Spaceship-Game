using System;
using static ICoreModule;
using UnityEngine.Events;
using static IDamageData;

public interface IDamageEvents 
{
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
    /// Handles damage being inflicted on a module.
    /// </summary>
    /// <param name="damageData">Contains information about the damage event.</param>
    /// <remarks>
    /// updates module health or state based on the damage received.
    /// Triggers the OnDamageEvent to notify other systems.
    /// </remarks>
    public void TakeDamage(WeaponCollisionData damageData);
}

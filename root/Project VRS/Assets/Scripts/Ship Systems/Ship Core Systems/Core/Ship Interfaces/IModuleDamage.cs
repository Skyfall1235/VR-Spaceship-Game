using System;
using UnityEngine.Events;
using static ICoreModule;

/// <summary>
/// Defines a common set of methods for modules to accept and remove damage values.
/// </summary>
public interface IModuleDamage : IDamageData, IDamageEvents
{

    /// <summary>
    /// UnityEvent triggered when a core module receives healing.
    /// </summary>
    [Serializable]
    public class OnHealEvent : UnityEvent<HealModuleData, IHealthEvents, ModuleStateChangeType> { }

    /// <summary>
    /// Applies healing to a module.
    /// </summary>
    /// <param name="healData">Contains information about the healing action.</param>
    /// <remarks>
    /// restores module health or state based on the healing data.
    /// triggers the OnHealEvent to notify other systems.
    /// </remarks>
    public void HealObject(HealModuleData healData);

    
}

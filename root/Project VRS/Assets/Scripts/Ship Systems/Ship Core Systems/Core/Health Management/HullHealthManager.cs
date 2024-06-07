using System.Collections.Generic;
using UnityEngine;
using static IDamageData;
using static IModuleDamage;

public class HullHealthManager : BC_UniversalEntityHealth, IHealthEvents
{
    //will need to subscribe to all modules heallth events

    public List<BC_UniversalEntityHealth> EntityHealths = new List<BC_UniversalEntityHealth>();

    private const int FlatRateReduction = 20;

    #region Events

    [SerializeField]
    protected OnHealEvent m_onHealEvent = new();

    [SerializeField]
    protected IDamageEvents.OnDamageEvent m_onDamageEvent = new();

    [SerializeField]
    protected IDamageEvents.OnDeathEvent m_onDeathEvent = new();

    /// <summary>
    /// Unity Event for activation of the Heal action.
    /// </summary>
    public OnHealEvent onHealEvent
    {
        get
        {
            return m_onHealEvent;
        }
        set
        {
            m_onHealEvent = value;
        }
    }

    /// <summary>
    /// Unity Event for activation of the Damage action.
    /// </summary>
    public IDamageEvents.OnDamageEvent onDamageEvent
    {
        get
        {
            return m_onDamageEvent;
        }
        set
        {
            m_onDamageEvent = value;
        }
    }

    /// <summary>
    /// Unity Event for activation of the Death action.
    /// </summary>
    public IDamageEvents.OnDeathEvent onDeathEvent
    {
        get
        {
            return m_onDeathEvent;
        }
        set
        {
            m_onDeathEvent = value;
        }
    }

    #endregion


    #region Subscription

    /// <summary>
    /// Subscribes to the health of all modules in a provided list.
    /// </summary>
    /// <param name="allModules">A List containing BC_CoreModule instances to subscribe to for health updates.</param>
    /// <remarks>
    /// This method iterates through each `BC_CoreModule` in the provided `allModules` list.
    /// For each module, it calls the `SubscribeToModule` method to add its `InternalModuleHealth` component to the internal `EntityHealths` list.
    /// This allows the script to track the health of all modules in the list for centralized monitoring or processing.
    /// </remarks>
    public void InitialSubscribeToAllModuleEvents(List<BC_CoreModule> allModules)
    {
        //add all the interanl modules to the health data
        foreach (BC_CoreModule module in allModules)
        {
            SubscribeToModule(module);
        }
    }

    /// <summary>
    /// Removes all modules subscriptions for the hull.
    /// </summary>
    public void DumpAllModules()
    {
        EntityHealths.Clear();
    }

    /// <summary>
    /// Subscribes to the health of another module.
    /// </summary>
    /// <param name="module">The BC_CoreModule instance to subscribe to for health updates.</param>
    /// <remarks>
    /// This method retrieves the `InternalModuleHealth` component from the provided `module`. 
    /// It then adds this health component to an internal list named `EntityHealths`. 
    /// </remarks>
    public void SubscribeToModule(BC_CoreModule module)
    {
        BC_UniversalEntityHealth entityHP = module.InternalModuleHealth;
        EntityHealths.Add(entityHP);
    }

    /// <summary>
    /// Unsubscribes from the health of another module.
    /// </summary>
    /// <param name="module">The BC_CoreModule instance to unsubscribe from for health updates.</param>
    /// <remarks>
    /// This method retrieves the `InternalModuleHealth` component from the provided `module`. 
    /// It then removes this health component from the internal list named `EntityHealths`. 
    /// This stops the script from tracking the health of the specific module.
    /// </remarks>
    public void UnsubscribeToModule(BC_CoreModule module)
    {
        BC_UniversalEntityHealth entityHP = module.InternalModuleHealth;
        EntityHealths.Remove(entityHP);
    }

    #endregion

    #region Health Application

    private void OnModuleTakeDamage(WeaponCollisionData dataPack)
    {

    }

    public override void InitializeHealth(IHealthEvents owner)
    {
        base.InitializeHealth(owner);
    }

    /// <summary>
    /// Applies damage to this module based on the provided damage data.
    /// </summary>
    /// <param name="damageData">The weapon collision data containing damage information.</param>
    public override void TakeDamage(IDamageData.WeaponCollisionData damageData)
    {
        //allow the health script to handle the actual number stuff and then invoke the event
        int damageValueFromDataPack = damageData.damageVal;
        StartCoroutine(DamageEntityAction(damageValueFromDataPack));
        base.TakeDamage(damageData);
    }

    /// <summary>
    /// Heals this module based on the provided heal data.
    /// </summary>
    /// <param name="healData">The heal module data containing healing information.</param>
    public override void HealObject(IDamageData.HealModuleData healData)
    {
        StartCoroutine(HealEntityAction(healData));
        base.HealObject(healData);
    }

    #endregion


    //now we need to do the hull damage thingy and factor in the reduction to damage based on the module

}

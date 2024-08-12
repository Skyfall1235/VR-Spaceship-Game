using System.Collections.Generic;
using UnityEngine;


public class HullHealthManager : Health
{
    //will need to subscribe to all modules heallth events

    public List<Health> EntityHealths = new List<Health>();

    private const int FlatRateReduction = 20;

    #region Events



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
            //SubscribeToModule(module);
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
        Health entityHP = module.InternalModuleHealth;
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
        Health entityHP = module.InternalModuleHealth;
        EntityHealths.Remove(entityHP);
    }

    #endregion

    #region Health Application

    //STILL NOT DONE
    private void OnModuleTakeDamage(BC_CoreModule module)
    {

    }

    public override void InitializeHealth()
    {
        base.InitializeHealth();
    }

    public override void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false)
    {
        base.Damage(damageData, ignoreInvulnerabilityAfterDamage, ignoreArmor);
    }


    #endregion


    //now we need to do the hull damage thingy and factor in the reduction to damage based on the module

}

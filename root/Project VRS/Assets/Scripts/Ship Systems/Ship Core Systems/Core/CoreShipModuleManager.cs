using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages a collection of core modules within a ship system.
/// This class is responsible for registering core modules, applying various operations (start-up, shut-down, reboot) to them, 
/// and retrieving information about their state and health.
/// </summary>
[RequireComponent(typeof(HullHealthManager))]
public partial class CoreShipModuleManager : MonoBehaviour, ICoreModule
{
    //what does the manmager need to handle?
    public List<BC_CoreModule> coreModules = new List<BC_CoreModule>();

    public UnityEvent<BC_CoreModule> OnModuleAdded = new UnityEvent<BC_CoreModule>();
    public UnityEvent<BC_CoreModule> OnModuleRemoved = new UnityEvent<BC_CoreModule>();

    public ReportModuleEvent OnModuleEvent = new ReportModuleEvent();


    //what is registration?
    //its adding the item to the core modules list if it not already there, and adding it to the module script.
    #region Health Management

    private void Awake()
    {
        HullHealthManager manager = GetComponent<HullHealthManager>();
        if (manager != null)
        {
            OnModuleAdded.AddListener(manager.SubscribeToModule);
            OnModuleRemoved.AddListener(manager.UnsubscribeToModule);
        }
        manager.InitialSubscribeToAllModuleEvents(coreModules);
    }

    #endregion

    #region Adding, removing, and Registering modules

    /// <summary>
    /// Adds a core module to the internal registry.
    /// </summary>
    /// <param name="module">The core module to be added.</param>
    /// <remarks>
    /// This method attempts to add the specified core module to the internal registry of modules managed by this class. 
    /// If the module is already registered, no action is taken.
    /// 
    /// After successful registration, the method attempts to establish a link between the module and this manager. This may involve 
    /// the module registering for events or accessing functionalities provided by this manager.
    /// </remarks>
    public void AddSingleModule(BC_CoreModule module)
    {
        //if the module is already registered, we dont want to do it again
        if (coreModules.Contains(module))
        {
            return;
        }
        coreModules.Add(module);
        module.AttemptToLinkManager(this);
        OnModuleAdded.Invoke(module);
    }

    public void RemoveSingleModule(BC_CoreModule module)
    {
        //if the module is already registered, we dont want to do it again
        if (coreModules.Contains(module))
        {
            return;
        }
        coreModules.Remove(module);
        module.DeregisterCoreSystemManager(this);
        OnModuleRemoved.Invoke(module);
        //remove all references to any unity events
    }

    #endregion

    #region Module Affectors

    /// <summary>
    /// Applies the specified affector to all registered core modules.
    /// </summary>
    /// <param name="affectorType">The type of affector to apply (StartUp, ShutDown, Reboot).</param>
    private void ApplyAffectorOnAllModules(AffectorType affectorType)
    {
        //apply the affector to all modules registered
        ApplyAffectorOnSelectModules(affectorType, coreModules.ToArray());
    }

    /// <summary>
    /// Applies the specified affector to a selection of core modules.
    /// </summary>
    /// <param name="affectorType">The type of affector to apply (StartUp, ShutDown, Reboot).</param>
    /// <param name="selectedModules">An array of core modules to which the affector will be applied.</param>
    private void ApplyAffectorOnSelectModules(AffectorType affectorType, params BC_CoreModule[] selectedModules)
    {
        foreach (BC_CoreModule module in selectedModules)
        {
            //cann the action with the module as the parameter
            ApplyAffectorOnModule(affectorType, module);
        }
    }

    /// <summary>
    /// Applies a specific affector to a single core module.
    /// </summary>
    /// <param name="affectorType">The type of affector to apply (StartUp, ShutDown, Reboot).</param>
    /// <param name="chosenModule">The core module to which the affector will be applied.</param>
    private void ApplyAffectorOnModule(AffectorType affectorType, BC_CoreModule chosenModule)
    {
        switch (affectorType)
        {
            case AffectorType.StartUp:
                chosenModule.StartUp();
                break;
            case AffectorType.ShutDown:
                chosenModule.ShutDown();
                break;
            case AffectorType.Reboot:
                chosenModule.Reboot();
                break;
        }
    }

    #endregion

    #region Data Retrieval

    //a method that is able to grab the health of any singular core module
    public int RetrieveHealthOfSelectedModule(BC_CoreModule module)
    {
        return module.InternalModuleHealth.ModuleHealth;
    }

    //method that checks the operational state of all objects whenever it is called
    //this needs to be able to retrieve any 1 state so we can use it in constructors
    public ICoreModule.CoreModuleState GetSingleCoreModuleState(BC_CoreModule selectedModule)
    {
        return selectedModule.CoreModuleState;
    }

    public ICoreModule.ModuleOperationalState GetSingleCoreModuleOperationalState(BC_CoreModule selectedModule)
    {
        return selectedModule.OperationalState;
    }

    #endregion

    #region Module Event Handler

    public void OnModuleEventRepeater(BC_CoreModule module, ICoreModule.ModuleStateChangeType changeType)
    {
        OnModuleEvent.Invoke(module, changeType);
    }

    #endregion

    #region Custom DataTypes

    /// <summary>
    /// Custom UnityEvent class specifically designed to report core module state changes.
    /// This class inherits from UnityEvent and overrides its functionality to handle two arguments:
    /// - BC_CoreModule: The core module that experienced the state change.
    /// - ICoreModule.ModuleStateChangeType: The type of state change that occurred (e.g., Core State, OperationalState, Health, Destroyed).
    /// </summary>
    [Serializable]
    public class ReportModuleEvent : UnityEvent<BC_CoreModule, ICoreModule.ModuleStateChangeType> { }

    /// <summary>
    /// Defines the different types of operations that can be applied to core modules.
    /// </summary>
    [Serializable]
    public enum AffectorType
    {
        /// <summary>
        /// Initiates the startup process for a core module.
        /// </summary>
        StartUp,

        /// <summary>
        /// Initiates the shutdown process for a core module.
        /// </summary>
        ShutDown,

        /// <summary>
        /// Initiates a reboot process for a core module.
        /// </summary>
        Reboot
    }

    #endregion
}

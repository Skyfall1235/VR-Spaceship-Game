using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines the interface for core systems within the VR spaceship, providing common functionalities like startup, shutdown, resource management, and state management.
/// </summary>
public interface ICoreModule
{
    #region Behavior Definition

    /// <summary>
    /// Defines the core system state, which can be Operational, Standby, or Disabled.
    /// </summary>
    [Serializable]
    public enum CoreModuleState
    {
        /// <summary>
        /// The system is fully functional and ready to use.
        /// </summary>
        Operational,

        /// <summary>
        /// The system is not currently active but can be quickly brought online.
        /// </summary>
        Standby,

        /// <summary>
        /// The system is offline and cannot be used until repaired or reactivated.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Defines the different operational states a system can be in, such as Active, Preparing, ReadyForUse, Damaged, or Rebooting.
    /// </summary>
    [Serializable]
    public enum ModuleOperationalState
    {
        /// <summary>
        /// The system is currently active and performing its primary function.
        /// </summary>
        Active,

        /// <summary>
        /// The system is currently preparing to become active, such as warming up or initializing components.
        /// </summary>
        Preparing,

        /// <summary>
        /// The system is ready to be used but may not be actively performing its function yet.
        /// </summary>
        ReadyForUse,

        /// <summary>
        /// The system has sustained damage and may not be functioning at full capacity.
        /// </summary>
        Damaged,

        /// <summary>
        /// The system is currently rebooting and may be unavailable for a short period.
        /// </summary>
        Rebooting
    }


    /// <summary>
    /// Enumeration defining the different types of state changes that can occur for a core module.
    /// </summary>
    [Serializable]
    public enum ModuleStateChangeType
    {
        /// <summary>
        /// Indicates a change in the core module's overall state (e.g., Operational, On Standby, Disabled).
        /// </summary>
        CoreState,

        /// <summary>
        /// Indicates a change in the core module's operational state.
        /// </summary>
        OperationalState,

        /// <summary>
        /// Indicates a change in the core module's health (damage taken or healing received).
        /// </summary>
        Health,

        /// <summary>
        /// Indicates the core module has been destroyed.
        /// </summary>
        Destroyed
    }



    /// <summary>
    /// UnityEvent triggered when a core module's overall state changes.
    /// </summary>
    [Serializable]
    public class OnCoreModuleStateChange : UnityEvent<CoreModuleState, BC_CoreModule, ModuleStateChangeType> { }



    /// <summary>
    /// UnityEvent triggered when a core module's operational state changes.
    /// </summary>
    [Serializable]
    public class OnModuleOperationalStateChange : UnityEvent<ModuleOperationalState, BC_CoreModule, ModuleStateChangeType> { }


    #endregion
}

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines the interface for core systems within the VR spaceship, providing common functionalities like startup, shutdown, resource management, and state management.
/// </summary>
public interface ICoreSystem
{
    #region Behavior Definition

    /// <summary>
    /// Defines the core system state, which can be Operational, Standby, or Disabled.
    /// </summary>
    public enum CoreSystemState
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
    public enum SystemOperationalState
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
    /// Defines the type of resources required by the system, such as Energy, Fuel, Material, or combinations thereof.
    /// </summary>
    public enum SystemResourceRequirement
    {
        /// <summary>
        /// The system requires only Energy.
        /// </summary>
        Energy,

        /// <summary>
        /// The system requires only Fuel.
        /// </summary>
        Fuel,

        /// <summary>
        /// The system requires only Material.
        /// </summary>
        Material,

        /// <summary>
        /// The system requires both Energy and Material.
        /// </summary>
        MaterialAndEnergy,

        /// <summary>
        /// The system requires both Energy and Fuel.
        /// </summary>
        EnergyAndFuel,

        /// <summary>
        /// The system requires both Fuel and Material.
        /// </summary>
        FuelAndMaterial,

        /// <summary>
        /// The system does not require any resources.
        /// </summary>
        None
    }

    /// <summary>
    /// A custom UnityEvent class specifically designed to handle OnCoreStateChange events.
    /// </summary>
    public class OnSystemOperationalStateChange : UnityEvent<SystemOperationalState> { }

    #endregion
}

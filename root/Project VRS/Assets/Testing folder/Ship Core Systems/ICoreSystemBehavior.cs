using UnityEngine;

/// <summary>
/// Defines the expected behavior for core systems.
/// </summary>
public interface ICoreSystemBehavior
{
    #region Required Methods

    /// <summary>
    /// Starts up the system and initializes its functionality.
    /// </summary>
    public void StartUp();

    /// <summary>
    /// Shuts down the system and safely releases resources.
    /// </summary>
    public void ShutDown();

    /// <summary>
    /// Registers the specified CoreShipSystemManager with this system.
    /// </summary>
    /// <param name="currentManager">The CoreShipSystemManager to register.</param>
    public void RegisterCoreSystemManager(CoreShipSystemManager currentManager);

    /// <summary>
    /// Deregisters the specified CoreShipSystemManager from this system.
    /// </summary>
    /// <param name="currentManager">The CoreShipSystemManager to deregister.</param>
    public void DeregisterCoreSystemManager(CoreShipSystemManager currentManager);

    /// <summary>
    /// Allocates the specified resources for the system.
    /// </summary>
    /// <param name="resources">The resources to allocate (Energy, Fuel, etc.).</param>
    /// <param name="amount">The amount of each resource to allocate.</param>
    /// <returns>True if allocation was successful, false otherwise.</returns>
    public bool AllocateResources(ICoreSystem.SystemResourceRequirement resources, Vector3Int amount);

    /// <summary>
    /// Checks if the system has enough resources available for the specified allocation.
    /// </summary>
    /// <param name="resources">The resources to check (Energy, Fuel, etc.).</param>
    /// <param name="amount">The amount of each resource needed.</param>
    /// <returns>True if resources are available, false otherwise.</returns>
    public bool CheckResourceAvailability(ICoreSystem.SystemResourceRequirement resources, Vector3Int amount);

    #endregion
}

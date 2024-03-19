using UnityEngine;

/// <summary>
/// Defines the expected behavior for core systems.
/// </summary>
public interface ICoreModuleBehavior
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
    /// Registers the specified CoreShipModuleManager with this system.
    /// </summary>
    /// <param name="currentManager">The CoreShipModuleManager to register.</param>
    public void RegisterCoreModuleManager(CoreShipModuleManager currentManager);

    /// <summary>
    /// Deregisters the specified CoreShipModuleManager from this system.
    /// </summary>
    /// <param name="currentManager">The CoreShipModuleManager to deregister.</param>
    public void DeregisterCoreSystemManager(CoreShipModuleManager currentManager);

    #endregion
}

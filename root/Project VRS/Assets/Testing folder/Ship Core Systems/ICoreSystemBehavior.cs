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

    #endregion
}

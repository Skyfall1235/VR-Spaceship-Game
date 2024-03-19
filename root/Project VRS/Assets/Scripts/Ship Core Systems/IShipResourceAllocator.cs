using UnityEngine;

public interface IShipResourceAllocator
{
    /// <summary>
    /// Allocates the specified resources for the system.
    /// </summary>
    /// <param name="resources">The resources to allocate (Energy, Fuel, etc.).</param>
    /// <param name="resourceAmount">the amount of resources the module wishes to allocate</param>
    /// <returns>True if allocation was successful, false otherwise.</returns>
    public bool AllocateResources(ICoreModule.SystemResourceRequirement resources, Vector3Int resourceAmount);

    /// <summary>
    /// Checks if the system has enough resources available for the specified allocation.
    /// </summary>
    /// <param name="resources">The resources to check (Energy, Fuel, etc.).</param>
    /// <param name="resourceAmount">the amount of resources the module wishes to allocate</param>
    /// <returns>True if resources are available, false otherwise.</returns>
    public bool CheckResourceAvailability(ICoreModule.SystemResourceRequirement resources, Vector3Int resourceAmount);
}

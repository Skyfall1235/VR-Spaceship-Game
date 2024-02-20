using UnityEngine;

/// <summary>
/// 
/// </summary>
public interface ICoreSystem
{
    /// <summary>
    /// 
    /// </summary>
    enum CoreSystemState
    {
        /// <summary>
        /// 
        /// </summary>
        Operational,
        /// <summary>
        /// 
        /// </summary>
        Standby,
        /// <summary>
        /// 
        /// </summary>
        Disabled
    }
    /// <summary>
    /// 
    /// </summary>
    public CoreSystemState CoreState { get; }

    /// <summary>
    /// 
    /// </summary>
    enum SystemOperationalState
    {
        /// <summary>
        /// 
        /// </summary>
        Active,
        /// <summary>
        /// 
        /// </summary>
        Preparing,
        /// <summary>
        /// 
        /// </summary>
        ReadyForUse,
        /// <summary>
        /// 
        /// </summary>
        Damaged,
        /// <summary>
        /// 
        /// </summary>
        Rebooting
    }
    /// <summary>
    /// 
    /// </summary>
    public SystemOperationalState OperationalState { get; }

    /// <summary>
    /// 
    /// </summary>
    public float SystemHealth { get; set; }

    /// <summary>
    /// 
    /// </summary>
    enum SystemResourceRequirement
    {
        /// <summary>
        /// 
        /// </summary>
        Energy,
        /// <summary>
        /// 
        /// </summary>
        Fuel,
        /// <summary>
        /// 
        /// </summary>
        Material,
        /// <summary>
        /// 
        /// </summary>
        MaterialAndEnergy,
        /// <summary>
        /// 
        /// </summary>
        EnergyAndFuel,
        /// <summary>
        /// 
        /// </summary>
        FuelAndMaterial,
        /// <summary>
        /// 
        /// </summary>
        None
    }
    /// <summary>
    /// 
    /// </summary>
    public SystemResourceRequirement requiredResources { get; }

    /// <summary>
    /// 
    /// </summary>
    public Vector3Int RequiredResourceAllocations { get; set; }
}

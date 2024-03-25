using System;
using UnityEngine;

//NOT SURE WHAT TO USE THIS FOR, PERHAPS WILL BE IN THE BASE CLASS SOMEWHERE?

[CreateAssetMenu(menuName = "Core System Object")]
[Serializable]
public class SO_CoreModule : ScriptableObject
{

    #region Current States
    /// <summary>
    /// Represents the maximum total amount of resources that can be allocated to the system.
    /// </summary>
    const int m_maximumResourceAllocationAmount = 7;
    /// <summary>
    /// Represents the maximum amount of resources that can be allocated to any individual section within the system.
    /// </summary>
    const int m_maximumResourceAllocationPerSection = 5;

    /// <summary>
    /// The specific resource allocations required by the system, expressed as a Vector3Int where each component represents the amount of energy applied to the engines, avionics, and weapons.
    /// </summary>
    [SerializeField]
    public Vector3Int m_requiredResourceAllocations;

    #endregion
}

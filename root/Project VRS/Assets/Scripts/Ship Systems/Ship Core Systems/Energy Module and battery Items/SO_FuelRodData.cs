using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Fuel Rod Data")]
[Serializable]
public class SO_FuelRodData : ScriptableObject
{
    [SerializeField]
    [Tooltip("The maximum amount of total energy this fuel rod can provide before it is spent. This value affects the depletion status.")]
    private int m_maxEnergyGenerationValue;
    /// <summary>
    /// Gets the maximum energy generation value (capacity) of the energy generation module.
    /// </summary>
    public int MaxEnergyGenerationValue
    {
        get { return m_maxEnergyGenerationValue; }
    }

    [Tooltip("The maximum energy output per second that this fuel rod can produce.")]
    [SerializeField]
    private int m_energyGenerationRate;
    /// <summary>
    /// Gets the energy generation rate of the energy generation module (how much energy it generates per unit of time).
    /// </summary>
    public int EnergyGenerationRate
    {
        get { return m_energyGenerationRate; }
    }
}

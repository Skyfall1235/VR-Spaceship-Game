using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelRodBehavior : MonoBehaviour
{
    [SerializeField]
    SO_FuelRodData fuelData;

    [Tooltip("The maximum energy output per second that this fuel rod can produce.")]
    public int EnergyGenerationRate
    {
        get => fuelData.EnergyGenerationRate;
    }

    [SerializeField]
    [Tooltip("The current energy output per second that this fuel rod can produce.")]
    private int m_currentEnergyGenerationValue;
    /// <summary>
    /// The current energy output per second that this fuel rod can produce.
    /// </summary>
    public int CurrentEnergygenerationValue
    {
        get
        {
            return m_currentEnergyGenerationValue;
        }
        set
        {
            m_currentEnergyGenerationValue = value;
            if( m_currentEnergyGenerationValue > 0)
            {
                m_depletionStatus = EnergyGenerationModule.DepletionStatus.Partial;
            }
            if( m_currentEnergyGenerationValue <= 0 )
            {
                m_depletionStatus = EnergyGenerationModule.DepletionStatus.Depleted;
            }
        }
    }

    [SerializeField]
    [Tooltip("The current status of the fuel rod relative to how used it is.")]
    private EnergyGenerationModule.DepletionStatus m_depletionStatus = EnergyGenerationModule.DepletionStatus.Full;
    /// <summary>
    /// Gets the current depletion status of the energy generation module.
    /// </summary>
    public EnergyGenerationModule.DepletionStatus DepletionStatus
    {
        get { return m_depletionStatus; }
    }

    //All methods
    private void Awake()
    {
        InitializeFuelRod();
    }

    void InitializeFuelRod()
    {
        m_currentEnergyGenerationValue = fuelData.MaxEnergyGenerationValue;
    }

}

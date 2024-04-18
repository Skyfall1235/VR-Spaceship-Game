using UnityEngine;

/// <summary>
/// Represents the behavior of a single fuel rod within an energy generation module.
/// This class manages the fuel rod's energy generation capabilities and depletion state.
/// </summary>
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
    [Tooltip("The current amount of energy that this fuel rod can produce.")]
    private int m_currentEnergyGenerationValue;
    /// <summary>
    /// The current energy output per second that this fuel rod can produce.
    /// </summary>
    public int CurrentEnergyGenerationValue
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

    /// <summary>
    /// percent of the fuel that is currently remaining.
    /// </summary>
    public int UsagePercentRemaining
    {
        get
        {
            float convertedVal = (float)m_currentEnergyGenerationValue;
            convertedVal.Remap(0f, (float)fuelData.MaxEnergyGenerationValue, 0f, 100f);
            return (int)convertedVal;
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

    public int SpendUpToFuelRate(int requestedOutput, out bool isDepleted)
    {
        int pullAmount = 0;
        int requestedPullMin = Mathf.Min(requestedOutput, EnergyGenerationRate);
        // Check if current energy is enough for the full pull
        if (CurrentEnergyGenerationValue >= requestedPullMin)
        {
            // Enough energy, pull at full rate
            pullAmount = requestedPullMin;
            isDepleted = false;
        }
        else
        {
            // Not enough energy, pull all remaining energy
            pullAmount = requestedPullMin;
            isDepleted = true;
        }
        //reduce the current energy amount by whatever the end value is
        CurrentEnergyGenerationValue -= pullAmount;
        return pullAmount;
    }

}

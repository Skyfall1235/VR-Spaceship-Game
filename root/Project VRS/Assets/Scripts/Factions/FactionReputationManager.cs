using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionReputationManager : MonoBehaviour
{
    public List<FactionToReputationsBinding> FactionToFactionReputations = new();

    [SerializeField]
    CustomLogger m_customLogger;

    [System.Serializable]
    public class FactionToReputationsBinding
    {
        [SerializeField]
        private SO_FactionData m_factionInfo;
        public SO_FactionData FactionData 
        { 
            get 
            { 
                return m_factionInfo; 
            }
        }
        [SerializeField]
        private FactionReputation m_factionRepuationData;
        public FactionReputation FactionRepuationData
        {
            get
            {
                return m_factionRepuationData;
            }
        }
    }
}

[System.Serializable]
public class FactionReputation
{
    public List<ReputationDatum> ReputationData = new List<ReputationDatum>();
    private const int MaxOffsetFromNeutral = 10000;

    /// <summary>
    /// Modifies the reputation standing of a faction.
    /// </summary>
    /// <param name="faction">The faction whose reputation to modify.</param>
    /// <param name="value">The amount to change the reputation by (positive for increase, negative for decrease).</param>
    /// <remarks>
    /// This method attempts to find the reputation data for the specified faction. If found, the value is added to the existing standing. 
    /// If the faction data is not found, a log message is created using the provided CustomLogger class.
    /// </remarks>
    public void ModifyStanding(SO_FactionData faction, int value)
    {
        ReputationDatum foundDatum;
        if (FindReputationDatum(faction, out foundDatum))
        {
            foundDatum.Value += value;
        }
        else
        {
            CustomLogger.Log
                (
                "Attempting to increase standing on a faction that does not exist in the reputation data of this faction", 
                CustomLogger.LogLevel.Error, 
                CustomLogger.LogCategory.System
                );
        }
    }

    /// <summary>
    /// Adds a new faction to the reputation data if it doesn't already exist.
    /// </summary>
    /// <param name="faction">The faction data to add.</param>
    /// <remarks>
    /// This method checks if the reputation data already contains information for the provided faction. 
    /// If not, a new ReputationDatum object is created and added to the internal ReputationData list.
    /// </remarks>
    public void AddFactionToReputationData(SO_FactionData faction)
    {
        if (!FindReputationDatum(faction))
        {
            ReputationDatum newDatum = new ReputationDatum(faction);
            ReputationData.Add(newDatum);
        }
    }

    /// <summary>
    /// Checks the reputation of a given faction.
    /// </summary>
    /// <param name="factionToCheck">The faction whose reputation to check.</param>
    /// <param name="repValue">Outputs the reputation value of the faction (if found).</param>
    /// <returns>True if the faction's reputation data is found, False otherwise.</returns>
    public bool CheckFactionReputation(SO_FactionData factionToCheck, out ReputationValue repValue)
    {
        // Handle null input for factionToCheck
        if (factionToCheck == null)
        {
            repValue = ReputationValue.Neutral; // Set neutral reputation for invalid input
            return false;
        }

        // Find the reputation data for the faction
        ReputationDatum foundDatum;
        if (!FindReputationDatum(factionToCheck, out foundDatum)) // Use negation for clarity
        {
            repValue = ReputationValue.Neutral; // Set neutral reputation if not found
            return false;
        }

        // Reputation data found, set output value and return success
        repValue = ConvertIntToReputationValue(foundDatum.Value);
        return true;
    }

    private bool FindReputationDatum(SO_FactionData factionToCheck, out ReputationDatum foundDatum)
    {
        // Iterate through each ReputationDatum in the ReputationData list
        foreach (ReputationDatum datum in ReputationData)
        {
            // Check if the faction stored in the datum matches the provided faction
            if (datum.FactionRef == factionToCheck)
            {
                // Found a match, set output parameter and return success
                foundDatum = datum;
                return true;
            }
        }

        // No match found, set output parameter to null and return false
        foundDatum = null;
        return false;
    }

    //override in case i just need to check if one already exists
    private bool FindReputationDatum(SO_FactionData factionToCheck)
    {
        // Iterate through each ReputationDatum in the ReputationData list
        foreach (ReputationDatum datum in ReputationData)
        {
            // Check if the faction stored in the datum matches the provided faction
            if (datum.FactionRef == factionToCheck)
            {
                return true;
            }
        }
        return false;
    }

    private ReputationValue ConvertIntToReputationValue(int intValueToConvert)
    {
        const int Divider = 1000;
        //since the value is clamped to +/-10k, Divider will never fail
        int divisors = intValueToConvert / Divider;
        switch (divisors)
        {
            case -5:
            case -4:
            case -3:
            case -2:
                return ReputationValue.Hostile;
            case -1:
                return ReputationValue.Disfavored;
            case 0:
                return ReputationValue.Neutral;
            case 1:
                return ReputationValue.Cooperative;
            case 2:
            case 3:
            case 4:
            case 5:
                return ReputationValue.Trusted;
            default:
                return ReputationValue.Neutral;
        }
    }

    #region Custom Data types

    /// <summary>
    /// Stores data associated with a faction's reputation.
    /// </summary>
    [System.Serializable]
    public class ReputationDatum
    {
        [SerializeField]
        private string m_factionName;
        public string FactionName
        {
            get
            {
                return m_factionName;
            }
        }

        [SerializeField]
        private SO_FactionData m_factionRef;

        /// <summary>
        /// A reference to the SO_FactionData ScriptableObject representing the faction this data applies to.
        /// </summary>
        public SO_FactionData FactionRef
        {
            get
            {
                return m_factionRef;
            }
        }

        [SerializeField]
        private int m_value = 0;

        /// <summary>
        /// The numerical value representing the faction's reputation standing (typically ranges from -MaxOffsetFromNeutral to MaxOffsetFromNeutral).
        /// </summary>
        public int Value
        {
            get 
            { 
                return m_value;
            }
            set
            {
                m_value = Mathf.Clamp(value, -MaxOffsetFromNeutral, MaxOffsetFromNeutral);
            }
        }

        public ReputationDatum(SO_FactionData factionRef)
        {
            m_factionRef = factionRef;
            m_factionName = factionRef.name;
        }
    }


    /// <summary>
    /// Represents the reputation level a player or faction may have with another faction.
    /// </summary>
    [SerializeField]
    public enum ReputationValue
    {
        /// <summary>
        /// The faction is considered an enemy and will be attacked on sight.
        /// </summary>
        Hostile,

        /// <summary>
        /// The faction is viewed with suspicion and may be treated with hostility.
        /// </summary>
        Disfavored,

        /// <summary>
        /// The faction has no standing reputation with the other faction. They will be treated with indifference.
        /// </summary>
        Neutral,

        /// <summary>
        /// The faction has established a positive working relationship with the other faction.
        /// </summary>
        Cooperative,

        /// <summary>
        /// The faction has earned the trust and respect of the other faction.
        /// </summary>
        Trusted
    }

    #endregion

}


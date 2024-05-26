using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionReputationManager : MonoBehaviour
{
    public List<FactionToReputationsBinding> FactionToFactionReputations = new();

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

    public void IncreaseStanding(Faction faction, int value)
    {

    }

    public void DecreaseStanding(Faction faction, int value)
    {

    }

    public void AddFactionToReputationData(Faction faction)
    {

    }

    /// <summary>
    /// Checks the reputation of a given faction.
    /// </summary>
    /// <param name="factionToCheck">The faction whose reputation to check.</param>
    /// <param name="repValue">Outputs the reputation value of the faction (if found).</param>
    /// <returns>True if the faction's reputation data is found, False otherwise.</returns>
    public bool CheckFactionReputation(Faction factionToCheck, out ReputationValue repValue)
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
        repValue = foundDatum.ReputationStanding;
        return true;
    }

    private bool FindReputationDatum(Faction factionToCheck, out ReputationDatum foundDatum)
    {
        // Iterate through each ReputationDatum in the ReputationData list
        foreach (ReputationDatum datum in ReputationData)
        {
            // Check if the faction stored in the datum matches the provided faction
            if (datum.FactionRef.Faction == factionToCheck)
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
        private ReputationValue m_reputationStanding = ReputationValue.Neutral;

        /// <summary>
        /// The current reputation level this faction has with another faction (e.g., Hostile, Neutral, Trusted).
        /// </summary>
        public ReputationValue ReputationStanding
        {
            get
            {
                return m_reputationStanding;
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


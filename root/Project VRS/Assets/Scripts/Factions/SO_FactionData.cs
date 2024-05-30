using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project VRS/New Faction Data")]
[System.Serializable]
public class SO_FactionData : ScriptableObject
{
    [SerializeField]
    public Faction Faction = new();
}

/// <summary>
/// The main class representing a faction, storing its name, description, emblem, government type, and associated ship and station data.
/// </summary>
[System.Serializable]
public class Faction
{
    [SerializeField]
    private string m_name;
    /// <summary>
    /// Gets the name of the faction.
    /// </summary>
    public string Name
    {
        get
        {
            return m_name;
        }
    }

    [SerializeField]
    private string m_description;
    /// <summary>
    /// Gets a textual description of the faction.
    /// </summary>
    public string Description
    {
        get
        {
            return m_description;
        }
    }

    [SerializeField]
    private Texture m_emblem;
    /// <summary>
    /// Gets the visual emblem representing the faction.
    /// </summary>
    public Texture Emblem
    {
        get
        {
            return m_emblem;
        }
    }

    [SerializeField]
    private Government m_governmentType;
    /// <summary>
    /// Gets the type of government the faction has (e.g., Federation, Empire).
    /// </summary>
    public Government GovernmentType
    {
        get
        {
            return m_governmentType;
        }
    }

    const int NumberOfShipTypes = 6;

    [SerializeField]
    private ClassifiedShipDatum[] m_classifiedVesselData = new ClassifiedShipDatum[NumberOfShipTypes];
    public ClassifiedShipDatum[] ClassifiedShipData
    {
        get
        {
            return m_classifiedVesselData;
        }
    }

    [SerializeField]
    public List<GameObject> m_stationPrefabs = new List<GameObject>();
    public List<GameObject> StationPrefabs
    {
        get
        {
            return m_stationPrefabs;
        }
    }

    #region Custom Data types

    /// <summary>
    /// Stores data associated with a classified ship type.
    /// </summary>
    [System.Serializable]
    public class ClassifiedShipDatum
    {
        [SerializeField]
        [Tooltip("This value is used solely to label elements in the list")]
        private string m_name;

        /// <summary>
        /// The size and capabilities of the ship (fighter, corvette, frigate, etc.).
        /// </summary>
        [SerializeField]
        [Tooltip("This is the ship descriptor for what the ships in this list are")]
        private EntitySize m_entityType;
        public EntitySize EntityType
        {
            get
            {
                return m_entityType;
            }
        }

        /// <summary>
        /// A list of prefabs representing the different variations or models of this classified ship type.
        /// </summary>
        [SerializeField]
        [Tooltip("These are all ships of this entity type")]
        private List<GameObject> m_shipPrefabs;
        public List<GameObject> ShipPrefabs
        {
            get
            { 
                return m_shipPrefabs;
            }
        }
    }

    /// <summary>
    /// Specifies the size and capabilities of an entity (ship, station, etc.).
    /// </summary>
    [SerializeField]
    public enum EntitySize
    {
        /// <summary>
        /// A small, agile spacecraft, often used for reconnaissance or dogfighting.
        /// </summary>
        Fighter,
        /// <summary>
        /// Slightly larger and more heavily armed than a fighter, capable of taking on multiple smaller targets.
        /// </summary>
        Corvette,
        /// <summary>
        /// A versatile warship with balanced firepower, armor, and maneuverability. Forms the backbone of a fleet.
        /// </summary>
        Frigate,
        /// <summary>
        /// A larger and more powerful warship, excelling in firepower and durability. Can serve as a command ship or spearhead attacks.
        /// </summary>
        Cruiser,
        /// <summary>
        /// A heavily armored and heavily armed behemoth, capable of devastating firepower against smaller ships.
        /// </summary>
        Battleship,
        /// <summary>
        /// The largest and most powerful warship class, often serving as a flagship with unmatched firepower and resilience.
        /// </summary>
        Dreadnaught
    }

    /// <summary>
    /// Represents the type of government a faction might have.
    /// </summary>
    [SerializeField]
    public enum Government
    {
        /// <summary>
        /// A union of independent states or planets working together for mutual benefit.
        /// </summary>
        Federation,
        /// <summary>
        /// A vast, centralized government with a single ruler or ruling body holding absolute power.
        /// </summary>
        Empire,
        /// <summary>
        /// A democratic system where citizens participate in electing representatives to govern on their behalf.
        /// </summary>
        Republic,
        /// <summary>
        /// A loose alliance of independent states or planets with a central governing body for mutual defense or trade.
        /// </summary>
        Confederacy,
        /// <summary>
        /// A powerful corporation or group of corporations that holds significant political and economic control over a region or species.
        /// </summary>
        Conglomerate,
        /// <summary>
        /// A collective intelligence where individual members act in unison under the control of a central consciousness.
        /// </summary>
        HiveMind,
        /// <summary>
        /// A society divided into distinct castes based on genetic engineering or predetermined roles.
        /// </summary>
        GeneticCaste,
        /// <summary>
        /// A government ruled by a single hereditary leader (king, queen, emperor, etc.).
        /// </summary>
        Monarchy,
        /// <summary>
        /// A governing body composed of representatives from different groups or factions, working together for the benefit of the whole.
        /// </summary>
        Council
    }

    #endregion
}

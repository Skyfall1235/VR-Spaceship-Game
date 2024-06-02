using UnityEngine;

public interface IBeacon : IBroadcastingProtocol
{
    /// <summary>
    /// Stores the beacon data associated with this broadcaster.
    /// </summary>
    public BeaconData BroadcastingData { get; }

    /// <summary>
    /// Class representing data transmitted by a beacon implementing the IBroadcastingProtocol interface.
    /// Inherits from the BroadcastData base class.
    /// </summary>
    public class BeaconData : BroadcastData
    {
        /// <summary>
        /// The faction data of the entity.
        /// </summary>
        public SO_FactionData m_factionData;

        /// <summary>
        /// The position in 3D space where the beacon is located.
        /// </summary>
        public Vector3 broadcastPosition;
    }
}

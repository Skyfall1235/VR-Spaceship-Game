using UnityEngine;

public interface IBeacon : IBroadcastingProtocol
{
    /// <summary>
    /// Stores the beacon data associated with this broadcaster.
    /// </summary>
    public BeaconData m_broadcastData { get; set; }

    /// <summary>
    /// Class representing data transmitted by a beacon implementing the IBroadcastingProtocol interface.
    /// Inherits from the BroadcastData base class.
    /// </summary>
    public class BeaconData : BroadcastData
    {
        /// <summary>
        /// (Temporary) The name of the faction that owns the beacon.
        /// </summary>
        public string factionName;

        /// <summary>
        /// The position in 3D space where the beacon is located.
        /// </summary>
        public Vector3 broadcastPosition;
    }
}

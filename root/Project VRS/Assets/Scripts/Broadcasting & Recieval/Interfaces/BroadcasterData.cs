using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static IBeacon;

public class BroadcasterData
{
    public class DistressBeaconData : BeaconData
    {
        public DistressType BeaconDistressType;
        public enum DistressType
        {
            DriveFailure,
            CriticalDamage,
            Derelict,
            Unknown
        }

        //is used to trigger false flag events like starting meteor swarms or a pirate attack
        public bool FalseFlag;
        public UnityEvent FalseFlagEvent;

        public DistressBeaconData(SO_FactionData factionData, Vector3 broadcastPosition) : base(factionData, broadcastPosition)
        {
        }
    }
    public class InterceptionBeaconData : BeaconData
    {
        //to be done at a later date when encryption becomes an added feature
        public InterceptionBeaconData(SO_FactionData factionData, Vector3 broadcastPosition) : base(factionData, broadcastPosition)
        {
        }
    }
    public class AntennaData : BeaconData
    {
        public AntennaData(SO_FactionData factionData, Vector3 broadcastPosition) : base(factionData, broadcastPosition)
        {
        }
    }

}

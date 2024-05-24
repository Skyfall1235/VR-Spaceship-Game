using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static IBeacon;
using static IBroadcastingProtocol;

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
            Unknown,
        }

        //is used to trigger false flag events like starting meteor swarms or a pirate attack
        public bool FalseFlag;
        public UnityEvent FalseFlagEvent;


    }
    public class InterceptionBeaconData : BeaconData
    {

    }
    public class AntennaData : BeaconData
    {

    }

}

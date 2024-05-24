using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShipIdentificationData :MonoBehaviour
{
    public enum ShipSizeType
    {
        Fighter,
        Corvette,
        Frigate,
        Cruiser,
        Battleship,
        Dreadnaught
    }

    //something like a list that contains a bool asking for docking capbabilty and what ship sizes, and how many :)
    //the locations any other stuff can be lefts to another system, this is just a data system, a reference if you would

    [System.Serializable]
    public struct DockingCapability
    {
        public bool HasSlots;
        public int SlotCount;
    }
}

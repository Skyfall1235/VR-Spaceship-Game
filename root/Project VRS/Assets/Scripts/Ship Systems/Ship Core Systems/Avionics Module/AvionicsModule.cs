using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvionicsModule : BC_CoreModule
{
    //neds to be tracking all sorts of data
    //we need position, the energy percent of rods and thier status, an HP pool and a showcase of damaged modules
    //the current pip syste, the velocity of the ship,

    //seperate into sections, maybe use a struct for each area?

    //current weapon, the amonut of bullets remaining or magwzine left
    //targeting stuff like priority target

    //the shield HP

    //Gs pulled currently

    //the current position of the joystick and the brake if its applied
    //speed and throttle


    //sections 
    //-Flight
    //  -casual flight/ fast travel stuff
    //  -weapons and targeting and shields
    //  -mining and resource collection
    //-Energy section
    //  -held fuel rod data
    //  -other info?

    //Radio Encryption/Decryption
    //  -???

    //resource management?
    //  -???

    //upgrading station
    //  -??? profit
    public override void Reboot()
    {
        throw new System.NotImplementedException();
    }

    public override void ReleaseResources()
    {
        throw new System.NotImplementedException();
    }

    public override void ShutDown()
    {
        throw new System.NotImplementedException();
    }

    public override void StartUp()
    {
        throw new System.NotImplementedException();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyGenerationModule : BC_CoreModule
{
    EnergyGenSocketBehavior[] sockets = new EnergyGenSocketBehavior[2];//change later for actual value

    public int EnergyPool;

    public UnityEvent<int> OnDepletionOfFuelRod = new UnityEvent<int>();

    //note, thiss is for UI elements to know the status of an object. there may be expnasion to make this pass a tuple or struct
    public DepletionStatus GetFuelRodDepletionStatus(int index)
    {
        if (index < sockets.Length)
        {
            return sockets[index].CurrentlyPluggedFuelRod.DepletionStatus;
        }
        else
        {
            //logger.Log(LogType.Error, $"Invalid index {index} for sockets array", this.gameObject.name);
            return DepletionStatus.Nulled;
        }
    }


    //som sort of corotuine that trickles down the plugged in fuel rods available fuel, and if the fuel rod hits zero, call the unity event

    //we might everse this and require an energy amount needed and the energy system attempts to pull it.
    //this way ,we can use the pip system to see where all available energy is directed
    //
    IEnumerator ExpendFuelRod()
    {
        int totalAvailableEnergy = 0;
        //every second, cycle down
        for(int i = 0; i < sockets.Length; i++)
        {
            bool isDepleted = false;
            FuelRodBehavior fuelRod = sockets[i].CurrentlyPluggedFuelRod;
            if (fuelRod.DepletionStatus != DepletionStatus.Depleted || fuelRod.DepletionStatus != DepletionStatus.Nulled)
            {
                //this needs to be changed as it is nolonger corrected
                int fuelRodPullRate = fuelRod.SpendUpToFuelRate(5, out isDepleted);
                
                totalAvailableEnergy += fuelRodPullRate;
            }
            //then check if object is depeleted
            if(isDepleted)
            {
                OnDepletionOfFuelRod.Invoke(i);
            }
        }
        const float CycleDelay = 1f;
        yield return new WaitForSeconds(CycleDelay);
    }


    #region Base Class Methods

    public override void Awake()
    {
        base.Awake();
    }

    protected override void PreStartUpLogic()
    {

    }

    protected override void PostStartUpLogic()
    {

    }

    protected override void PreShutDownLogic()
    {

    }

    protected override void PostShutDownLogic()
    {

    }

    #endregion

    public enum DepletionStatus
    {
        Full,
        Partial,
        Depleted,
        Nulled
    }
}

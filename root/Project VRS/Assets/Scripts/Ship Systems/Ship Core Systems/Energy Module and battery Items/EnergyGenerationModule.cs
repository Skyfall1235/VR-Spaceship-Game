using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnergyGenerationModule : BC_CoreModule
{
    EnergyGenSocketBehavior[] sockets = new EnergyGenSocketBehavior[2];//change later for actual value

    public int EnergyPool;

    public UnityEvent<int> OnDepletionOfFuelRod = new UnityEvent<int>();

    [SerializeField]
    Logger logger;

    //note, thiss is for UI elements to know the status of an object. there may be expnasion to make this pass a tuple or struct
    public DepletionStatus GetFuelRodDepletionStatus(int index)
    {
        if (index < sockets.Length)
        {
            return sockets[index].CurrentlyPluggedFuelRod.DepletionStatus;
        }
        else
        {
            logger.Log(LogType.Error, $"Invalid index {index} for sockets array", this.gameObject.name);
            return DepletionStatus.Nulled;
        }
    }


    //a pool of total energy to be pulled

    public void UpdateCurrentSockets()
    {

    }

    public void UpdateEnergyPool()
    {
        //iterate through the sockets and add up the 
    }

    //som sort of corotuine that trickles down the plugged in fuel rods available fuel, and if the fuel rod hits zero, call the unity event
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
                
                int fuelRodPullRate = fuelRod.SpendUpToFuelRate(out isDepleted);
                
                totalAvailableEnergy += fuelRodPullRate;
            }
            //then check if object is depeleted
            if(isDepleted)
            {

            }
        }
        const float CycleDelay = 1f;
        yield return new WaitForSeconds(CycleDelay);
    }


    #region Base Class Methods

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

    #endregion

    public enum DepletionStatus
    {
        Full,
        Partial,
        Depleted,
        Nulled
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerationModule : BC_CoreModule
{
    EnergyGenSocketBehavior[] sockets = new EnergyGenSocketBehavior[2];//change later for actual value

    [SerializeField]
    Logger logger;

    public DepletionStatus GetFuelRodDepletionStatus(int index)
    {
        if (index < sockets.Length)
        {
            return sockets[index].CurrentlyPluggedFuelRod.DepletionStatus;
        }
        else
        {
            Debug.LogError($"Invalid index {index} for sockets array");
            return DepletionStatus.Nulled;
        }
    }


    //a pool of total energy to be pulled

    public void UpdateCurrentSockets()
    {

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

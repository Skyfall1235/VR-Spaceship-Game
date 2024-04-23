using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CoreShipModuleManager : MonoBehaviour, ICoreModule
{
    int m_incomingPower;
    public int IncomingPower
    {
        get 
        {
            return m_incomingPower; 
        }
        set 
        { 
            m_incomingPower = value;
            HandlePowerDistribution(coreModules);
        }
    }

    int m_remainingPower;
    public int RemainingPower 
    {
        get 
        {
            return m_remainingPower; 
        } 
        private set 
        { 
            m_remainingPower = value; 
        }
    }
    List<BC_CoreModule> OrderModulesByPriority(List<BC_CoreModule> moduleList)
    {
        List<BC_CoreModule> newModuleList = new List<BC_CoreModule>(moduleList);
        newModuleList.Sort((module1, module2) => module1.ModuleEnergyPriority.CompareTo(module2.ModuleEnergyPriority));
        return newModuleList;
    }

    void HandlePowerDistribution(List<BC_CoreModule> moduleList)
    {
        int powerUsage = 0;
        List<BC_CoreModule> orderedList = OrderModulesByPriority(moduleList);
        foreach (BC_CoreModule module in orderedList)
        {
            if(powerUsage + module.PowerRequirements <= IncomingPower)
            {
                if(module.CoreModuleState == ICoreModule.CoreModuleState.Disabled)
                {
                    module.StartUp();
                }
                powerUsage += module.PowerRequirements;
            }
            else
            {
                if(module.CoreModuleState != ICoreModule.CoreModuleState.Disabled)
                {
                    module.ShutDown();
                }
            }
        }
        m_remainingPower = IncomingPower - powerUsage;
    }
}
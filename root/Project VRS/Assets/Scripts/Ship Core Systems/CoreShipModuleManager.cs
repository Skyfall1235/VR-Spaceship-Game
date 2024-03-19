using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreShipModuleManager : MonoBehaviour, ICoreModule
{
    //what does the manmager need to handle?
    public List<BC_CoreModule> coreModules = new List<BC_CoreModule>();

    //what is registration?
    //its adding the item to the core modules list if it not already there, and adding it to the module script.


    //registers all the systems to it, and them store them

    #region Adding, removing, and Registering modules
    public void AddSingleModule(BC_CoreModule module)
    {
        //if the module is already registered, we dont want to do it again
        if (coreModules.Contains(module))
        {
            return;
        }
        coreModules.Add(module);
        module.AttemptToLinkManager(this);
        //needs to grab all the unity events and everything else it needs from the modules
    }

    public void RemoveSingleModule(BC_CoreModule module)
    {
        //if the module is already registered, we dont want to do it again
        if (coreModules.Contains(module))
        {
            return;
        }
        coreModules.Remove(module);
        module.DeregisterCoreSystemManager(this);
        //remove all references to any unity events
    }

    internal void RegisterModules(bool register)
    {
        //go through the list of all modules and register or deregester them based on the bool
        foreach (BC_CoreModule module in coreModules)
        {
            if (register)
            {
                AddSingleModule(module);
            }
            else
            {
                RemoveSingleModule(module);
            }
        }
    }

    #endregion

    #region Module Affectors


    //ALL EFFECTORS
    private void RebootAllModules()
    {
        PerformActionOnModuleList(RebootSingleModule);
    }
    private void StartUpAllModules()
    {
        PerformActionOnModuleList(StartUpSingleModule);
    }
    private void ShutDownAllModules()
    {
        PerformActionOnModuleList(ShutDownSingleModule);
    }

    //SINGLE AFFECTORS
    private void RebootSingleModule(BC_CoreModule chosenModule)
    {

    }
    private void StartUpSingleModule(BC_CoreModule chosenModule)
    {

    }
    private void ShutDownSingleModule(BC_CoreModule chosenModule)
    { 
    
    }

    #endregion

    #region Data Retrieval

    //a method that is able to grabthe health of oany singular core module
    public int RetrieveHealthOfSelectedModule(BC_CoreModule module)
    {
        return 0;
    }

    //method that checks the operational state of all objects whenever it is called
    //this needs to be able to retrieve any 1 state so we can use it in constructors
    public ICoreModule.CoreModuleState GetSingleCoreModuleState(BC_CoreModule selectedModule)
    {
        return selectedModule.m_coreState;
    }

    #endregion

    /// <summary>
    /// A generic Statement that enacts a single method on every module in the list
    /// </summary>
    /// <param name="action"> is the method enacted on every module</param>
    internal void PerformActionOnModuleList(Action<BC_CoreModule> action)
    {
        foreach (BC_CoreModule module in coreModules)
        {
            //cann the action with the module as the parameter
            action(module);
        }
    }



}

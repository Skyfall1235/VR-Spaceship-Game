using System;
using System.Collections.Generic;
using UnityEngine;

public partial class CoreShipModuleManager : MonoBehaviour, ICoreModule
{
    //what does the manmager need to handle?
    public List<BC_CoreModule> coreModules = new List<BC_CoreModule>();

    //what is registration?
    //its adding the item to the core modules list if it not already there, and adding it to the module script.


    //registers all the systems to it, and them store them


    //we register and link, then we start up. once startup is complete, we should tell the manager.
    //maybe have 2 lists, 1 of modules that are prepared and ready and 1 that is still getting ready?

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

    protected void RegisterModules(bool register)
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


    //MULTI EFFECTORS
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
        chosenModule.Reboot();
    }
    private void StartUpSingleModule(BC_CoreModule chosenModule)
    {
        chosenModule.StartUp();
    }
    private void ShutDownSingleModule(BC_CoreModule chosenModule)
    { 
        chosenModule.ShutDown();
    }

    #endregion

    #region Data Retrieval

    //a method that is able to grab the health of any singular core module
    public int RetrieveHealthOfSelectedModule(BC_CoreModule module)
    {
        return module.InternalModuleHealth.ModuleHealth;
    }

    //method that checks the operational state of all objects whenever it is called
    //this needs to be able to retrieve any 1 state so we can use it in constructors
    public ICoreModule.CoreModuleState GetSingleCoreModuleState(BC_CoreModule selectedModule)
    {
        return selectedModule.CoreModuleState;
    }

    public ICoreModule.ModuleOperationalState GetSingleCoreModuleOperationalState(BC_CoreModule selectedModule)
    {
        return selectedModule.OperationalState;
    }

    #endregion

    //NON OF THIS IS PLANNED OR DONE YET

    //what is the purpose of these? to pipe new data to where they need to go.
    //these should bascially just be calling other events and providing those events with updated inforation
    #region Module Event Handlers
    
    //we need to get the operational state changes, he module states, the heals, the damages, and on death


    public void ModuleCoreStateHasChanged(BC_CoreModule module)
    {

    }

    public void ModuleOperationalstateHasChanged(BC_CoreModule module)
    {

    }

    public void ModulehasBeenHealed(BC_CoreModule module)
    {

    }

    public void ModuleHasbeenDamaged(BC_CoreModule module)
    {

    }

    public void ModuleHasBeenDestroyed(BC_CoreModule module)
    {

    }

    #endregion

    /// <summary>
    /// A generic Statement that enacts a single method on every module in the list
    /// </summary>
    /// <param name="action"> is the method enacted on every module</param>
    protected void PerformActionOnModuleList(Action<BC_CoreModule> action)
    {
        foreach (BC_CoreModule module in coreModules)
        {
            //cann the action with the module as the parameter
            action(module);
        }
    }



}

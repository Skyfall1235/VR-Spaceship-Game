using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class BC_CoreModule : MonoBehaviour, ICoreModule, ICoreModuleBehavior, IModuleDamage
{
    #region Virtual Methods for Setup and Usage
    public virtual void ShutDown()
    {
        throw new System.NotImplementedException();
    }

    public virtual void StartUp()
    {
        throw new System.NotImplementedException();
    }

    public virtual void HealModule(int healAmount)
    {
        throw new System.NotImplementedException();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        throw new System.NotImplementedException();
    }

    public virtual void RegisterCoreModuleManager(CoreShipModuleManager currentManager)
    {
        AttemptToLinkManager(currentManager);
    }

    public virtual void DeregisterCoreSystemManager(CoreShipModuleManager currentManager)
    {
        m_shipModuleManager = null;
    }

    #endregion

    [SerializeField] private CoreShipModuleManager m_shipModuleManager;
    public CoreShipModuleManager ShipModuleManager
    { get => m_shipModuleManager; }

    [SerializeField] private SO_CoreModule m_coreModuleVariables;
    public SO_CoreModule CoreModuleVariables
    { get => m_coreModuleVariables; }

    public IModuleDamage.OnHealEvent OnHealEvent = new();
    public IModuleDamage.OnDamageEvent OnDamageEvent = new();

    /// <summary>
    /// Attempts to initialize a connection between this module and a CoreShipModuleManager.
    /// If the module already has a manager assigned, no action is taken.
    /// Otherwise, it searches for a CoreShipModuleManager component attached to the module's parent object
    /// and attempts to link with it if found.
    /// </summary>
    public void IntializeToManager()
    {
        //if it has a manager, we dont attempt to reassign
        if (m_shipModuleManager != null)
        {
            return;
        }
        //attempt assignment
        //get the parent
        Transform parentObject = gameObject.transform.parent;
        //attempt the search for the manager
        CoreShipModuleManager manager = parentObject.GetComponent<CoreShipModuleManager>();
        //if found, use register method
        if (manager != null)
        {
            AttemptToLinkManager(manager);
        }
    }

    /// <summary>
    /// Attempts to link this module with a provided CoreShipModuleManager.
    /// If the module already has a manager assigned, no action is taken.
    /// Otherwise, it registers the module with the provided manager and stores the manager reference.
    /// </summary>
    /// <remarks>
    /// This is an internal call from the <see cref="RegisterCoreModuleManager(CoreShipModuleManager)"/> to couple the manager with the module on initialization
    /// </remarks>
    /// <param name="currentManager">The CoreShipModuleManager to link with.</param>
    internal void AttemptToLinkManager(CoreShipModuleManager currentManager)
    {
        //if it has a manager, we dont attempt to reassign
        if (m_shipModuleManager != null)
        {
            return;
        }
        if (currentManager != null)
        {
            //confirm manager link goes both ways
            currentManager.AddModule(this);
            m_shipModuleManager = currentManager;
        }
    }

}

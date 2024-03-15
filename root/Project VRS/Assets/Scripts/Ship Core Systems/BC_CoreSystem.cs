using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class BC_CoreSystem : MonoBehaviour, ICoreSystem, ICoreSystemBehavior, ISystemDamage
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

    public virtual void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
    public virtual void RegisterCoreSystemManager(CoreShipSystemManager currentManager)
    {
        m_shipModuleManager = currentManager;
    }

    public virtual void DeregisterCoreSystemManager(CoreShipSystemManager currentManager)
    {
        m_shipModuleManager = null;
    }

    #endregion

    //we need at least some basic imlementation for some of these methods, like the registering of the manager
    //for this, we do need to define some varaibles. they can be serialized so we can see them but they should be private

    [SerializeField] private CoreShipSystemManager m_shipModuleManager;
    public CoreShipSystemManager ShipModuleManager
    { get => m_shipModuleManager; }

    [SerializeField] private SO_CoreSystem m_coreSystemVariables;
    public SO_CoreSystem CoreSystemVariables
    { get => m_coreSystemVariables; }

    internal virtual void Awake()
    {
        //attempt link
        AttemptToLinkManagerInParent();
    }

    private void AttemptToLinkManagerInParent()
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
        CoreShipSystemManager manager = parentObject.GetComponent<CoreShipSystemManager>();
        //if found, use register method
        if (manager != null)
        {
            RegisterCoreSystemManager(manager);
        }
    }

}


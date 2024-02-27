using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public virtual void RegisterCoreSystemManager(CoreShipSystemManager currentManager)
    {
        throw new System.NotImplementedException();
    }

    public virtual void DeregisterCoreSystemManager(CoreShipSystemManager currentManager)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool AllocateResources(ICoreSystem.SystemResourceRequirement resources, Vector3Int amount)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool CheckResourceAvailability(ICoreSystem.SystemResourceRequirement resources, Vector3Int amount)
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

    #endregion

    //we need at least some basic imlementation for some of these methods, like the registering of the manager
    //for this, we do need to define some varaibles. they can be serialized so we can see them but they should be private

    private CoreShipSystemManager m_shipModuleManager;
    public CoreShipSystemManager ShipModuleManager
    { get => m_shipModuleManager; }


}

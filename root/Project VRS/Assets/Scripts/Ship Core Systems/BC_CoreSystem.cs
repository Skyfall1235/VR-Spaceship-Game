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
        m_shipModuleManager = currentManager;
    }

    public virtual void DeregisterCoreSystemManager(CoreShipSystemManager currentManager)
    {
        m_shipModuleManager = null;
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

public class SystemResourceQueue
{
    public int EngineVal
    {
        get => Engines.Count;
    }
    public int InternalSystemsVal
    {
        get => InternalSystems.Count;
    }
    public int WeaponsVal
    {
        get => Weapons.Count;
    }

    Stack<int> Engines         = new(5);
    Stack<int> InternalSystems = new(5);
    Stack<int> Weapons         = new(5);

    Stack<int> Items = new(7);

    public enum PipSelection
    { 
        Engine,
        internalSystems,
        Weapons
    }

    public void AddToList(PipSelection array)
    {

    }

    public void RemoveFromList(PipSelection array) 
    { 

    }

    private bool HasCapacity(PipSelection array)
    {
        switch (array)
        {
            case PipSelection.Engine:
                if (Engines.Count >= 5) { return false; }
                break;

            case PipSelection.internalSystems:
                if (InternalSystems.Count >= 5) { return false; }
                break;

            case PipSelection.Weapons:
                if (Weapons.Count >= 5) { return false; }
                break;
        }
        return true;
    }

    //called when the item has capacity but there are none in the stack
    private void ReallocateOldestResource()
    {
        //rules: take fro mthe leftmost item possible, take from the
    }


}

using UnityEngine;

[RequireComponent(typeof(InternalModuleHealth))]
public class BC_CoreModule : MonoBehaviour, ICoreModule, ICoreModuleBehavior, IModuleDamage
{

    #region Variables

    [Header("Core States and Behavior Events")]

    /// <summary>
    /// The ship module manager that is in charge of this module
    /// </summary>
    [SerializeField] private CoreShipModuleManager m_shipModuleManager;
    public CoreShipModuleManager ShipModuleManager
    { get => m_shipModuleManager; }

    [SerializeField]
    protected InternalModuleHealth m_internalModuleHealth;


    //I NEED TO FIND A WAY TO TRIGGER UNITY EVENTS WHEN THESE CHANGE, OR AT LEAST CHANGE THEM IN CODE SOMEWHERE ELSE
    //bruh.

    /// <summary>
    /// Sets the current CoreModuleState of the system.
    /// </summary>
    [SerializeField]
    protected ICoreModule.CoreModuleState m_coreState;
    public ICoreModule.CoreModuleState CoreModuleState
    { get => m_coreState; }

    /// <summary>
    /// Represents the current operational state of the system, such as Active, Preparing, ReadyForUse, Damaged, or Rebooting.
    /// </summary>
    [SerializeField]
    protected ICoreModule.ModuleOperationalState m_operationalState;
    public ICoreModule.ModuleOperationalState OperationalState
    { get => m_operationalState; }


    /// <summary>
    /// Represents the resource requirement that this module requires.
    /// </summary>
    [SerializeField]
    public ICoreModule.ModuleResourceRequirement m_resourceRequirement;

    #endregion

    #region Base Class unity Events

    /// <summary>
    /// An event that is raised whenever the CoreState of the system changes.
    /// </summary>
    public ICoreModule.OnCoreModuleStateChange m_onCoreModuleStateChange = new();

    /// <summary>
    /// An event that is raised whenever the operational state of the system changes.
    /// </summary>
    public ICoreModule.OnModuleOperationalStateChange m_onModuleOperationalStateChange = new();

    /// <summary>
    /// An event that is raised whenever the module recieves healing, along with information of how much it is healed by and at what rate.
    /// </summary>
    public IModuleDamage.OnHealEvent OnHealEvent = new();

    /// <summary>
    /// An event that is raised whenever the module recieves damage, along with the information on how much damage it took.
    /// </summary>
    public IModuleDamage.OnDamageEvent OnDamageEvent = new();

    /// <summary>
    /// An event that is raised whenever the module dies due to a lack of health
    /// </summary>
    public IModuleDamage.OnDeathEvent OnDeathEvent = new();

    #endregion

    #region Intitialization to Manager

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
    /// Assumes the module is ready to initialize with a provided CoreShipModuleManager.
    /// If the module already has a manager assigned, no action is taken.
    /// Otherwise, it registers the module with the provided manager and stores the manager reference.
    /// </summary>
    /// <remarks>
    /// This is an internal call from the <see cref="RegisterCoreModuleManager(CoreShipModuleManager)"/> to couple the manager with the module on initialization
    /// </remarks>
    /// <param name="currentManager">The CoreShipModuleManager to link with.</param>
    public void AttemptToLinkManager(CoreShipModuleManager currentManager)
    {
        //if it has a manager, we dont attempt to reassign
        if (m_shipModuleManager != null)
        {
            return;
        }
        if (currentManager != null)
        {
            //confirm manager link goes both ways
            currentManager.AddSingleModule(this);
            m_shipModuleManager = currentManager;
        }
    }

    #endregion

    #region Virtual Methods for Setup and Usage

    public virtual void InitializeModule()
    {
        m_internalModuleHealth.InitializeHealth();
    }

    //not in use yet so dont touch it
    public virtual void ReleaseResources()
    {
        throw new System.NotImplementedException();
    }

    public virtual void ShutDown()
    {
        throw new System.NotImplementedException();
    }

    public virtual void StartUp()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Reboot()
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

    #region Health Management

    /// <summary>
    /// Applies damage to this module based on the provided damage data.
    /// </summary>
    /// <param name="damageData">The weapon collision data containing damage information.</param>
    public void TakeDamage(IDamageData.WeaponCollisionData damageData)
    {
        //allow the health script to handle the actual number stuff and then invoke the event
        m_internalModuleHealth.TakeDamage(damageData);
        OnDamageEvent.Invoke(damageData, this);
    }

    /// <summary>
    /// Heals this module based on the provided heal data.
    /// </summary>
    /// <param name="healData">The heal module data containing healing information.</param>
    public void HealModule(IDamageData.HealModuleData healData)
    {
        m_internalModuleHealth.HealModule(healData);
        OnHealEvent.Invoke(healData, this);
    }

    #endregion

}

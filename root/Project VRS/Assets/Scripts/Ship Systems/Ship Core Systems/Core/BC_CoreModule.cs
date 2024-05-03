using UnityEngine;
using System.Collections;
using static SystemResourceQueue;

/// <summary>
/// Abstract base class representing a core module within a ship system. This class defines core functionalities, properties, and events related to a module's state, health, and management.
/// </summary>
/// <remarks>
/// <para>
/// * **State Management:**
///     * Tracks the module's overall state (e.g., Starting Up, Running, Shutting Down) through an interface (ICoreModule.CoreModuleState). This state can change and trigger the `OnCoreModuleStateChange` event.
///     * Tracks the module's operational state (e.g., Active, Idle, Offline) through an interface (ICoreModule.ModuleOperationalState). This state can represent operational readiness and trigger the `OnModuleOperationalStateChange` event.
/// </para>
/// <para>
/// * **Health Management:**
///     * Provides an `InternalModuleHealth` component for handling health changes (damage and healing).
///     * Exposes methods for taking damage (`TakeDamage`) and receiving healing (`HealModule`).
///     * Triggers `OnDamageEvent` and `OnHealEvent` (with `ModuleStateChangeType.Health`) upon health changes.
/// </para>
/// <para>
/// * **Manager Registration:**
///     * Allows registering with a `CoreShipModuleManager` using `RegisterCoreModuleManager`. This establishes a link between the module and the manager for potential coordination.
///     * Provides `AttemptToLinkManager` for internal use during initialization to link with a provided manager.
///     * Offers virtual methods for `DeregisterCoreSystemManager` to break the link with the manager if needed.
/// </para>
/// <para>
/// * **Initialization and Virtual Methods:**
///     * Defines an `InitializeModule` method for potential module-specific initialization (currently calls `m_internalModuleHealth.InitializeHealth`).
///     * Provides abstract methods for core functionalities:
///         * `ReleaseResources`: Needs implementation for releasing resources when a module is no longer needed.
///         * `ShutDown`: Requires implementation for handling module shutdown procedures.
///         * `StartUp`: Requires implementation for handling module startup procedures.
///         * `Reboot`: Requires implementation for handling module reboot procedures.
/// </para>
/// <para>
/// * **Events:**
///     * `OnCoreModuleStateChange`: Triggered when the module's overall state changes.
///     * `OnModuleOperationalStateChange`: Triggered when the module's operational state changes.
///     * `OnDamageEvent`: Triggered when the module takes damage.
///     * `OnHealEvent`: Triggered when the module receives healing.
///     * `OnDeathEvent`: Potentially intended to be triggered when the module is destroyed due to health depletion .
/// </para>
/// <para>
/// **Inheritance:**
/// This class is marked as abstract, meaning it cannot be directly instantiated. Concrete core module classes should inherit from `BC_CoreModule` and implement the abstract methods to provide specific functionalities.
/// Abstract base class representing a core module within a ship system. This class defines core functionalities, properties, and events related to a module's state, health, and management.
/// </para>
/// </remarks>
[RequireComponent(typeof(InternalModuleHealth))]
public abstract class BC_CoreModule : MonoBehaviour, ICoreModule, ICoreModuleBehavior, IModuleDamage
{

    #region Variables

    [Header("Core States and Behavior Events")]

    /// <summary>
    /// The ship module manager that is in charge of this module
    /// </summary>
    [SerializeField] private CoreShipModuleManager m_shipModuleManager;
    public CoreShipModuleManager ShipModuleManager
    { get => m_shipModuleManager; }

    protected InternalModuleHealth m_internalModuleHealth;
    public InternalModuleHealth InternalModuleHealth
    {
        get
        {
            if(m_internalModuleHealth != null)
            {
                return m_internalModuleHealth;
            }
            m_internalModuleHealth = GetComponent<InternalModuleHealth>();
            return m_internalModuleHealth;
        }
    }

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

    [Header("Energy Management")]

    /// <summary>
    /// The percent boost this module gets to apply to its functions.
    /// </summary>
    [SerializeField]
    protected int m_boostPercent = 0;
    public int BoostPercent
    {
        get => m_boostPercent;
        set => m_boostPercent = value;
    }

    /// <summary>
    /// The power requirement for this module.
    /// </summary>
    [SerializeField]
    protected int m_powerRequirement;
    public int PowerRequirements
    {
        get => m_powerRequirement;
    }

    ///<summary>
    /// The priory of this module in the energy system.
    /// </summary>
    /// <remarks>
    /// The lower the value, the higher the priority to keep power in the case of an outage.
    /// </remarks>
    [SerializeField]
    protected int m_moduleEnergyPriorty = 0;
    public int ModuleEnergyPriority
    {
        get => m_moduleEnergyPriorty;
    }

    [SerializeField]

    protected PipSelection m_systemType = PipSelection.internalSystems; 
    public PipSelection SystemType
    {
        get => m_systemType;
    }

    [Header("Delays for State Changes")]
    [SerializeField]
    protected float m_startUpDelay = 1f;

    [SerializeField]
    protected float m_shutDownDelay = 1f;

    [SerializeField]
    protected float m_rebootDelay = 1f;


    #endregion

    #region Base Class unity Events

    /// <summary>
    /// An event that is raised whenever the CoreState of the system changes.
    /// </summary>
    public ICoreModule.OnCoreModuleStateChange OnCoreModuleStateChange = new();

    /// <summary>
    /// An event that is raised whenever the operational state of the system changes.
    /// </summary>
    public ICoreModule.OnModuleOperationalStateChange OnModuleOperationalStateChange = new();

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

    #region Setup

    public virtual void Awake()
    {
        InitializeModule();
    }

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
        m_internalModuleHealth = GetComponent<InternalModuleHealth>();
        m_internalModuleHealth.InitializeHealth();
    }

    #region Start up, restart, and shut down logic
    public virtual void ShutDown()
    {
        StartCoroutine(ShutDownRoutine(false));
    }

    public virtual void StartUp()
    {
        StartCoroutine(StartUpRoutine());
    }

    public virtual void Reboot()
    {
        StartCoroutine(RebootRoutine());
    }

    #region pre and post logic for state changes

    protected virtual void PreStartUpLogic()
    {

    }

    protected virtual void PostStartUpLogic()
    {

    }

    protected virtual void PreShutDownLogic()
    {

    }

    protected virtual void PostShutDownLogic()
    {

    }

    #endregion

    IEnumerator StartUpRoutine()
    {
        //do any logic before the module turns on
        PreStartUpLogic();

        //set as preparing for both the core module state and the operational state
        m_operationalState = ICoreModule.ModuleOperationalState.Preparing;
        OnModuleOperationalStateChange.Invoke(m_operationalState, this, ICoreModule.ModuleStateChangeType.OperationalState);

        m_coreState = ICoreModule.CoreModuleState.Disabled;
        OnCoreModuleStateChange.Invoke(m_coreState, this, ICoreModule.ModuleStateChangeType.CoreState);

        //wait
        yield return new WaitForSeconds(m_startUpDelay);

        //do post start up logic
        PostStartUpLogic();

        m_coreState = ICoreModule.CoreModuleState.Operational;
        OnCoreModuleStateChange.Invoke(m_coreState, this, ICoreModule.ModuleStateChangeType.CoreState);

        m_operationalState = ICoreModule.ModuleOperationalState.Active;
        OnModuleOperationalStateChange.Invoke(m_operationalState, this, ICoreModule.ModuleStateChangeType.OperationalState);
    }

    IEnumerator ShutDownRoutine(bool isRebooting)
    {
        //logic before the shut down
        PreShutDownLogic();

        //set as preparing for both the core module state and the operational state
        m_operationalState = ICoreModule.ModuleOperationalState.Preparing;
        OnModuleOperationalStateChange.Invoke(m_operationalState, this, ICoreModule.ModuleStateChangeType.OperationalState);

        m_coreState = ICoreModule.CoreModuleState.Standby;
        OnCoreModuleStateChange.Invoke(m_coreState, this, ICoreModule.ModuleStateChangeType.CoreState);

        //wait
        yield return new WaitForSeconds(m_shutDownDelay);

        //call post process logic
        PostShutDownLogic();

        m_coreState = ICoreModule.CoreModuleState.Disabled;
        OnCoreModuleStateChange.Invoke(m_coreState, this, ICoreModule.ModuleStateChangeType.CoreState);

        //reboot notifiers
        if (isRebooting)
        {
            m_operationalState = ICoreModule.ModuleOperationalState.Rebooting;
            OnModuleOperationalStateChange.Invoke(m_operationalState, this, ICoreModule.ModuleStateChangeType.OperationalState);
        }
    }

    IEnumerator RebootRoutine()
    {
        //shutdowm then start up
        StartCoroutine(ShutDownRoutine(true));
        yield return new WaitForSeconds(m_rebootDelay);
        StartCoroutine(StartUpRoutine());
    }


    #endregion

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
        OnDamageEvent.Invoke(damageData, this, ICoreModule.ModuleStateChangeType.Health);
    }

    /// <summary>
    /// Heals this module based on the provided heal data.
    /// </summary>
    /// <param name="healData">The heal module data containing healing information.</param>
    public void HealModule(IDamageData.HealModuleData healData)
    {
        m_internalModuleHealth.HealModule(healData);
        OnHealEvent.Invoke(healData, this, ICoreModule.ModuleStateChangeType.Health);
    }

    #endregion

}

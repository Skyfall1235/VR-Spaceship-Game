using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BC_Weapon : MonoBehaviour
{

    #region Variables
    /// <summary>
    /// Minimum time in seconds between firing the weapon.
    /// </summary>
    public float m_minimumTimeBetweenFiring { get; protected set; } = 1;

    /// <summary>
    /// If the guns scriptable object uses a magazine, this represents the amount of ammo currently in the magazine.
    /// </summary>
    [SerializeField]
    uint m_currentMag = 0;
    int CurrentMag
    {
        get 
        {
            return (int) m_currentMag; 
        }
        set 
        {
            if(value <= 0)
            {
                m_currentMag = 0;
                if(CurrentWeaponState != WeaponState.Reloading)
                {
                    TryReload();
                }
            }
            else
            {
                m_currentMag = (uint)value;
            }
        }
    }
    uint m_magCapacity = 0;

    bool m_currentWeaponFiringState = false;
    /// <summary>
    /// Transform representing the base of the weapon where projectiles are instantiated.
    /// </summary>
    [field: SerializeField] 
    public Transform WeaponBase { get; protected set; }

    /// <summary>
    /// Reference to the attached SO_WeaponData scriptable object.
    /// </summary>
    [SerializeField]
    protected SO_WeaponData m_weaponData;
    public SO_WeaponData WeaponData
    {
        get 
        { 
            return m_weaponData;
        }
    }

    /// <summary>
    /// GameObject representing the point where projectiles are instantiated.
    /// </summary>
    [SerializeField]
    protected GameObject m_instantiationPoint;
    public GameObject InstantiationPoint
    {
        get 
        { 
            return m_instantiationPoint; 
        }
    }

    /// <summary>
    /// Gets or sets the current weapon state.
    /// </summary>
    public WeaponState CurrentWeaponState = WeaponState.Ready;

    
    /// <summary>
    /// UnityEvent triggered whenever the weapon's state changes.
    /// This event provides the new weapon state as an argument.
    /// </summary>
    public UnityEvent<WeaponState> OnChangeWeaponState = new UnityEvent<WeaponState>();

    /// <summary>
    /// UnityEvent triggered whenever the weapon performs an action.
    /// This event provides the specific weapon action as an argument.
    /// </summary>
    public UnityEvent<WeaponAction> OnWeaponAction = new UnityEvent<WeaponAction>();

    private BC_FireType m_FireType;

    #endregion

    #region Data Structures

    /// <summary>
    /// Represents the possible states of a weapon.
    /// </summary>
    public enum WeaponState
    {
        /// <summary>
        /// The weapon is preparing to fire.
        /// </summary>
        Preparing,
        /// <summary>
        /// The weapon is reloading.
        /// </summary>
        Reloading,
        /// <summary>
        /// The weapon is ready to fire.
        /// </summary>
        Ready,
        /// <summary>
        /// The weapon is disabled and cannot fire.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// The possible actions a weapon can perform.
    /// </summary>
    public enum WeaponAction
    {
        /// <summary>
        /// The weapon is not performing any action.
        /// </summary>
        None,

        /// <summary>
        /// The weapon is currently firing.
        /// </summary>
        Firing,

        /// <summary>
        /// The weapon is currently reloading.
        /// </summary>
        Reloading
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the weapon based on the attached WeaponData scriptable object.
    /// </summary>
    protected virtual void Awake()
    {
        //setup weapon data
        if (m_weaponData != null)
        {
            m_minimumTimeBetweenFiring = m_weaponData.MinimumTimeBetweenFiring;
            if (WeaponData.UsesMag)
            {
                m_magCapacity = (uint)WeaponData.MagazineCapacity;
                m_currentMag = m_magCapacity;
            }
            //setup firing mode
            if(m_weaponData.WeaponFiringMode != null) 
            {
                m_FireType = (BC_FireType)Activator.CreateInstance(m_weaponData.WeaponFiringMode, new object[] {
                    this,
                    (BC_FireType.Fire)OnFire,
                    (BC_FireType.StartFire)OnStartFire,
                    (BC_FireType.StopFire)OnStopFire,
                    (WeaponData.UsesMag) ? (BC_FireType.PostFire)OnPostFire + (() => CurrentMag--) : OnPostFire,
                    (BC_FireType.PreFire)OnPreFire,
                });
            }
            else
            {
                Debug.LogError("No Fire Type setup in Scriptable Object");
            }
        }
        else
        {
            throw new System.Exception("No scriptable object found");
        }
        
    }

    /// <summary>
    /// Updates the weapon's firing state and attempts to fire if necessary.
    /// </summary>
    /// <param name="newFiringState">The new firing state (true for firing, false for not firing).</param>
    public void UpdateFiringState(bool newFiringState)
    {
        if(CurrentWeaponState != WeaponState.Reloading && m_FireType != null)
        {
            m_FireType.UpdateFiringState(newFiringState);
        }
        m_currentWeaponFiringState = newFiringState;
    }

    
    //Methods to override for individualized functionaility
    public virtual void OnStartFire()
    {

    }
    public virtual void OnStopFire()
    {

    }
    public virtual void OnPreFire()
    {

    }
    public virtual void OnPostFire()
    {
        
    }
    public abstract void OnFire();

    #endregion
    /// <summary>
    /// checks if the weapon uses a magazine and reloads tries to run the reload coroutine if it does
    /// </summary>
    public virtual void TryReload()
    {
        if (WeaponData.UsesMag)
        {
            StartCoroutine(ReloadAsync());
        }
    }
    /// <summary>
    /// Changes the firing state of the weapon then reloads it and resets the firing state to the desired value from input
    /// </summary>
    /// <returns>Nothing</returns>
    IEnumerator ReloadAsync()
    {
        m_FireType.UpdateFiringState(false);
        CurrentWeaponState = WeaponState.Reloading;
        yield return new WaitForSeconds((float)WeaponData.ReloadTime);
        m_currentMag = m_magCapacity;
        CurrentWeaponState = WeaponState.Ready;
        if (m_currentWeaponFiringState)
        {
            m_FireType.UpdateFiringState(m_currentWeaponFiringState);
        }
    }
}
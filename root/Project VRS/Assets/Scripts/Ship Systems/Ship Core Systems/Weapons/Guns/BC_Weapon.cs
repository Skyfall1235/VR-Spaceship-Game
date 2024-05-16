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
    int m_currentAmountInMagazine = 0;
    public int CurrentAmountInMagazine
    {
        get
        {
            return m_currentAmountInMagazine;
        }
        set
        {
            if(m_weaponData.MagazineCapacity != null)
            {
                m_currentAmountInMagazine = Mathf.Clamp(value, 0, (int)m_weaponData.MagazineCapacity);
                if(m_currentAmountInMagazine <= 0)
                {
                    Reload();
                }
            }
            else
            {
                m_currentAmountInMagazine = 0;
                Debug.LogError($"attempting to write to current amount in magazine with null in object: {this.gameObject.name}");
            } 
        }
    }

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
        if (m_weaponData != null)
        {
            m_minimumTimeBetweenFiring = m_weaponData.MinimumTimeBetweenFiring;
        }
        else
        {
            throw new System.Exception("No scriptable object found");
        }

        switch (m_weaponData.WeaponFiringMode)
        {
            case (SO_WeaponData.FiringMode.Auto):

                if (m_weaponData.UsesMag)
                {
                    m_FireType = new AutomaticFire(this, () => OnStartFire(), () => OnEndFire(), () => OnFire(), () => CurrentAmountInMagazine -= 1, () => Debug.Log("should call this param"));
                }
                else
                {
                    m_FireType = new AutomaticFire(this, () => OnStartFire(), () => OnEndFire(), () => OnFire());
                }
                
                break;
            case (SO_WeaponData.FiringMode.SemiAuto):

                if (m_weaponData.UsesMag)
                {
                    m_FireType = new SemiAutomaticFire(this, () => OnStartFire(), () => OnEndFire(), () => OnFire(), () => CurrentAmountInMagazine--);
                }
                else
                {
                    m_FireType = new SemiAutomaticFire(this, () => OnStartFire(), () => OnEndFire(), () => OnFire());
                }
                break;
            case(SO_WeaponData.FiringMode.Beam):

                if (m_weaponData.UsesMag)
                {
                    m_FireType = new BeamFire(this, () => OnStartFire(), () => OnEndFire(), () => OnFire(), () => CurrentAmountInMagazine--);
                }
                else
                {
                    m_FireType = new BeamFire(this, () => OnStartFire(), () => OnEndFire(), () => OnFire());
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the weapon's firing state and attempts to fire if necessary.
    /// </summary>
    /// <param name="newFiringState">The new firing state (true for firing, false for not firing).</param>
    public void UpdateFiringState(bool newFiringState)
    {
        m_FireType.UpdateFiringState(newFiringState);
    }

    public virtual void Reload()
    {
        if (m_weaponData.UsesMag == false)
        {
            Debug.Log($"{this.gameObject.name} does not use a magazine");
            return;
        }
        Debug.Log("called reload");
        StartCoroutine(OnReload());
    }

    /// <summary>
    /// Attempts to reload the weapon if it's currently in the Ready state.
    /// </summary>
    protected virtual IEnumerator OnReload()
    {
        // Check if the weapon is ready to be reloaded
        if (CurrentWeaponState == WeaponState.Ready)
        {
            Debug.Log("Reload seq 1");
            // Initiate reload sequence
            CurrentWeaponState = WeaponState.Reloading;
            OnChangeWeaponState.Invoke(CurrentWeaponState); // Announce reloading state change

            // Calculate amount to reload (magazine capacity - current ammo)
            int amountToReload = (int)m_weaponData.MagazineCapacity - CurrentAmountInMagazine;
            Debug.Log("Reload seq 2");
            if (amountToReload > 0)
            {
                Debug.Log("there is ammo to reload");
                Debug.Log($"reloading {amountToReload}");
                // Notify of reload action (optional for visual/sound cues)
                OnWeaponAction.Invoke(WeaponAction.Reloading);

                // Simulate reload delay
                yield return new WaitForSeconds((float)m_weaponData.ReloadTime);

                // Update ammo and state
                CurrentAmountInMagazine += amountToReload;
            }
            Debug.Log("Reload seq 3");

            // Revert to ready state and announce
            CurrentWeaponState = WeaponState.Ready;
            OnChangeWeaponState.Invoke(CurrentWeaponState);
        }
        else
        {
            // Do nothing if not in Ready state
            yield return null;
        }
    }

    public virtual void UseSingleAmmo()
    {
        
    }

    public virtual void OnStartFire()
    {

    }
    public virtual void OnEndFire()
    {

    }
    public abstract void OnFire();

    #endregion


}
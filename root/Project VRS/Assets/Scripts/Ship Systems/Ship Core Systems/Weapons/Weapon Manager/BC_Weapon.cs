using System.Collections;
using UnityEngine;

public abstract class BC_Weapon : MonoBehaviour
{

    #region Variables

    /// <summary>
    /// Gets or sets whether the weapon fires automatically.
    /// </summary>
    public bool IsAutomatic { get; protected set; } = false;
    /// <summary>
    /// Minimum time in seconds between firing the weapon.
    /// </summary>
    protected float m_minimumTimeBetweenFiring = 1;
    /// <summary>
    /// Internal flag indicating if the weapon is currently firing.
    /// </summary>
    bool m_currentFiringState = false;

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
    public WeaponState CurrentWeaponState { get; protected set; } = WeaponState.Ready;

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
    }

    /// <summary>
    /// Updates the weapon's firing state and attempts to fire if necessary.
    /// </summary>
    /// <param name="newFiringState">The new firing state (true for firing, false for not firing).</param>
    public void UpdateFiringState(bool newFiringState)
    {
        if(newFiringState == true && m_currentFiringState == false)
        {
            StartCoroutine(TryFire());
        }
        m_currentFiringState = newFiringState;
    }

    /// <summary>
    /// This method reloads the weapon. 
    /// </summary>
    protected abstract void Reload();

    /// <summary>
    /// Attempts to fire the weapon if it is ready.
    /// </summary>
    /// <returns>An IEnumerator object for controlling the firing sequence.</returns>
    protected virtual IEnumerator TryFire()
    {
        if(CurrentWeaponState == WeaponState.Ready)
        {
            Fire();
            CurrentWeaponState = WeaponState.Preparing;
            yield return new WaitForSeconds(m_minimumTimeBetweenFiring);
            CurrentWeaponState = WeaponState.Ready;
            if (IsAutomatic && m_currentFiringState == true)
            {
                StartCoroutine(TryFire());
            }
        }
        else
        {
            yield return null;
        }
    }

    /// <summary>
    /// This method fires the weapon.
    /// </summary>
    protected abstract void Fire();

    #endregion

}
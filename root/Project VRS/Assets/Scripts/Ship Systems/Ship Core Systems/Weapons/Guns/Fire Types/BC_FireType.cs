using System;
using UnityEngine;

public abstract class BC_FireType
{
    /// <summary>
    /// Void methods fired when the weapon starts firing
    /// </summary>
    public delegate void StartFire();
    /// <summary>
    /// Void methods fired when the weapon stops firing
    /// </summary>
    public delegate void StopFire();
    /// <summary>
    /// Void methods fire when the weapon fires
    /// </summary>
    public delegate void Fire();

    protected StartFire m_startFireMethods;
    protected StopFire m_stopFireMethods;
    protected Fire m_fireMethods;
    protected BC_Weapon m_weapon;

    protected bool m_currentFiringState = false;
    /// <summary>
    /// Method updates the underlying fire state of the weapon
    /// </summary>
    /// <param name="newFiringState">The new firing state of the weapon</param>
    virtual public void UpdateFiringState(bool newFiringState)
    {
        if (m_currentFiringState == false && newFiringState == true)
        {
            m_startFireMethods?.Invoke();
        }
        if (m_currentFiringState == true && newFiringState == false)
        {
            m_stopFireMethods?.Invoke();
        }
        m_currentFiringState = newFiringState;
    }
    /// <summary>
    /// Creates a new BC_FireType
    /// </summary>
    /// <param name="fireAction">Void methods fired when the weapon starts firing</param>
    /// <param name="weapon">Monobehavior that the coroutines will run on</param>
    /// <param name="startFireAction">Void methods fired when the weapon stops firing</param>
    /// <param name="stopFireAction">Void methods fired when the weapon stops firing</param>
    public BC_FireType(BC_Weapon weapon, StartFire startFireAction = null, StopFire stopFireAction = null, Fire[] fireActions = null) 
    {
        foreach (Fire fireAction in fireActions)
        {
            m_fireMethods += fireAction;
        }
        m_startFireMethods += startFireAction;
        m_stopFireMethods += stopFireAction;
        m_weapon = weapon;
    }
}
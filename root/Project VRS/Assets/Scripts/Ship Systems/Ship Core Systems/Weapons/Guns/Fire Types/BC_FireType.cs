using System;
using System.Collections;
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
    public delegate void PreFire();
    public delegate void PostFire();

    protected StartFire m_startFireMethods;
    protected StopFire m_stopFireMethods;
    protected Fire m_fireMethods;
    protected PreFire m_preFireMethods;
    protected PostFire m_postFireMethods;

    protected BC_Weapon m_weapon;

    public bool m_currentFiringState { get; protected set; } = false;
    /// <summary>
    /// Method updates the underlying fire state of the weapon
    /// </summary>
    /// <param name="newFiringState">The new firing state of the weapon</param>
    virtual public void UpdateFiringState(bool newFiringState)
    {
        if (newFiringState == true)
        {
            m_startFireMethods?.Invoke();
        }
        if (newFiringState == false)
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
    public BC_FireType(BC_Weapon weapon, Fire mainFireAction, StartFire startFireAction = null, StopFire stopFireAction = null, PostFire postFireAction = null, PreFire preFireAction = null) 
    {
        m_postFireMethods += postFireAction;
        m_preFireMethods += preFireAction;
        m_fireMethods += mainFireAction;
        m_startFireMethods += startFireAction + StartFireLogic;
        m_stopFireMethods += stopFireAction;
        m_weapon = weapon;
    }
    /// <summary>
    /// Starts the TryFireLogicAsync Coroutine
    /// </summary>
    void StartFireLogic()
    {
        m_weapon.StartCoroutine(TryFireLogicAsync());
    }
    /// <summary>
    /// Logic for triggering when each weapon is fired
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerator TryFireLogicAsync();

}
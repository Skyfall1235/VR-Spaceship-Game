using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticFire : BC_FireType
{
    /// <summary>
    /// Creates a new AutomaticFire
    /// </summary>
    /// <param name="fireAction">Void methods fired when the weapon starts firing</param>
    /// <param name="weapon">Monobehavior that the coroutines will run on</param>
    /// <param name="startFireAction">Void methods fired when the weapon stops firing</param>
    /// <param name="stopFireAction">Void methods fired when the weapon stops firing</param> 
    public AutomaticFire
        (
            BC_Weapon weapon,
            StartFire startFireAction = null,
            StopFire stopFireAction = null,
            params Fire[] fireActions
        ) : base(weapon, startFireAction, stopFireAction, fireActions)
    {
        m_startFireMethods += StartTryFire;
    }
    void StartTryFire()
    {
        m_weapon.StartCoroutine(TryFireLogicAsync());
    }
    IEnumerator TryFireLogicAsync()
    {
        if(m_weapon.CurrentWeaponState == BC_Weapon.WeaponState.Ready)
        {
            m_weapon.OnFire();
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Preparing;
            yield return new WaitForSeconds(m_weapon.m_minimumTimeBetweenFiring);
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Ready;
            if(m_currentFiringState == true) 
            {
                m_weapon.StartCoroutine(TryFireLogicAsync());
            }
        }
    }
}

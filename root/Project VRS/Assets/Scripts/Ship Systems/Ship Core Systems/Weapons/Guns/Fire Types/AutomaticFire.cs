using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticFire : BC_FireType
{
    public AutomaticFire
        (
            BC_Weapon weapon,
            Fire fireAction,
            StartFire startFireAction = null,
            StopFire stopFireAction = null,
            PostFire postFireAction = null,
            PreFire preFireAction = null

        ) : base(weapon, fireAction, startFireAction, stopFireAction, postFireAction, preFireAction)
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
            m_preFireMethods?.Invoke();
            m_fireMethods?.Invoke();
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Preparing;
            yield return new WaitForSeconds(m_weapon.m_minimumTimeBetweenFiring);
            if(m_weapon.CurrentWeaponState != BC_Weapon.WeaponState.Preparing)
            {
                yield break;
            }
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Ready;
            m_postFireMethods?.Invoke();
            if (m_currentFiringState == true)
            {
               m_weapon.StartCoroutine(TryFireLogicAsync());
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamFire : BC_FireType
{
    public BeamFire
        (
            BC_Weapon weapon,
            StartFire startFireAction = null,
            StopFire stopFireAction = null,
            params Fire[] fireActions
        ) : base(weapon, startFireAction, stopFireAction, fireActions)
    {

    }
    IEnumerator TryFireLogicAsync()
    {
        if (m_weapon.CurrentWeaponState == BC_Weapon.WeaponState.Ready)
        {
            m_weapon.OnFire();
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Preparing;
            yield return new WaitForFixedUpdate();
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Ready;
            if (m_currentFiringState == true)
            {
                m_weapon.StartCoroutine(TryFireLogicAsync());
            }
        }
    }
}

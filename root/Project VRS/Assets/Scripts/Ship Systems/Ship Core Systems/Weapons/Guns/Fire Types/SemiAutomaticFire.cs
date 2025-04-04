using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutomaticFire : BC_FireType
{
    /// <summary>
    /// Creates a new AutomaticFire
    /// </summary>
    /// <param name="fireAction">Void methods fired when the weapon starts firing</param>
    /// <param name="weapon">Monobehavior that the coroutines will run on</param>
    /// <param name="startFireAction">Void methods fired when the weapon stops firing</param>
    /// <param name="stopFireAction">Void methods fired when the weapon stops firing</param> 
    public SemiAutomaticFire
        (
            BC_Weapon weapon,
            Fire fireAction,
            StartFire startFireAction = null,
            StopFire stopFireAction = null,
            PostFire postFireAction = null,
            PreFire preFireAction = null
        ) : base(weapon, fireAction, startFireAction, stopFireAction, postFireAction, preFireAction)
    {

    }
    protected override IEnumerator TryFireLogicAsync()
    {
        if (m_weapon.CurrentWeaponState == BC_Weapon.WeaponState.Ready)
        {
            m_preFireMethods?.Invoke();
            m_fireMethods?.Invoke();
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Preparing;
            yield return new WaitForSeconds(m_weapon.m_minimumTimeBetweenFiring);
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Ready;
            m_postFireMethods?.Invoke();
        }
    }
}

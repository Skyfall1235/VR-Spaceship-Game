using System.Collections;
using UnityEngine;

public class BeamFire : BC_FireType
{
    public BeamFire
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
            yield return new WaitForFixedUpdate();
            m_weapon.CurrentWeaponState = BC_Weapon.WeaponState.Ready;
            m_postFireMethods?.Invoke();
            if (m_currentFiringState == true)
            {
                m_weapon.StartCoroutine(TryFireLogicAsync());
            }
        }
    }
}

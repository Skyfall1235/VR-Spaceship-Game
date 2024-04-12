using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool IsAutomatic { get; protected set; } = false;
    protected float _minimumTimeBetweenFiring = 1;
    bool _currentFireState = false;
    public enum WeaponState
    {
        Preparing,
        Reloading,
        Ready,
        Disabled
    }
    public WeaponState CurrentWeaponState {  get; protected set; } = WeaponState.Ready;

    public void UpdateFireState(bool newFireState)
    {
        if(newFireState == true && _currentFireState == false)
        {
            StartCoroutine(TryFire());
        }
        _currentFireState = newFireState;
        //Debug.Log(newFireState);
    }
    public abstract void Reload();
    protected virtual IEnumerator TryFire()
    {
        if(CurrentWeaponState == WeaponState.Ready)
        {
            Fire();
            yield return new WaitForSeconds(_minimumTimeBetweenFiring);
            CurrentWeaponState = WeaponState.Ready;
            if (IsAutomatic && _currentFireState == true)
            {
                StartCoroutine(TryFire());
            }
        }
        else
        {
            yield return null;
        }
    }
    protected virtual void Fire()
    {
        CurrentWeaponState = WeaponState.Preparing;
    }
}

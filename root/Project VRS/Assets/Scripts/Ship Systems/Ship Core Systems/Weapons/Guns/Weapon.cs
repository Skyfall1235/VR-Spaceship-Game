using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool IsAutomatic { get; protected set; } = false;
    protected float _minimumTimeBetweenFiring = 1;
    bool _currentFiringState = false;

    [SerializeField]
    protected SO_WeaponData m_weaponData;
    public SO_WeaponData WeaponData
    {
        get 
        { 
            return m_weaponData;
        }
    }
    [SerializeField]
    protected GameObject m_instantiationPoint;
    public GameObject InstantiationPoint
    {
        get 
        { 
            return m_instantiationPoint; 
        }
    }
    public enum WeaponState
    {
        Preparing,
        Reloading,
        Ready,
        Disabled
    }
    public virtual void Awake()
    {
        if(m_weaponData != null) 
        {
            _minimumTimeBetweenFiring = m_weaponData.minimumTimeBetweenFiring;
        }
        else
        {
            throw new System.Exception("No scriptable object found");
        }
    }
    public WeaponState CurrentWeaponState {  get; protected set; } = WeaponState.Ready;
    public void UpdateFiringState(bool newFiringState)
    {
        if(newFiringState == true && _currentFiringState == false)
        {
            StartCoroutine(TryFire());
        }
        _currentFiringState = newFiringState;
        //Debug.Log(newFireState);
    }
    public abstract void Reload();
    protected virtual IEnumerator TryFire()
    {
        if(CurrentWeaponState == WeaponState.Ready)
        {
            Fire();
            CurrentWeaponState = WeaponState.Preparing;
            yield return new WaitForSeconds(_minimumTimeBetweenFiring);
            CurrentWeaponState = WeaponState.Ready;
            if (IsAutomatic && _currentFiringState == true)
            {
                StartCoroutine(TryFire());
            }
        }
        else
        {
            yield return null;
        }
    }
    protected abstract void Fire();
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool IsAutomatic { get; protected set; } = false;
    protected float _minimumTimeBetweenFiring = 1;
    bool _canFireAgain = true;

    public virtual void Awake()
    {

    }
    public void TryFire()
    {
        if (_canFireAgain)
        {
            Fire();
            _canFireAgain = false;
            StartCoroutine(ReenableFiringAfterCooldown());
        }
    }
    protected virtual void Fire()
    {
        throw new System.NotImplementedException();
    }
    public virtual void Reload()
    {
        throw new System.NotImplementedException();
    }
    IEnumerator ReenableFiringAfterCooldown()
    {
        yield return new WaitForSeconds(_minimumTimeBetweenFiring);
        _canFireAgain = true;
    }
}

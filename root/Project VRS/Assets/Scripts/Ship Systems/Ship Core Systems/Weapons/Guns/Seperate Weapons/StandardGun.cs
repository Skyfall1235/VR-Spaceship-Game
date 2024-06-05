using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class StandardGun : PooledWeapon
{
    public override void OnFire()
    {
        base.OnFire();
    }


    protected override void Awake()
    {
        base.Awake();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GatlingGun : PooledWeapon
{
    public override void OnFire()
    {
        base.OnFire();
    }

    protected override void Reload()
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();
    }
}

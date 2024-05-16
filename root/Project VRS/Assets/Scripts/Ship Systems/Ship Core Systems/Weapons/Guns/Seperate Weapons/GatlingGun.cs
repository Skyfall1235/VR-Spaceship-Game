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

    //public override void Reload()
    //{
    //    base.Reload();
    //}

    protected override void Awake()
    {
        base.Awake();
    }
}

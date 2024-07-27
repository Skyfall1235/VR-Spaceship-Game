using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TestingGun : PooledWeapon
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnFire()
    {
        Debug.Log("are we getting here?");
        base.OnFire();
    }
}

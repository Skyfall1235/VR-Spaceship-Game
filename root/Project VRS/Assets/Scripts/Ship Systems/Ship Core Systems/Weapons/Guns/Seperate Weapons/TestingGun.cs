using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TestingGun : PooledWeapon
{
    protected override void Awake()
    {
        base.Awake();
        m_pool = new ObjectPool<GameObject>(OnCreatePooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
    }
    public override void Reload()
    {
        base.Reload();
    }

    public override void OnFire()
    {
        m_pool.Get();
    }
}

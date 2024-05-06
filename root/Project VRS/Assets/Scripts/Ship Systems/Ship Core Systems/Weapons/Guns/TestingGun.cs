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
    protected override void Fire()
    {
        //m_pool.Get();
    }
    protected override void Reload()
    {
        Debug.Log("Reload");
    }
}

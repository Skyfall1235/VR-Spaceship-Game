using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledWeapon : BC_Weapon
{
    [SerializeField]
    protected ObjectPool<GameObject> Pool;

    protected override void Awake()
    {
        base.Awake();
        Pool = new ObjectPool<GameObject>(OnCreatePooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
    }

    protected GameObject OnCreatePooledObject()
    {
        return Instantiate(WeaponData.PrefabBullet, m_instantiationPoint.transform.position, gameObject.transform.rotation);
    }
    protected void OnPulledFromPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if (objectPulled.HasComponent<BasicProjectile>())
        {
            BasicProjectile projectileScript = objectPulled.GetComponent<BasicProjectile>();
            projectileScript.gunThatFiredProjectile = this;
            projectileScript.Setup(m_instantiationPoint.transform.position, transform.rotation);
        }
    }
    protected void OnReturnedToPool(GameObject objectReturned)
    {
        objectReturned.SetActive(false);
    }
    protected void DestroyPooledObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Gun : Weapon
{
    public ObjectPool<GameObject> Pool;
    public override void Awake()
    {
        base.Awake();
        Pool = new ObjectPool<GameObject>(OnCreatePooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
    }
    protected override void Fire()
    {
        Pool.Get();
    }
    public override void Reload()
    {
        Debug.Log("Reload");
    }

    GameObject OnCreatePooledObject()
    {
        return Instantiate(WeaponData.PrefabBullet, m_instantiationPoint.transform.position, gameObject.transform.rotation);
    }
    void OnPulledFromPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if (objectPulled.HasComponent<BasicProjectile>())
        {
            BasicProjectile projectileScript = objectPulled.GetComponent<BasicProjectile>();
            projectileScript.gunThatFiredProjectile = this;
            projectileScript.Setup(m_instantiationPoint.transform.position, transform.rotation);
        }
    }
    void OnReturnedToPool(GameObject objectReturned)
    {
        objectReturned.SetActive(false);
    }
    void DestroyPooledObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}

using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public abstract class PooledWeapon : BC_Weapon
{
    [field:SerializeField]public List<ObjectPool<GameObject>> projectilePools { get; private set; } = new List<ObjectPool<GameObject>>();
    protected override void Awake()
    {
        base.Awake();
        for(uint i = 0; i <= WeaponData.WeaponProjectileData.CalculateDepth(); i++)
        {
            GameObject prefabToInstantiate = WeaponData.WeaponProjectileData.GetDataAtDepth(i).Prefab;
            ObjectPool<GameObject> pool = null;
            pool = new ObjectPool<GameObject>
                (
                    () => OnCreate(prefabToInstantiate, pool),
                    OnPulledFromPool,
                    OnReturnedToPool,
                    DestroyPooledObject
                );
       
            projectilePools.Add
            (
                pool
            );
        }
        //Debug.Log(projectilePools.Count);
    }
    public override void OnFire()
    {
        const int firstProjectileIndex = 0;
        for (uint i = 0; i < m_weaponData.WeaponProjectileData.RootProjectileData.ProjectileCount; i++)
        {
            GameObject firedProjectile = projectilePools[firstProjectileIndex].Get();
            if (firedProjectile.HasComponent<Projectile>())
            {
                firedProjectile.GetComponent<Projectile>().Fire
                (
                    m_instantiationPoint.transform.position,
                    m_instantiationPoint.transform.rotation,
                    m_weaponData.WeaponProjectileData.RootProjectileData,
                    firstProjectileIndex
                );
            }
        }
    }
    
    protected GameObject OnCreate(GameObject prefabToInstantiate, ObjectPool<GameObject> pool = null)
    {
        GameObject newProjectile = Instantiate(prefabToInstantiate);
        if(pool != null && newProjectile.HasComponent<Projectile>())
        {
            newProjectile.GetComponent<Projectile>().poolToReturnTo = pool;
        }
        return newProjectile;
    }
    protected void OnPulledFromPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if(objectPulled.HasComponent<Projectile>()) 
        {
            objectPulled.GetComponent<Projectile>().SetFiringWeapon(this);
        }
    }
    /// <summary>
    /// Called when a GameObject is returned to the object pool.
    /// This method deactivates the object.
    /// </summary>
    /// <param name="objectReturned">
    /// The GameObject that was returned to the object pool.
    /// </param>
    protected void OnReturnedToPool(GameObject objectReturned)
    {
        objectReturned.SetActive(false);
    }

    /// <summary>
    /// Destroys the specified GameObject.
    /// </summary>
    /// <param name="objectToDestroy">
    /// The GameObject to be destroyed.
    /// </param>
    protected void DestroyPooledObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}
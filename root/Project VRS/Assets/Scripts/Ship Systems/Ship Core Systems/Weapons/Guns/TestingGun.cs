using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TestingGun : BC_Weapon
{
    public ObjectPool<GameObject> Pool;
    protected override void Awake()
    {
        base.Awake();
        Pool = new ObjectPool<GameObject>(OnCreatePooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
    }
    protected override void Fire()
    {
        Pool.Get();
    }
    protected override void Reload()
    {
        Debug.Log("Reload");
    }

    /// <summary>
    /// Instantiates a new bullet object from the WeaponData.PrefabBullet prefab.
    /// </summary>
    /// <returns>
    /// The newly instantiated GameObject representing the bullet.
    /// </returns>
    private GameObject OnCreatePooledObject()
    {
        return Instantiate(WeaponData.PrefabBullet, m_instantiationPoint.transform.position, gameObject.transform.rotation);
    }

    /// <summary>
    /// Called when a GameObject is pulled from the object pool.
    /// This method activates the object and sets up its projectile script if it has one.
    /// </summary>
    /// <param name="objectPulled">
    /// The GameObject that was pulled from the object pool.
    /// </param>
    private void OnPulledFromPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if (objectPulled.HasComponent<BasicProjectile>())
        {
            BasicProjectile projectileScript = objectPulled.GetComponent<BasicProjectile>();
            projectileScript.gunThatFiredProjectile = this;
            projectileScript.Setup(m_instantiationPoint.transform.position, transform.rotation);
        }
    }

    /// <summary>
    /// Called when a GameObject is returned to the object pool.
    /// This method deactivates the object.
    /// </summary>
    /// <param name="objectReturned">
    /// The GameObject that was returned to the object pool.
    /// </param>
    private void OnReturnedToPool(GameObject objectReturned)
    {
        objectReturned.SetActive(false);
    }

    /// <summary>
    /// Destroys the specified GameObject.
    /// </summary>
    /// <param name="objectToDestroy">
    /// The GameObject to be destroyed.
    /// </param>
    private void DestroyPooledObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }

}

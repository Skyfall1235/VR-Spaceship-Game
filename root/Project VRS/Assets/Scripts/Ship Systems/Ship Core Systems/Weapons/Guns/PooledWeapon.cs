using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledWeapon : BC_Weapon
{
    [SerializeField]
    protected ObjectPool<GameObject> m_primaryProjectilePool;
    public ObjectPool<GameObject> PrimaryProjectilePool
    {
        get { return m_primaryProjectilePool; }
    }

    protected ObjectPool<GameObject> m_secondaryProjectilePool;
    public ObjectPool<GameObject> SecondaryProjectilePool
    {
        get
        {
            return m_secondaryProjectilePool;
        }
    }

    public override void OnFire()
    {
        m_primaryProjectilePool.Get();
    }

    protected override void Awake()
    {
        base.Awake();
        m_primaryProjectilePool = new ObjectPool<GameObject>(OnCreatePrimaryPooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
        if(m_weaponData.UseProjectile && m_weaponData.ProjectileData.SpawnSubProjectiles)
        {
            m_secondaryProjectilePool = new ObjectPool<GameObject>(OnCreateSecondaryPooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
        }
    }

    /// <summary>
    /// Instantiates a new bullet object from the WeaponData.PrefabBullet prefab.
    /// </summary>
    /// <returns>
    /// The newly instantiated GameObject representing the bullet.
    /// </returns>
    protected GameObject OnCreatePrimaryPooledObject()
    {
        return Instantiate(WeaponData.ProjectileData.ProjectilePrefab, m_instantiationPoint.transform.position, gameObject.transform.rotation);
    }

    /// <summary>
    /// Instantiates a new bullet object from the WeaponData.SubPrefabBullet prefab.
    /// </summary>
    /// <returns>
    /// The newly instantiated GameObject representing the bullet.
    /// </returns>
    protected GameObject OnCreateSecondaryPooledObject()
    {
        return Instantiate(WeaponData.ProjectileData.SubProjectilePrefab, m_instantiationPoint.transform.position, gameObject.transform.rotation);
    }
    /// <summary>
    /// Called when a GameObject is pulled from the object pool.
    /// This method activates the object and sets up its projectile script if it has one.
    /// </summary>
    /// <param name="objectPulled">
    /// The GameObject that was pulled from the object pool.
    /// </param>
    protected void OnPulledFromPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if(objectPulled.HasComponent<Projectile>())
        {
            Projectile projectileComponent = objectPulled.GetComponent<Projectile>();
            projectileComponent.SetProjectileData(m_weaponData.ProjectileData);
            projectileComponent.SetFiringWeapon(this);
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

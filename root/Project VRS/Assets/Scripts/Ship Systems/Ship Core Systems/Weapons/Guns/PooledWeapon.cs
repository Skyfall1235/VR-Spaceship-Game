using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledWeapon : BC_Weapon
{
    [SerializeField]
    public SO_ProjectileData ProjectileData { get; private set;}

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
        m_primaryProjectilePool = new ObjectPool<GameObject>(OnCreatePrimaryPooledObject, OnPulledFromPrimaryPool, OnReturnedToPrimaryPool, DestroyPrimaryPooledObject);
    }

    /// <summary>
    /// Instantiates a new bullet object from the WeaponData.PrefabBullet prefab.
    /// </summary>
    /// <returns>
    /// The newly instantiated GameObject representing the bullet.
    /// </returns>
    protected GameObject OnCreatePrimaryPooledObject()
    {
        //FOR THE LOVE OF GOD GET PROJECTILE CODE OUT OF GUN CODE THEY DONT BELONG
        return Instantiate(WeaponData.ProjectileData.ProjectilePrefab, m_instantiationPoint.transform.position, gameObject.transform.rotation);
    }

    /// <summary>
    /// Called when a GameObject is pulled from the object pool.
    /// This method activates the object and sets up its projectile script if it has one.
    /// </summary>
    /// <param name="objectPulled">
    /// The GameObject that was pulled from the object pool.
    /// </param>
    protected void OnPulledFromPrimaryPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if (objectPulled.HasComponent<Projectile>())
        {
            Projectile projectileScript = objectPulled.GetComponent<Projectile>();
            projectileScript.m_gunThatFiredProjectile = this;
            if (WeaponData.UseSpread)
            {
                Quaternion bulletForward = transform.rotation;
                bulletForward *= Quaternion.Euler(
                    new Vector3(
                        Random.Range(
                            -((Vector2)WeaponData.SpreadValues).x,
                            ((Vector2)WeaponData.SpreadValues).x
                        ),
                        0,
                        Random.Range(
                            -((Vector2)WeaponData.SpreadValues).y,
                            ((Vector2)WeaponData.SpreadValues).y
                        )
                    )
                );
                projectileScript.Setup(m_instantiationPoint.transform.position, bulletForward);
            }
            else
            {
                projectileScript.Setup(m_instantiationPoint.transform.position, transform.rotation);
            }
        }
    }

    /// <summary>
    /// Called when a GameObject is returned to the object pool.
    /// This method deactivates the object.
    /// </summary>
    /// <param name="objectReturned">
    /// The GameObject that was returned to the object pool.
    /// </param>
    protected void OnReturnedToPrimaryPool(GameObject objectReturned)
    {
        objectReturned.SetActive(false);
    }

    /// <summary>
    /// Destroys the specified GameObject.
    /// </summary>
    /// <param name="objectToDestroy">
    /// The GameObject to be destroyed.
    /// </param>
    protected void DestroyPrimaryPooledObject(GameObject objectToDestroy)
    {
        Destroy(objectToDestroy);
    }
}

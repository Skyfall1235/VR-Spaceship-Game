using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledWeapon : BC_Weapon
{
    [SerializeField]
    protected ObjectPool<GameObject> m_pool;
    public ObjectPool<GameObject> Pool
    {
        get { return m_pool; }
        //set { m_pool = value; }
    }

    public override void OnFire()
    {
        m_pool.Get();
    }

    protected override void Awake()
    {
        base.Awake();
        m_pool = new ObjectPool<GameObject>(OnCreatePooledObject, OnPulledFromPool, OnReturnedToPool, DestroyPooledObject);
    }

    /// <summary>
    /// Instantiates a new bullet object from the WeaponData.PrefabBullet prefab.
    /// </summary>
    /// <returns>
    /// The newly instantiated GameObject representing the bullet.
    /// </returns>
    protected GameObject OnCreatePooledObject()
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
    protected void OnPulledFromPool(GameObject objectPulled)
    {
        objectPulled.SetActive(true);
        if (objectPulled.HasComponent<BasicProjectile>())
        {
            BasicProjectile projectileScript = objectPulled.GetComponent<BasicProjectile>();
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

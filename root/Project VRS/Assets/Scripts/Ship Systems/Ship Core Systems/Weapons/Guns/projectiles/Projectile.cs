using System.Collections;
using UnityEngine;
using UnityEngine.Pool;


[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    public PooledWeapon m_gunThatFiredProjectile;
    public int ProjectilePoolIndex { get; private set; }
    [SerializeField] protected ForceMode m_forceMode = ForceMode.Impulse;
    [SerializeField] protected Rigidbody m_projectileRigidBody;
    public ObjectPool<GameObject> poolToReturnTo;
    protected TrailRenderer m_trailRender;
    protected SO_ProjectileData.ProjectileData m_projectileData;
    private void Awake()
    {
        //setup trail renderer
        m_trailRender = GetComponent<TrailRenderer>();
    }
    protected virtual IEnumerator DestroyAfterTimeAsync()
    {
        //wait
        yield return new WaitForSeconds(m_projectileData.Lifetime);
        //if the gun still exists, we can release the projectile
        if(m_gunThatFiredProjectile != null && poolToReturnTo != null)
        {
            poolToReturnTo.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual IEnumerator SplitAfterTimeAsync()
    {
        yield return new WaitForSeconds((float)m_projectileData.SubProjectileSpawnTime);
        for(int i = 0; i < m_projectileData.SubProjectileData.ProjectileCount; i++)
        {
            GameObject firedProjectile = m_gunThatFiredProjectile.projectilePools[ProjectilePoolIndex + 1].Get();

            if (firedProjectile.HasComponent<Projectile>())
            {
                firedProjectile.GetComponent<Projectile>().Fire
                (
                    transform.position,
                    Quaternion.FromToRotation(Vector3.up, m_projectileRigidBody.velocity),
                    m_projectileData.SubProjectileData,
                    ProjectilePoolIndex + 1
                );
            }
        }
    }

    /// <summary>
    /// Sets up the main projectile from the gun.
    /// </summary>
    /// <param name="startingPosition"> is the starting position for the projectile</param>
    /// <param name="startingRotation"> is the starting rotation for the projectile</param>
    public virtual void Fire
    (
        Vector3 startingPosition,
        Quaternion startingRotation,
        SO_ProjectileData.ProjectileData projectileData,
        int poolIndex
    )
    {
        //setup the position and rotation
        m_projectileRigidBody.velocity = Vector3.zero;
        transform.position = startingPosition;
        //Update projectile data
        m_projectileData = projectileData;
        //setup current index
        ProjectilePoolIndex = poolIndex;

        Quaternion fireRotation = startingRotation;
        fireRotation *= Quaternion.Euler(
            new Vector3(
                Random.Range(
                    -m_projectileData.Spread.x,
                    m_projectileData.Spread.x
                ),
                0,
                Random.Range(
                    -m_projectileData.Spread.y,
                    m_projectileData.Spread.y
                )
            )
        );
        transform.rotation = fireRotation;
        
        //clear visual effects and ADD FORCE BABYYYYYY
        m_trailRender.Clear();
        m_projectileRigidBody.AddForce(transform.up * m_projectileData.Speed, m_forceMode);
        if (m_projectileData.SpawnSubProjectiles)
        {
            StartCoroutine(SplitAfterTimeAsync());
        }
        StartCoroutine(DestroyAfterTimeAsync());
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.gameObject.GetComponent<IDamagable>() != null) 
        {
            IDamagable objectHealth = collision.gameObject.GetComponent<IDamagable>();
            DamageData damageData = new DamageData(m_projectileData.Damage, m_projectileData.ArmorPenetration, this.gameObject);
            objectHealth.Damage(damageData);
        }
    }

    /// <summary>
    /// Passes weapon reference to the projectile
    /// </summary>
    /// <param name="weapon">weapon to pass</param>
    public void SetFiringWeapon(PooledWeapon weapon)
    {
        if (weapon != null)
        {
            m_gunThatFiredProjectile = weapon;
        }
    }
}
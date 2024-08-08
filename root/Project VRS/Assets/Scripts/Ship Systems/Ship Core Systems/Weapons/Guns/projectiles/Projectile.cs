using System.Collections;
using UnityEngine;
using UnityEngine.Pool;


[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// The weapon that fired a projectile
    /// </summary>
    public PooledWeapon m_gunThatFiredProjectile;
    /// <summary>
    /// The index of the projectile pool to return to on the pooled weapon
    /// </summary>
    public int ProjectilePoolIndex { get; private set; }
    [SerializeField] protected ForceMode m_forceMode = ForceMode.Impulse;
    [SerializeField] protected Rigidbody m_projectileRigidBody;
    protected TrailRenderer m_trailRender;
    [SerializeField]
    protected ProjectileData m_projectileData;
    private void Awake()
    {
        //setup trail renderer
        m_trailRender = GetComponent<TrailRenderer>();
    }
    private void FixedUpdate()
    {
        if(m_projectileRigidBody.velocity.magnitude > 0 && gameObject.activeSelf)
        {
            transform.rotation = Quaternion.LookRotation(m_projectileRigidBody.velocity);
        }
    }
    /// <summary>
    /// Destroys the projectile or returns it to its pool after a given time
    /// </summary>
    /// <returns>Nothing</returns>
    protected virtual IEnumerator DestroyAfterTimeAsync()
    {
        //wait
        yield return new WaitForSeconds(m_projectileData.Lifetime);
        //if the gun still exists, we can release the projectile
        if(m_gunThatFiredProjectile != null && ProjectilePoolIndex < m_gunThatFiredProjectile.projectilePools.Count)
        {
            m_gunThatFiredProjectile.projectilePools[ProjectilePoolIndex].Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Splits after a given period
    /// </summary>
    /// <returns>Nothing</returns>
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
                    Quaternion.FromToRotation(Vector3.up, transform.forward),
                    m_projectileData.SubProjectileData,
                    ProjectilePoolIndex + 1
                );
            }
        }
    }

    /// <summary>
    /// Initializes projectile variables, sets up vfx, and adds force to the projectile
    /// </summary>
    /// <param name="startingPosition">The place for the projectile to be fired from</param>
    /// <param name="startingRotation">The quaternion describing the rotation of the projectile</param>
    /// <param name="projectileData">The data that the pprojectile should use for its attributes</param>
    /// <param name="poolIndex">The pool index of the projectile</param>
    public virtual void Fire
    (
        Vector3 startingPosition,
        Quaternion startingRotation,
        ProjectileData projectileData,
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

        transform.rotation = startingRotation;
        Vector3 currentUp = transform.up;
        Vector3 currentRight = transform.right;
        Quaternion fireRotation = transform.rotation;
        fireRotation *= Quaternion.AngleAxis(Random.Range(-m_projectileData.Spread.y, m_projectileData.Spread.y), currentRight);
        fireRotation *= Quaternion.AngleAxis(Random.Range(-m_projectileData.Spread.x, m_projectileData.Spread.x), currentUp);
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
            DamageData damageData = new DamageData(m_projectileData.Damage, collision.contacts[0].point, m_projectileData.ArmorPenetration, this.gameObject);
            objectHealth.Damage(damageData);
        }
        Destroy(gameObject);
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
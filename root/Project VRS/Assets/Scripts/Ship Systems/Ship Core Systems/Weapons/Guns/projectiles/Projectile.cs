using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    public PooledWeapon m_gunThatFiredProjectile;

    [SerializeField] protected ForceMode m_forceMode = ForceMode.Impulse;
    [SerializeField] protected Rigidbody m_projectileRigidBody;
    protected TrailRenderer m_trailRender;
    protected SO_ProjectileData m_projectileData;
    private void Awake()
    {
        //setup trail renderer
        m_trailRender = GetComponent<TrailRenderer>();
    }
    protected virtual IEnumerator DestroyAfterTime()
    {
        //wait
        yield return new WaitForSeconds(m_projectileData.ProjectileDestroyTime);
        //if the gun still exists, we can release the projectile
        if(m_gunThatFiredProjectile != null)
        {
            m_gunThatFiredProjectile.PrimaryProjectilePool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Sets up the main projectile from the gun.
    /// </summary>
    /// <param name="startingPosition"> is the starting position for the projectile</param>
    /// <param name="startingRotation"> is the starting rotation for the projectile</param>
    public virtual void Fire(Vector3 startingPosition, Quaternion startingRotation)
    {
        //setup the position and rotation
        m_projectileRigidBody.velocity = Vector3.zero;
        transform.position = startingPosition;

        Quaternion fireRotation = startingRotation;
        fireRotation *= Quaternion.Euler(
            new Vector3(
                Random.Range(
                    -((Vector2)m_gunThatFiredProjectile.WeaponData.SpreadValues).x,
                    ((Vector2)m_gunThatFiredProjectile.WeaponData.SpreadValues).x
                ),
                0,
                Random.Range(
                    -((Vector2)m_gunThatFiredProjectile.WeaponData.SpreadValues).y,
                    ((Vector2)m_gunThatFiredProjectile.WeaponData.SpreadValues).y
                )
            )
        );
        transform.rotation = fireRotation;

        //clear visual effects and ADD FORCE BABYYYYYY
        m_trailRender.Clear();
        m_projectileRigidBody.AddForce(transform.up * m_projectileData.ProjectileSpeed, m_forceMode);
        StartCoroutine(DestroyAfterTime());
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.gameObject.GetComponent<IDamagable>() != null) 
        {
            IDamagable objectHealth = collision.gameObject.GetComponent<IDamagable>();
            DamageData damageData = new DamageData(m_projectileData.ProjectileDamage, m_projectileData.ArmorPenetration, this.gameObject);
            objectHealth.Damage(damageData);
        }
    }

    /// <summary>
    /// Passes projectile data to the projectile
    /// </summary>
    /// <param name="projectileData">The projectile data to pass</param>
    public void SetProjectileData(SO_ProjectileData projectileData)
    {
        if(projectileData != null)
        {
            m_projectileData = projectileData;
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
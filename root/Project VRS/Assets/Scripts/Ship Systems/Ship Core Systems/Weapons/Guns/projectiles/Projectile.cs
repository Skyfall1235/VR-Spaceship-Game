using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    public PooledWeapon m_gunThatFiredProjectile;

    [SerializeField] ForceMode m_forceMode = ForceMode.Impulse;
    [SerializeField] Rigidbody m_projectileRigidBody;
    TrailRenderer m_trailRender;
    SO_ProjectileData m_projectileData;
    private void Awake()
    {
        //setup trail renderer
        m_trailRender = GetComponent<TrailRenderer>();
    }
    protected IEnumerator DestroyAfterTime()
    {
        //wait
        yield return new WaitForSeconds(m_gunThatFiredProjectile.WeaponData.ProjectileData.ProjectileDestroyTime);
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
    public virtual void Setup(Vector3 startingPosition, Quaternion startingRotation, SO_ProjectileData projectileData)
    {
        //setup the position and rotation
        m_projectileRigidBody.velocity = Vector3.zero;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        m_projectileData = projectileData;

        //clear visual effects and ADD FORCE BABYYYYYY
        m_trailRender.Clear();
        m_projectileRigidBody.AddForce(transform.up * m_projectileData.ProjectileSpeed, m_forceMode);
        StartCoroutine(DestroyAfterTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null && collision.gameObject.GetComponent<IDamagable>() != null) 
        {
            Health objectHealth = collision.gameObject.GetComponent<Health>();
            uint damageVal = m_projectileData.m_projectileDamage;
            DamageData damageData = new DamageData(damageVal, 0, this.gameObject);
            objectHealth.Damage(damageData);
        }
    }
}


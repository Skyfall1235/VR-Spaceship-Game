using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour
{
    public PooledWeapon m_gunThatFiredProjectile;

    [SerializeField] ForceMode m_forceMode = ForceMode.Impulse;
    float m_projectileSpeed;
    [SerializeField] Rigidbody m_projectileRigidBody;
    TrailRenderer m_trailRender;

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
    public virtual void Setup(Vector3 startingPosition, Quaternion startingRotation)
    {
        //setup the position and rotation
        m_projectileRigidBody.velocity = Vector3.zero;
        transform.position = startingPosition;
        transform.rotation = startingRotation;

        //now, set the speed to what the scriptable object says it should be
        m_projectileSpeed = m_gunThatFiredProjectile.WeaponData.ProjectileData.ProjectileSpeed;

        //clear visual effects and ADD FORCE BABYYYYYY
        m_trailRender.Clear();
        m_projectileRigidBody.AddForce(transform.up * m_projectileSpeed, m_forceMode);
        StartCoroutine(DestroyAfterTime());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision != null) 
        {
            Health objectHealth = collision.gameObject.GetComponent<Health>();
            uint damageVal = m_gunThatFiredProjectile.ProjectileData.m_projectileDamage;
            DamageData damageData = new DamageData(damageVal, 0, this.gameObject);
            objectHealth.Damage(damageData);
        }
    }
}


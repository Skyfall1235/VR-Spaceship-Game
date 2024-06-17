using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubProjectile : Projectile
{
    public override void Fire(Vector3 startingPosition, Quaternion startingRotation)
    {
        //setup the position and rotation
        m_projectileRigidBody.velocity = Vector3.zero;
        transform.position = startingPosition;

        Quaternion fireRotation = startingRotation;
        fireRotation *= Quaternion.Euler(
            new Vector3(
                Random.Range(
                    -((Vector2)m_projectileData.SubProjectileSpread).x,
                    ((Vector2)m_projectileData.SubProjectileSpread).x
                ),
                0,
                Random.Range(
                    -((Vector2)m_projectileData.SubProjectileSpread).y,
                    ((Vector2)m_projectileData.SubProjectileSpread).y
                )
            )
        );
        transform.rotation = fireRotation;
        //clear visual effects and ADD FORCE BABYYYYYY
        m_trailRender.Clear();
        m_projectileRigidBody.AddForce(transform.up * (float)m_projectileData.SubProjectileSpeed, m_forceMode);
        StartCoroutine(DestroyAfterTime());
    }

    protected override IEnumerator DestroyAfterTime()
    {
        //wait
        yield return new WaitForSeconds((float)m_projectileData.SubProjectileDestroyTime);
        //if the gun still exists, we can release the projectile
        if (m_gunThatFiredProjectile != null)
        {
            m_gunThatFiredProjectile.SecondaryProjectilePool.Release(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.GetComponent<IDamagable>() != null)
        {
            IDamagable objectHealth = collision.gameObject.GetComponent<IDamagable>();
            DamageData damageData = new DamageData((uint)m_projectileData.SubProjectileDamage, (uint)m_projectileData.SubProjectileArmorPenetration, this.gameObject);
            objectHealth.Damage(damageData);
        }
    }
}

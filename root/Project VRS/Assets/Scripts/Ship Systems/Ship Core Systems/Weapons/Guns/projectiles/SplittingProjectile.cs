using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingProjectile : Projectile
{
    [SerializeField] Transform m_splitSpawnPoint;
    public override void Fire(Vector3 startingPosition, Quaternion startingRotation)
    {
        base.Fire(startingPosition, startingRotation);
        StartCoroutine(SplitAfterTimeAsync());
    }
    IEnumerator SplitAfterTimeAsync()
    {
        if (!m_projectileData.SpawnSubProjectiles)
        {
            yield break;
        }
        yield return new WaitForSeconds((float)m_projectileData.SubProjectileSpawnTime);
        Split();
    }
    void Split()
    {
        for(int i = 0; i < m_projectileData.SubProjectileCount; i++)
        {
            GameObject subProjectile = m_gunThatFiredProjectile.SecondaryProjectilePool.Get();
            //not sure why I need this additional rotation but it works. Whatever unity 
            subProjectile.GetComponent<Projectile>().Fire(transform.position, Quaternion.LookRotation(m_projectileRigidBody.velocity) * Quaternion.Euler(90 ,0, 0));
        }
    }
}

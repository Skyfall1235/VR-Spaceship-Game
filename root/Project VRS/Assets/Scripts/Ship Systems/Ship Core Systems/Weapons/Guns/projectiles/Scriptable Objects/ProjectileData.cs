using UnityEngine;
[System.Serializable]
public class ProjectileData
{

    [SerializeField] float m_speed = 0;
    /// <summary>
    /// The speed at which a projectile should move
    /// </summary>
    public float Speed { get { return m_speed; } }

    [SerializeField] uint m_damage = 0;
    /// <summary>
    /// The damage that a projectile should do
    /// </summary>
    public uint Damage { get { return m_damage; } }

    [SerializeField] uint m_armorPenetration = 0;
    /// <summary>
    /// The amount of armor penetration a projectile should have
    /// </summary>
    public uint ArmorPenetration { get { return m_armorPenetration; } }

    [SerializeField] float m_lifetime = 0;
    /// <summary>
    /// How long the projectile should stay alive for
    /// </summary>
    public float Lifetime { get { return m_lifetime; } }

    [SerializeField] uint m_projectileCount = 1;
    /// <summary>
    /// The total amount of projectiles that should be fired when the projectile of this data type is fired
    /// </summary>
    public uint ProjectileCount { get { return m_projectileCount; } }

    [SerializeField] Vector2 m_spread = new Vector2(0, 0);
    /// <summary>
    /// The random deviation on x and y each projectile should have
    /// </summary>
    public Vector2 Spread { get { return m_spread; } }

    [SerializeField] GameObject m_prefab = null;
    /// <summary>
    /// The prefab of the projectile
    /// </summary>
    public GameObject Prefab { get { return m_prefab; } }

    [SerializeField][HideInInspector] bool m_spawnSubProjectiles = false;
    /// <summary>
    /// Whether this projectile should spawn sub-projectiles
    /// </summary>
    public bool SpawnSubProjectiles { get { return m_spawnSubProjectiles; } }

    [SerializeField][HideInInspector] float m_subProjectileSpawnTime = 0;
    /// <summary>
    /// The point in the projectile's lifetime that it should spawn sub-projectiles
    /// </summary>
    public float? SubProjectileSpawnTime
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileSpawnTime;
            }
            return null;
        }
    }

    [HideInInspector][SerializeReference] ProjectileData m_subProjectileData = null;
    /// <summary>
    /// The data of the sub projectile spawned by the projectile
    /// </summary>
    public ProjectileData SubProjectileData { get { return m_subProjectileData; } internal set { m_subProjectileData = value; } }
}
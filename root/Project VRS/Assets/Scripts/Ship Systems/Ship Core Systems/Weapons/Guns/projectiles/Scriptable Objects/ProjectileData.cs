using UnityEngine;

[System.Serializable]
public class ProjectileData
{

    [SerializeField] float m_speed = 0;
    public float Speed { get { return m_speed; } }

    [SerializeField] uint m_damage = 0;
    public uint Damage { get { return m_damage; } }

    [SerializeField] public uint m_armorPenetration = 0;
    public uint ArmorPenetration { get { return m_armorPenetration; } }

    [SerializeField] float m_lifetime = 0;
    public float Lifetime { get { return m_lifetime; } }

    [SerializeField] Vector2 m_spread = new Vector2(0, 0);
    public Vector2 Spread { get { return m_spread; } }

    [SerializeField] GameObject m_prefab = null;
    public GameObject Prefab { get { return m_prefab; } }

    [SerializeField][HideInInspector] bool m_spawnSubProjectiles = false;
    public bool SpawnSubProjectiles { get { return m_spawnSubProjectiles; } }

    [SerializeField][HideInInspector] uint m_subProjectileCount = 0;
    public uint? SubProjectileCount
    {
        get
        {
            if (m_spawnSubProjectiles)
            {
                return m_subProjectileCount;
            }
            return null;
        }
    }

    [SerializeField][HideInInspector] float m_subProjectileSpawnTime = 0;
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
    public ProjectileData SubProjectileData { get { return m_subProjectileData; } internal set { m_subProjectileData = value; } }
}

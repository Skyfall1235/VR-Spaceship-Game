using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable
{
    uint m_currentHealth;
    public uint CurrentHealth
    {
        get
        {
            return m_currentHealth;
        }
        private set
        {
            CheckDeathState(m_currentHealth, value);
            m_currentHealth = value;
        }
    }
    uint m_maxHealth = 100;
    public uint MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
        private set
        {
            m_maxHealth = value;
            CurrentHealth = (uint)Mathf.Clamp((int)CurrentHealth, 0, (int)value);
        }
    }
    [SerializeField] bool m_useInvulnerabilityAfterDamage = true;
    [SerializeField] float m_inulnerabilityAfterDamageTimer = 3;

    [SerializeField] bool m_useArmor;
    [SerializeField] uint m_armor;
    [SerializeField] uint m_armorForFiftyPercentReduction = 50;
    
    bool m_isInvulnerable = false;
    IEnumerator m_currentInvincibilityTimer = null;
    public bool IsInvulnerable
    {
        get
        {
            return (m_isInvulnerable);
        }
        set
        {
            if (m_currentInvincibilityTimer != null)
            {
                StopCoroutine(m_currentInvincibilityTimer);
            }
            if (value)
            {
                OnBecameInvulnerable.Invoke();
            }
            else
            {
                OnBecameVulnerable.Invoke();
            }
            m_isInvulnerable = value;
        }
    }
    public bool IsAlive { get; private set; } = true;

    #region Events

    private DamageData.OnHealEvent m_onHeal = new DamageData.OnHealEvent();
    public DamageData.OnHealEvent OnHeal 
    {
        get
        {
            return m_onHeal;
        }
        set
        {
            m_onHeal = value;
        }
    }

    private DamageData.OnDamageEvent m_onDamage = new DamageData.OnDamageEvent();
    public DamageData.OnDamageEvent OnDamage 
    {
        get
        {
            return m_onDamage;
        }
        set
        {
            m_onDamage = value;
        }
    }

    private DamageData.OnHealthComponentInitialized m_onHealthInitialized = new DamageData.OnHealthComponentInitialized();
    public DamageData.OnHealthComponentInitialized OnHealthInitialized 
    {
        get
        {
            return m_onHealthInitialized;
        }
        set
        {
            m_onHealthInitialized = value;
        }
    }

    #endregion

    /// <summary>
    /// Triggered when a health component dies
    /// </summary>
    public UnityEvent OnDeath = new UnityEvent();
    /// <summary>
    /// Triggered when a health component gets revived
    /// </summary>
    public UnityEvent OnRevive = new UnityEvent();
    /// <summary>
    /// Triggered when a health component becomes invulnerable
    /// </summary>
    public UnityEvent OnBecameInvulnerable = new UnityEvent();
    /// <summary>
    /// Triggered when a health component becomes vulnerable
    /// </summary>
    public UnityEvent OnBecameVulnerable = new UnityEvent();

    

    void Start()
    {
        CurrentHealth = m_maxHealth;
        IsAlive = CurrentHealth > 0;
        m_onHealthInitialized.Invoke(m_maxHealth, CurrentHealth);
    }

    /// <summary>
    /// Damages the health component
    /// </summary>
    /// <param name="damageData">The data of the damage trying to be dealt</param>
    /// <param name="ignoreInvulnerabilityAfterDamage">Whether we ignore the invulnerability timer on this damage tick</param>
    /// <param name="ignoreArmor">Whether we ignore armor damage reduction on this damage tick</param>
    public void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false)
    {
        if (!IsInvulnerable)
        {
            //calculate the damage to be taken
            uint newHealth = (uint)Mathf.Clamp((int)CurrentHealth - (ignoreArmor || !m_useArmor ? (int)damageData.Damage : Mathf.FloorToInt((float)damageData.Damage * CalculateDamageReduction(damageData.ArmorPenetration))), 0, m_maxHealth);
            OnDamage.Invoke(m_maxHealth, CurrentHealth, newHealth);
            //take the damage
            CurrentHealth = newHealth;
            if (m_useInvulnerabilityAfterDamage && ignoreInvulnerabilityAfterDamage && gameObject.activeSelf)
            {
                StartCoroutine(InvulnerabilityTimerAsync());
            }
        }
    }
    /// <summary>
    /// Heals the health component
    /// </summary>
    /// <param name="amountToHeal">The amount to heal the health component</param>
    public void Heal(uint amountToHeal)
    {
        uint newHealth = (uint)Mathf.Clamp((int)CurrentHealth + (int)amountToHeal, 0, m_maxHealth); ;
        OnHeal.Invoke(m_maxHealth, CurrentHealth, newHealth);
        CurrentHealth = newHealth;
    }
    /// <summary>
    /// Updates the death state of a health component and calls the events
    /// </summary>
    /// <param name="previousHealth">The health before an action is performed on the health component</param>
    /// <param name="currentHealth">The health after an action is performed on the health component</param>
    void CheckDeathState(uint previousHealth, uint currentHealth)
    {
        if (currentHealth <= 0 && previousHealth > 0)
        {
            IsAlive = false;
            OnDeath.Invoke();
        }
        if (previousHealth <= 0 && currentHealth > 0)
        {
            IsAlive = true;
            OnRevive.Invoke();
        }
    }
    public void Reset()
    {
        CurrentHealth = m_maxHealth;
        IsAlive = CurrentHealth > 0;
    }
    /// <summary>
    /// calculates damage reduction percent based on armor and armor penetration values
    /// </summary>
    /// <param name="armorPentetration">The amount of armor penetration</param>
    /// <returns>The damage reduction percent</returns>
    float CalculateDamageReduction(uint armorPentetration)
    {
        uint armorAfterPenetration = (uint)Mathf.Clamp((int)m_armor - (int)armorPentetration, 0, uint.MaxValue);
        return (float)armorAfterPenetration / (float)(armorAfterPenetration + m_armorForFiftyPercentReduction);
    }
    IEnumerator InvulnerabilityTimerAsync()
    {
        m_isInvulnerable = true;
        OnBecameInvulnerable.Invoke();
        yield return new WaitForSeconds(m_inulnerabilityAfterDamageTimer);
        m_isInvulnerable = false;
        OnBecameVulnerable.Invoke();
    }
}

/// <summary>
/// A struct containing data required for dealing damage
/// </summary>
public struct DamageData
{
    public class OnHealEvent : UnityEvent<uint, uint, uint> { }

    public class OnDamageEvent : UnityEvent<uint, uint, uint> { }

    public class OnHealthComponentInitialized : UnityEvent<uint, uint> { }
    /// <summary>
    /// Creates a DamageData struct
    /// </summary>
    /// <param name="damage">The amount of damage dealt</param>
    /// <param name="armorPenetration">The amount of armor penetration of the damage</param>
    /// <param name="damager">The GameObject who dealt the damage</param>
    public DamageData(uint damage, uint armorPenetration = 0, GameObject damager = null)
    {
        Damage = damage;
        ArmorPenetration = armorPenetration;
        Damager = damager;
    }
    /// <summary>
    /// The amount of damage dealt
    /// </summary>
    public uint Damage { get; private set; }
    /// <summary>
    /// The amount of armor penetration of the damage
    /// </summary>
    public uint ArmorPenetration { get; private set; }
    /// <summary>
    /// The GameObject who dealt the damage
    /// </summary>
    public GameObject Damager { get; private set; }
}
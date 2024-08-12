using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable, IHealable
{
    [SerializeField] uint m_currentHealth;
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
    
    [SerializeField] uint m_maxHealth = 100;
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

    [SerializeField] bool m_isInvulnerable = false;
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


    /// <summary>
    /// Triggered whenever a health component takes damage
    /// </summary>
    [field:SerializeField] public UnityEvent<DamageData, uint, uint> OnDamage { get; set; } = new UnityEvent<DamageData, uint, uint>();

    /// <summary>
    /// Triggered whenever a health component is healed
    /// </summary>
    [field:SerializeField] public UnityEvent<HealData, uint, uint> OnHeal { get; set; } = new UnityEvent<HealData, uint, uint>();

    /// <summary>
    /// triggered when a healthcomponent is initialized
    /// Parameter 1 is the max health
    /// Parameter 2 is the current health
    /// </summary>
    public UnityEvent<uint, uint> OnHealthInitialized = new UnityEvent<uint, uint>();

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
    #endregion
    /// <summary>
    /// Initializes the Health Component with a full HP and invokes the declaration event
    /// </summary>
    public virtual void InitializeHealth()
    {
        CurrentHealth = m_maxHealth;
        IsAlive = CurrentHealth > 0;
        OnHealthInitialized.Invoke(m_maxHealth, CurrentHealth);
    }

    /// <summary>
    /// Damages the health component
    /// </summary>
    /// <param name="damageData">The data of the damage trying to be dealt</param>
    /// <param name="ignoreInvulnerabilityAfterDamage">Whether we ignore the invulnerability timer on this damage tick</param>
    /// <param name="ignoreArmor">Whether we ignore armor damage reduction on this damage tick</param>
    public virtual void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false)
    {
        if (!IsInvulnerable)
        {
            //calculate the damage to be taken
            uint newHealth = (uint)Mathf.Clamp((int)CurrentHealth - ((ignoreArmor || !m_useArmor) ? (int)damageData.Damage : Mathf.CeilToInt((float)damageData.Damage * (float)CalculateDamageReduction(damageData.ArmorPenetration))), 0, m_maxHealth);
            OnDamage.Invoke(damageData, CurrentHealth, newHealth);
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
    public virtual void Heal(HealData healData)
    {
        uint newHealth = (uint)Mathf.Clamp((int)CurrentHealth + (int)healData.HealAmount, 0, m_maxHealth); ;
        OnHeal.Invoke(healData, CurrentHealth, newHealth);
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
        return 1 - (float)armorAfterPenetration / ((float)armorAfterPenetration + (float)m_armorForFiftyPercentReduction);
    }
    /// <summary>
    /// Handles the invulnerability timer after taking damage
    /// </summary>
    /// <returns>Nothing</returns>
    IEnumerator InvulnerabilityTimerAsync()
    {
        m_isInvulnerable = true;
        OnBecameInvulnerable.Invoke();
        yield return new WaitForSeconds(m_inulnerabilityAfterDamageTimer);
        m_isInvulnerable = false;
        OnBecameVulnerable.Invoke();
    }
    void Awake()
    {
        InitializeHealth();
    }
}

/// <summary>
/// A struct containing data required for dealing damage
/// </summary>
public struct DamageData
{
    /// <summary>
    /// Creates a DamageData struct
    /// </summary>
    /// <param name="damage">The amount of damage dealt</param>
    /// <param name="armorPenetration">The amount of armor penetration of the damage</param>
    /// <param name="damager">The GameObject who dealt the damage</param>
    /// <param name="damageLocation">The location at which damage was dealt</param>
    public DamageData(uint damage, Vector3 damageLocation, uint armorPenetration = 0, GameObject damager = null)
    {
        Damage = damage;
        ArmorPenetration = armorPenetration;
        Damager = damager;
        DamageLocation = damageLocation;
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
    /// <summary>
    /// The location at which damage was dealt
    /// </summary>
    public Vector3 DamageLocation { get; private set; }
}

/// <summary>
/// A struct containing data required for healing
/// </summary>
public struct HealData
{
    /// <summary>
    /// Creates a HealData struct
    /// </summary>
    /// <param name="healAmount">The amount of damage healed</param>
    /// <param name="healer">The GameObject who healed the health component</param>
    /// <param name="healLocation">The location at which the healing occured</param>
    public HealData(uint healAmount, Vector3 healLocation, GameObject healer = null)
    {
        HealAmount = healAmount;
        Healer = healer;
        DamageLocation = healLocation;
    }
    /// <summary>
    /// The amount healed
    /// </summary>
    public uint HealAmount { get; private set; }

    /// <summary>
    /// The GameObject who healed the health component
    /// </summary>
    public GameObject Healer { get; private set; }
    /// <summary>
    /// The location at which the healing occured
    /// </summary>
    public Vector3 DamageLocation { get; private set; }
}
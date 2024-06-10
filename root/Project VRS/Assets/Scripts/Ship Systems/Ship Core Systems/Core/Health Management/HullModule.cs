using UnityEngine;

//just a register for hits
public class HullModule : MonoBehaviour, IDamagable
{
    [SerializeField]
    private HullHealthManager m_hullHealthManager;

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

    public void Damage(DamageData damageData, bool ignoreInvulnerabilityAfterDamage = false, bool ignoreArmor = false)
    {
        m_hullHealthManager.Damage(damageData, ignoreInvulnerabilityAfterDamage, ignoreArmor);
    }
}

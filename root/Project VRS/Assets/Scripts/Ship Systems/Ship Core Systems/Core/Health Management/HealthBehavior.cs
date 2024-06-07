using UnityEngine;
using static ICoreModule;
using static IModuleDamage;

/// <summary>
/// Defines the behaviors that IHealthEvents require.
/// </summary>
public class HealthBehavior : MonoBehaviour, IHealthEvents
{
    [SerializeField]
    protected OnHealEvent m_onHealEvent = new();

    [SerializeField]
    protected OnDamageEvent m_onDamageEvent = new();

    [SerializeField]
    protected OnDeathEvent m_onDeathEvent = new();

    /// <summary>
    /// Unity Event for activation of the Heal action.
    /// </summary>
    public OnHealEvent onHealEvent
    {
        get
        {
            return m_onHealEvent;
        }
        set
        {
            m_onHealEvent = value;
        }
    }

    /// <summary>
    /// Unity Event for activation of the Damage action.
    /// </summary>
    public OnDamageEvent onDamageEvent
    {
        get
        {
            return m_onDamageEvent;
        }
        set
        {
            m_onDamageEvent = value;
        }
    }

    /// <summary>
    /// Unity Event for activation of the Death action.
    /// </summary>
    public OnDeathEvent onDeathEvent
    {
        get
        {
            return m_onDeathEvent;
        }
        set
        {
            m_onDeathEvent = value;
        }
    }

    /// <summary>
    /// Applies damage to this module based on the provided damage data.
    /// </summary>
    /// <param name="damageData">The weapon collision data containing damage information.</param>
    public virtual void TakeDamage(IDamageData.WeaponCollisionData damageData)
    {
        //allow the health script to handle the actual number stuff and then invoke the event
        m_onDamageEvent.Invoke(damageData, this, ModuleStateChangeType.Health);
    }

    /// <summary>
    /// Heals this module based on the provided heal data.
    /// </summary>
    /// <param name="healData">The heal module data containing healing information.</param>
    public virtual void HealObject(IDamageData.HealModuleData healData)
    {
        m_onHealEvent.Invoke(healData, this, ModuleStateChangeType.Health);
    }
}

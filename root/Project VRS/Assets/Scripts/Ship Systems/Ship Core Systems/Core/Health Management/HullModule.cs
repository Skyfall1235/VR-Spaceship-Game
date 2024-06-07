using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IDamageData;
using static IModuleDamage;

//this is the actual containment for events
public class HullModule : HealthBehavior
{
    [SerializeField]
    private HullModuleHealth m_moduleHealth;

    public override void HealObject(IDamageData.HealModuleData healData)
    {
        base.HealObject(healData);
        m_moduleHealth.HealObject(healData);
    }

    public override void TakeDamage(IDamageData.WeaponCollisionData damageData)
    {
        base.TakeDamage(damageData);
        m_moduleHealth.TakeDamage(damageData);
    }
}

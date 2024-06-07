using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ICoreModule;
using static IDamageData;
using static IModuleDamage;

//just a register for hits
public class HullModule : MonoBehaviour, IDamageEvents
{
    [SerializeField]
    private HullHealthManager m_hullHealthManager;

    public IDamageEvents.OnDamageEvent OnDamageEvent = new();

    public void TakeDamage(WeaponCollisionData damageData)
    {
        OnDamageEvent.Invoke(damageData, m_hullHealthManager, ModuleStateChangeType.Health);
        m_hullHealthManager.TakeDamage(damageData);
    }
}

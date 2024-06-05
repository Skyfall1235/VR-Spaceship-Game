using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullModuleHealth : BC_UniversalEntityHealth
{
    //all it needs are alerts. das it
    public override void InitializeHealth(IHealthEvents owner)
    {
        base.InitializeHealth(owner);
    }

    public override void TakeDamage(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        base.TakeDamage(weaponCollisionData);
    }

    public override void HealObject(IDamageData.HealModuleData healModuleData)
    {
        base.HealObject(healModuleData);
    }
}

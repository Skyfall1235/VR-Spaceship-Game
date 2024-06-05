using UnityEngine;

/// <summary>
/// Manages the health of a core module within a ship system. 
/// This class is responsible for tracking the module's health, handling incoming damage and healing, and triggering health-related events.
/// </summary>
public class InternalModuleHealth : BC_UniversalEntityHealth
{

    #region public stuff

    public override void InitializeHealth(IHealthEvents owner)
    {
        base.InitializeHealth(owner);
    }

    //WE FOROGT TO DO THE ARMOR CALCUALTION YOU BUMBLING FOOL
    //EDIT: Fixed, dont know how i managed to forget that but its there now, i didnt do all thart math for nothin
    public override void TakeDamage(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        //calculate the damage before we make the health script do more work
        int damageValueFromDataPack = weaponCollisionData.damageVal;

        //use lookup table to find appropriate damage
        int damagePercentApplicable = FindDamageApplicable(weaponCollisionData);

        //find damage after armor negation
        int damageAfterArmor = CalculateDamageAfterArmor(damageValueFromDataPack, damagePercentApplicable);
        StartCoroutine(DamageEntityAction(damageAfterArmor));
        base.TakeDamage(weaponCollisionData);
    }

    public override void HealObject(IDamageData.HealModuleData healModuleData)
    {
        //start the heal corotuine and let it take the data
        StartCoroutine(HealEntityAction(healModuleData));
        base.HealObject(healModuleData);
    }

    #endregion
}


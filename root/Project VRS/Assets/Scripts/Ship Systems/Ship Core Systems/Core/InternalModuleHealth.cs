using UnityEngine;

/// <summary>
/// Manages the health of a core module within a ship system. 
/// This class is responsible for tracking the module's health, handling incoming damage and healing, and triggering health-related events.
/// </summary>
public class InternalModuleHealth : BC_UniversalEntityHealth
{
    public BC_CoreModule moduleOwner { get; set; }

    #region public stuff

    public override void InitializeHealth()
    {
        if(ModuleHealthData == null && logger != null)
        {
            logger.Log($"health Data scriptable object is missing on {this.gameObject.name}", CustomLogger.LogLevel.Warning, CustomLogger.LogCategory.Other, this);
        }
        ModuleHealth = ModuleHealthData.healthMax;
    }

    //WE FOROGT TO DO THE ARMOR CALCUALTION YOU BUMBLING FOOL
    //EDIT: Fixed, dont know how i managed to forget that but its there now, i didnt do all thart math for nothin
    public override void TakeDamage(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        int damageValueFromDataPack = weaponCollisionData.damageVal;
        int damagePercentApplicable = FindDamageApplicable(weaponCollisionData);
        int damageAfterArmor = CalculateDamageAfterArmor(damageValueFromDataPack, damagePercentApplicable);
        StartCoroutine(DamageModuleAction(damageAfterArmor));
    }

    public override void HealObject(IDamageData.HealModuleData healModuleData)
    {
        StartCoroutine(HealModuleAction(healModuleData));
    }

    #endregion
}


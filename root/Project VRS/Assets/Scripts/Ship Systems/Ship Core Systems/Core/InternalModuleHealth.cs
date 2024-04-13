using System.Collections;
using UnityEngine;

public class InternalModuleHealth : MonoBehaviour, IModuleDamage
{
    public BC_CoreModule moduleOwner { get; set; }

    public SO_ModuleHealthData ModuleHealthData;
    public int ModuleHealth;
    //use a scriptable object here?
    //EDIT: i did infact use a scriptable object :P

    #region public stuff

    public void InitializeHealth()
    {
        ModuleHealth = ModuleHealthData.healthMax;
    }

    //WE FOROGT TO DO THE ARMOR CALCUALTION YOU BUMBLING FOOL
    //EDIT: Fixed, dont know how i managed to forget that but its there now, i didnt do all thart math for nothin
    public void TakeDamage(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        int damageValueFromDataPack = weaponCollisionData.damageVal;
        int damagePercentApplicable = FindDamageApplicable(weaponCollisionData);
        int damageAfterArmor = CalculateDamageAfterArmor(damageValueFromDataPack, damagePercentApplicable);
        StartCoroutine(DamageModuleAction(damageAfterArmor));
    }

    public void HealModule(IDamageData.HealModuleData healModuleData)
    {
        StartCoroutine(HealModuleAction(healModuleData));
    }

    #endregion

    #region Coroutines

    //cause rate is over time, we need it to stealily increase/decrease
    private IEnumerator HealModuleAction(IDamageData.HealModuleData healModuleData)
    {
        //save some initial values so we dont forget where we are
        int amountModuleHealedThisAction = 0;
        int healRate = healModuleData.rateOfApplication;
        int healValue = healModuleData.amountToHeal;

        //i increment upwards because idk, cry about it
        while (amountModuleHealedThisAction < healValue)
        {
            amountModuleHealedThisAction += healRate;
            ModuleHealth += healRate;
            yield return null;
        }
    }
    
    private IEnumerator DamageModuleAction(int damageVal)
    {
        //save some initial values so we dont forget where we are
        int amountModuleDamagedThisAction = 0;
        int damageRate = ModuleHealthData.rateOfDamage;

        //i increment upwards because idk, cry about it
        while (amountModuleDamagedThisAction < damageVal)
        {
            amountModuleDamagedThisAction += damageRate;
            ModuleHealth -= damageRate;
            //check if damage can still be applied
            if(ModuleHealth <= 0)
            {
                //if the modules dead, we should cancel the rest of the application and call the unity event
                moduleOwner.OnDeathEvent.Invoke(moduleOwner);
                yield break;
            }
            yield return null;           
        }
    }

    #endregion

    #region Calculate Damage Values
    //GEN Comments:
    //armor do a reduction if within 1 move of type, pen on eqiv type, and overpen if normal type
    //EDIT: the scale has been set up in the SO_ModuleHealthData script, its a lil janky but it works
    private int FindDamageApplicable(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        //incoming data
        IDamageData.WeaponType weaponType = weaponCollisionData.weaponType;
        //internal data
        IDamageData.ArmorType moduleType = ModuleHealthData.armorType;
        //compare the armor against the wepon by int casting and subtracting. positives mean less damage, negatives mean more
        int armorToPenetrationVal = (int)moduleType - (int)weaponType;
        int DamagePecentApplicable = 0;
        //now we handle the percent of damage application. the actual math to determine the damage value to be applied will be in a different math problem (sorry, shes not in this castle!)
        DamagePecentApplicable = EvaluateDamageApplicationCurve(armorToPenetrationVal);
        return DamagePecentApplicable;
        //ig the easiest way to do this would be to compare the (int)armor to the (int)weaponType. maybe just subtract the weapontype from the armor, and then multiply that number by 10.
        //do i want overpen?
    }

    private int EvaluateDamageApplicationCurve(int caseVal)
    {
        //create the offset
        caseVal += 5;
        //0 case should be 5th object, and -4 whould be first object
        int tableValue = ModuleHealthData.damageApplicationCurveList[caseVal].value;
        return tableValue;
    }

    private int CalculateDamageAfterArmor(int damage, int damagePercent)
    {
        //convert our percent value to a float decimal
        float intToPercent = damagePercent / 100;
        //Needed to apply the damage percent as an int
        float AppliedPercentValue = damage * intToPercent;
        int newDamageValue = Mathf.RoundToInt(AppliedPercentValue);
        return newDamageValue;
    }

    #endregion
}


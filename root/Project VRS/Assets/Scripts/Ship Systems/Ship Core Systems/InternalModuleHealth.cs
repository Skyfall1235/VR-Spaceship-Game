using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalModuleHealth : MonoBehaviour, IModuleDamage
{
    //health and armor is gonna be silly
    //light, medium, heavy, and super heavy armor type
    //armor do a reduction if within 1 move of type, pen on eqiv type, and overpen if normal type


    //what else can be put here? CALLS FOR SPECIFIC STATUS EFFECTS?

    /// <summary>
    /// Gets or sets the reference to the module that owns this component.
    /// </summary>
    public BC_CoreModule moduleOwner { get; set; }

    public SO_ModuleHealthData moduleHealthData;
    public int health;
    //use a scriptable object here?

    public void InitializeHealth()
    {
        health = moduleHealthData.healthMax;
    }

    public void TakeDamage(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        throw new System.NotImplementedException();
    }

    public void HealModule(IDamageData.HealModuleData healModuleData)
    {
        throw new System.NotImplementedException();
    }

    private int FindDamageApplicable(IDamageData.WeaponCollisionData weaponCollisionData)
    {
        IDamageData.WeaponType weaponType = weaponCollisionData.weaponType;
        IDamageData.ArmorType moduleType = moduleHealthData.armorType;
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
        int tableValue = moduleHealthData.damageApplicationCurveList[caseVal].value;
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
    
}



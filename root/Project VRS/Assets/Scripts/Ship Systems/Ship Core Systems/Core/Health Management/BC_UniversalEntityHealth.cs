using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BC_UniversalEntityHealth : MonoBehaviour, IModuleDamage
{
    [SerializeField]
    protected IHealthEvents m_scriptOwner;
    public IHealthEvents ScriptOwner
    {  
        get 
        { 
            return m_scriptOwner; 
        }
    }

    [SerializeField]
    protected SO_ModuleHealthData m_moduleHealthData;
    public SO_ModuleHealthData ModuleHealthData
    {
        get
        {
            return m_moduleHealthData;
        }
    }

    [SerializeField]
    protected int m_moduleHealth;
    public int ModuleHealth
    {
        get
        {
            return m_moduleHealth;
        }
    }

    [SerializeField]
    protected CustomLogger m_customLogger;

    #region Virtual Methods

    /// <summary>
    /// Initializes the health of the module.
    /// </summary>
    /// <param name="owner">The object that owns this health component and implements the IHealthEvents interface.</param>
    /// <remarks>
    /// This method first checks if the `ModuleHealthData` scriptable object is assigned. If not, it logs a warning message using the provided `m_customLogger` (if available).
    /// It then sets the initial health value based on the `ModuleHealthData.healthMax` property and stores the owner reference for event handling.
    /// </remarks>
    public virtual void InitializeHealth(IHealthEvents owner)
    {
        if (ModuleHealthData == null && m_customLogger != null)
        {
            m_customLogger.Log($"health Data scriptable object is missing on {this.gameObject.name}", CustomLogger.LogLevel.Warning, CustomLogger.LogCategory.Other, this);
        }
        //This line is commented because it was causing errors that wouldn't let me test my code
        //m_moduleHealth = ModuleHealthData.healthMax;
        m_scriptOwner = owner;
    }

    /// <summary>
    /// Applies damage to the module and triggers damage events.
    /// </summary>
    /// <param name="weaponCollisionData">Data about the weapon collision that caused the damage.</param>
    /// <remarks>
    /// This method first calls the `onDamageEvent` delegate implemented by the `m_scriptOwner` (which should implement the `IHealthEvents` interface).
    /// This allows other components to react to the damage taken. 
    /// 
    /// **Important:** Base class damage handling (if applicable) should be called at the **end** of this method's logic for proper inheritance behavior.
    /// </remarks>
    public virtual void TakeDamage(IDamageData.WeaponCollisionData weaponCollisionData) 
    {
        //call for damage events. death events are inherited
        m_scriptOwner.onDamageEvent.Invoke(weaponCollisionData, m_scriptOwner, ICoreModule.ModuleStateChangeType.Health);
    }

    /// <summary>
    /// Applies healing to the module and triggers healing events.
    /// </summary>
    /// <param name="healModuleData">Data about the heal module that is providing healing.</param>
    /// <remarks>
    /// This method first calls the `onHealEvent` delegate implemented by the `m_scriptOwner` (which should implement the `IHealthEvents` interface).
    /// This allows other components to react to the healing received.
    /// 
    /// **Important:** Base class healing handling (if applicable) should be called at the **end** of this method's logic for proper inheritance behavior.
    /// </remarks>
    public virtual void HealObject(IDamageData.HealModuleData healModuleData) 
    {
        //call for heal events
        m_scriptOwner.onHealEvent.Invoke(healModuleData, m_scriptOwner, ICoreModule.ModuleStateChangeType.Health);
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// Heals THIS entity using a fixed rate that is stored in <see cref="SO_ModuleHealthData"/> 
    /// </summary>
    /// <param name="healModuleData">The data neede to heal the entity by what amount and what rate.</param>
    /// <returns></returns>
    protected IEnumerator HealEntityAction(IDamageData.HealModuleData healModuleData)
    {
        //save some initial values so we dont forget where we are
        int amountModuleHealedThisAction = 0;
        int healRate = healModuleData.rateOfApplication;
        int healValue = healModuleData.amountToHeal;

        //i increment upwards because idk, cry about it
        while (amountModuleHealedThisAction < healValue)
        {
            amountModuleHealedThisAction += healRate;
            m_moduleHealth += healRate;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Damages THIS entity using the modules rate of damage per frame that i
    /// </summary>
    /// <param name="damageVal"></param>
    /// <returns></returns>
    protected IEnumerator DamageEntityAction(int damageVal)
    {
        //save some initial values so we dont forget where we are
        int amountModuleDamagedThisAction = 0;
        int damageRate = ModuleHealthData.rateOfDamage;

        //i increment upwards because idk, cry about it
        while (amountModuleDamagedThisAction < damageVal)
        {
            amountModuleDamagedThisAction += damageRate;
            m_moduleHealth -= damageRate;
            //check if damage can still be applied
            if (m_moduleHealth <= 0)
            {
                //if the modules dead, we should cancel the rest of the application and call the unity event
                m_scriptOwner.onDeathEvent.Invoke(m_scriptOwner, ICoreModule.ModuleStateChangeType.Destroyed);
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region Optional Damage Calculation

    //GEN Comments:
    //armor do a reduction if within 1 move of type, pen on eqiv type, and overpen if normal type
    //EDIT: the scale has been set up in the SO_ModuleHealthData script, its a lil janky but it works
    protected int FindDamageApplicable(IDamageData.WeaponCollisionData weaponCollisionData)
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

    protected int EvaluateDamageApplicationCurve(int caseVal)
    {
        //create the offset
        caseVal += 5;
        //0 case should be 5th object, and -4 whould be first object
        int tableValue = ModuleHealthData.damageApplicationCurveList[caseVal].value;
        return tableValue;
    }

    protected int CalculateDamageAfterArmor(int damage, int damagePercent)
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Project VRS/Module Health Data")]
[Serializable]
public class SO_ModuleHealthData : ScriptableObject
{
    /// <summary>
    /// the type of armor the entity is wearing.
    /// </summary>
    [SerializeField]
    public IDamageData.ArmorType armorType;

    /// <summary>
    /// the maximum health points of the entity.
    /// </summary>
    [SerializeField]
    public int healthMax;

    /// <summary>
    /// The rate of damage that this module can take in a given damage tick.
    /// </summary>
    [SerializeField]
    public int rateOfDamage;

    /// <summary>
    /// Hard coded values as a lookup table for damage reduction or increase.
    /// </summary>
    [SerializeField]
    public List<DamageValue> damageApplicationCurveList = new List<DamageValue>()
    {
        new DamageValue("30%", 30), new DamageValue("50%", 50), new DamageValue("70%", 70), new DamageValue("90%", 90),
        new DamageValue("100%", 100),
        new DamageValue("125%", 120), new DamageValue("150%", 150), new DamageValue("175%", 175), new DamageValue("200%", 200),
    };

    [Serializable]
    public struct DamageValue
    {
        //needed only to tell the values without opening the struct up
        [SerializeField]
        string name;

        [SerializeField]
        public int value;

        public DamageValue(string name, int value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
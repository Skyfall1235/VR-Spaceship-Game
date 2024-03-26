using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Module Health Data")]
[Serializable]
public class SO_ModuleHealthData : ScriptableObject
{
    /// <summary>
    /// Gets or sets the type of armor the entity is wearing.
    /// </summary>
    [SerializeField]
    public IDamageData.ArmorType armorType;

    /// <summary>
    /// Gets or sets the maximum health points of the entity.
    /// </summary>
    [SerializeField]
    public int healthMax;

    /// <summary>
    /// Gets or sets an animation curve used for modifying damage dealt or received.
    /// </summary>
    /// <remarks>to work this properly, the value should be multiplied by like, 100 to 1000 to get the percent applicable.
    /// chart should go from 0 to the multiple of 100% that the maximum should be allowed to have
    /// </remarks>
    [SerializeField]
    public AnimationCurve damageApplicationCurve;

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
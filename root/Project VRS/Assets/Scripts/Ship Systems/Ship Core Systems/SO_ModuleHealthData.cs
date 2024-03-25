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
    public IDamageData.ArmorType armorType { get; set; }

    /// <summary>
    /// Gets or sets the current health points of the entity.
    /// </summary>
    public int health { get; set; }

    /// <summary>
    /// Gets or sets the maximum health points of the entity.
    /// </summary>
    public int healthMax { get; set; }

    /// <summary>
    /// Gets or sets an animation curve used for modifying damage dealt or received.
    /// </summary>
    public AnimationCurve DamageCurve { get; set; }
}

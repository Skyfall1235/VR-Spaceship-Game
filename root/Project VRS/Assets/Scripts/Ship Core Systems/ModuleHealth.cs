using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalModuleHealth : MonoBehaviour
{
    //health and armor is gonna be silly
    //light, medium, heavy, and super heavy armor type
    //armor do a reduction if within 1 move of type, pen on eqiv type, and overpen if normal type
    //scratch that, energy, AP, normal, explosive, and flak

    public enum ArmorType
    {
        None,
        Light,
        Medium,
        Heavy,
        SuperHeavy
    }

    public enum WeaponType
    {
        energy,
        ArmorPenetrating,
        Standard,
        Explosive,
        Flak
    }


    public ArmorType armorType;
    public int health;
    public int healthMax;
    //use a scriptable object here?

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void HealModule(int healAmount)
    {
        throw new System.NotImplementedException();
    }

    //the info about the weaon that hit the ship, and the amount of damage it does and the armor pen type
    public struct WeaponCollisionData
    {
        WeaponType weaponType;
        int damageVal;
        //what else can be put here? CALLS FOR SPECIFIC STATUS EFFECTS?
    }


}

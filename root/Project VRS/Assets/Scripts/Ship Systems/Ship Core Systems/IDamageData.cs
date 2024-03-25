using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageData
{
    /// <summary>
    /// Defines the different types of armor available.
    /// </summary>
    /// <remarks>
    /// weapons should have an armorpiecing value between 0 and 4. this value represents the 'strength' of the attack. the <see cref="WeaponType"/> is organised  from weakest to strongest
    public enum ArmorType
    {
        /// <summary>
        /// No armor equipped.
        /// </summary>
        None,

        /// <summary>
        /// Lightweight armor offering basic protection.
        /// </summary>
        Light,

        /// <summary>
        /// Medium armor providing a balance of protection.
        /// </summary>
        Medium,

        /// <summary>
        /// Heavy armor offering significant protection.
        /// </summary>
        Heavy,

        /// <summary>
        /// Extremely heavy armor with the highest level of protection.
        /// </summary>
        SuperHeavy
    }

    /// <summary>
    /// Defines the different types of weapons available.
    /// </summary>
    public enum WeaponType
    {
        /// <summary>
        /// Weapon that deals primarily energy damage.
        /// </summary>
        energy,

        /// <summary>
        /// Balanced weapon dealing standard damage.
        /// </summary>
        Standard,

        /// <summary>
        /// Weapon that deals high damage in a short burst, often used against shields or light armor.
        /// </summary>
        Flak,

        /// <summary>
        /// Weapon that deals area-of-effect explosive damage.
        /// </summary>
        Explosive,

        /// <summary>
        /// Weapon designed to pierce through armor effectively.
        /// </summary>
        ArmorPenetrating
    }


    /// <summary>
    /// Stores data related to a weapon collision, including the weapon type, damage value, and affected module.
    /// </summary>
    public struct WeaponCollisionData
    {
        /// <summary>
        /// The type of weapon that caused the collision.
        /// </summary>
        public WeaponType weaponType;

        /// <summary>
        /// The amount of damage inflicted by the weapon collision.
        /// </summary>
        public int damageVal;

        /// <summary>
        /// The specific BC_CoreModule that was hit by the weapon.
        /// </summary>
        public BC_CoreModule moduleHit;

        /// <summary>
        /// Creates a new WeaponCollisionData instance with the specified properties.
        /// </summary>
        /// <param name="weaponType">The type of weapon that caused the collision.</param>
        /// <param name="damageVal">The amount of damage inflicted.</param>
        /// <param name="moduleHit">The BC_CoreModule that was hit.</param>
        public WeaponCollisionData(WeaponType weaponType, int damageVal, BC_CoreModule moduleHit)
        {
            this.weaponType = weaponType;
            this.damageVal = damageVal;
            this.moduleHit = moduleHit;
        }
    }

    /// <summary>
    /// Stores data related to healing a module, including the amount of healing and the rate of application.
    /// </summary>
    public struct HealModuleData
    {
        /// <summary>
        /// The amount of health to be restored to the module.
        /// </summary>
        public int amountToHeal;

        /// <summary>
        /// The rate at which the healing should be applied (e.g., health per second).
        /// </summary>
        public float rateOfApplication;

        /// <summary>
        /// Creates a new HealModuleData instance with the specified properties.
        /// </summary>
        /// <param name="amountToHeal">The amount of health to heal.</param>
        /// <param name="rateOfApplication">The rate at which to apply the healing.</param>
        public HealModuleData(int amountToHeal, float rateOfApplication)
        {
            this.amountToHeal = amountToHeal;
            this.rateOfApplication = rateOfApplication;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    [field:SerializeField] public List<WeaponSlot> WeaponSlots { get; private set; } = new List<WeaponSlot>();
    public LinkedList<WeaponSlot> PlayerWeapons { get; private set; } = new LinkedList<WeaponSlot>();
    
    LinkedListNode<WeaponSlot> m_selectedWeapon;
    bool m_lastFireState = false;

    /// <summary>
    /// Struct containing the weapon refernce, the weapons transform, and the size of the weapon
    /// </summary>
    [System.Serializable]
    public struct WeaponSlot
    {
        public BC_Weapon Weapon;
        public Transform Transform;
        public SO_WeaponData.WeaponSize WeaponSize;
        public WeaponSlot(Transform newPosition, SO_WeaponData.WeaponSize weaponSize, BC_Weapon newWeapon = null)
        {
            Weapon = newWeapon;
            Transform = newPosition;
            WeaponSize = weaponSize;
        }
    }

    /// <summary>
    /// Stores the firing state (true/false) from the last mouse down event.
    /// This value is used to update the firing state of the selected weapon when it changes.
    /// </summary>
    public bool LastFireState
    { 
        get 
        { 
            return m_lastFireState; 
        }
        set 
        {
            m_lastFireState = value;
            if (m_selectedWeapon != null && m_selectedWeapon.Value.Weapon != null)
            {
                m_selectedWeapon.Value.Weapon.UpdateFiringState(value);

            }
        }
    }

    #region registration, deregistration, creation, and destruction of weapons 

    /// <summary>
    /// Registers a weapon to a weapon slot on the player.
    /// </summary>
    /// <param name="weaponToAdd">The weapon to register.</param>
    /// <param name="transform">The transform to attach the weapon to. If null, it will find an available slot.</param>
    /// <param name="autoManageWeapon">If true, the weapon will be parented to the transform and reset its local position and rotation.</param>
    /// <returns>True if the weapon was registered successfully, false otherwise.</returns>
    public bool RegisterWeapon(BC_Weapon weaponToAdd,Transform transform = null, bool autoManageWeapon = true)
    {
        if(transform == null)
        {
            // Find an available weapon slot with sufficient size for the weapon
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if ((int)WeaponSlots[i].WeaponSize >= (int)weaponToAdd.WeaponData.RequiredHardpointSize && WeaponSlots[i].Weapon == null)
                {
                    WeaponSlots[i] = new WeaponSlot(WeaponSlots[i].Transform, WeaponSlots[i].WeaponSize, weaponToAdd);
                    if(WeaponSlots[i].Weapon.WeaponBase != null && autoManageWeapon)
                    {
                        WeaponSlots[i].Weapon.WeaponBase.parent = WeaponSlots[i].Transform;
                        WeaponSlots[i].Weapon.WeaponBase.localPosition = Vector3.zero;
                        WeaponSlots[i].Weapon.WeaponBase.localRotation = Quaternion.identity;
                    }
                    if (!WeaponSlots[i].Weapon.WeaponData.AutoFiring)
                    {
                        PlayerWeapons.AddLast(WeaponSlots[i]);
                    }
                    return true;
                }
            }
            return false;
        }
        else
        {
            // Find the specified weapon slot and register the weapon if it's available and has sufficient size
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if(WeaponSlots[i].Transform == transform && WeaponSlots[i].Weapon == null && (int)WeaponSlots[i].WeaponSize >= (int)weaponToAdd.WeaponData.RequiredHardpointSize)
                {
                    WeaponSlots[i] = new WeaponSlot(WeaponSlots[i].Transform, WeaponSlots[i].WeaponSize,weaponToAdd);
                    if (WeaponSlots[i].Weapon.WeaponBase != null && autoManageWeapon)
                    {
                        WeaponSlots[i].Weapon.WeaponBase.parent = WeaponSlots[i].Transform;
                        WeaponSlots[i].Weapon.WeaponBase.localPosition = Vector3.zero;
                        WeaponSlots[i].Weapon.WeaponBase.localRotation = Quaternion.identity;
                    }
                    if (!WeaponSlots[i].Weapon.WeaponData.AutoFiring)
                    {
                        PlayerWeapons.AddLast(WeaponSlots[i]);
                    }
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Deregisters a weapon from the player.
    /// </summary>
    /// <param name="weaponToRemove">The specific weapon to deregister (optional).</param>
    /// <param name="position">The transform position of the weapon to deregister (optional).</param>
    /// <param name="autoManageWeapon">If true, the weapon will be parented to the specified transform (if provided).</param>
    /// <returns>True if the weapon was deregistered successfully, false otherwise.</returns>
    public bool DeregisterWeapon(BC_Weapon weaponToRemove = null, Transform position = null, bool autoManageWeapon = true)
    {
        if (position != null)
        {
            // Find the weapon slot at the specified position and deregister it
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if(WeaponSlots[i].Transform == position)
                {
                    BC_Weapon removedWeapon = WeaponSlots[i].Weapon;
                    if (!removedWeapon.WeaponData.AutoFiring)
                    {
                        PlayerWeapons.Remove(PlayerWeapons.Find(WeaponSlots[i]));
                    }
                    WeaponSlots[i] = new WeaponSlot(WeaponSlots[i].Transform, WeaponSlots[i].WeaponSize, null);
                    if(removedWeapon.WeaponBase != null && autoManageWeapon)
                    {
                        WeaponSlots[i].Weapon.WeaponBase.parent = transform;
                    }
                    return true;
                }
            }
            return false;
        }
        if(weaponToRemove != null)
        {
            for(int i = 0; i < WeaponSlots.Count; i++)
            {
                if (WeaponSlots[i].Weapon == weaponToRemove)
                {
                    BC_Weapon removedWeapon = WeaponSlots[i].Weapon;
                    if (!removedWeapon.WeaponData.AutoFiring)
                    {
                        PlayerWeapons.Remove(PlayerWeapons.Find(WeaponSlots[i]));
                    }
                    WeaponSlots[i] = new WeaponSlot(position, WeaponSlots[i].WeaponSize, null);
                    if (removedWeapon.WeaponBase != null && autoManageWeapon)
                    {
                        WeaponSlots[i].Weapon.WeaponBase.parent = transform;
                    }
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    /// <summary>
    /// Returns the currently selected weapon
    /// </summary>
    /// <returns></returns>
    public BC_Weapon RetrieveSelectedWeapon()
    {
        return m_selectedWeapon.Value.Weapon;
    }

    /// <summary>
    /// Gets a list of all currently equipped BC_Weapons on the player.
    /// </summary>
    /// <returns>A list of BC_Weapon objects representing the player's equipped weapons.</returns>
    public List<BC_Weapon> GetWeapons()
    {
        List<BC_Weapon> listToReturn = new List<BC_Weapon>();
        foreach (WeaponSlot weaponSlot in WeaponSlots)
        {
            if (weaponSlot.Weapon != null)
            {
                listToReturn.Add(weaponSlot.Weapon);
            }
        }
        return listToReturn;
    }

    #endregion

    #region weapon cycling

    public void RotateSelectedWeaponForward()
    {
        if(m_selectedWeapon != null)
        {
            m_selectedWeapon.Value.Weapon.UpdateFiringState(false);
            m_selectedWeapon = m_selectedWeapon.NextOrFirst();
            m_selectedWeapon.Value.Weapon.UpdateFiringState(m_lastFireState);
        }
    }

    public void RotateSelectedWeaponBackward()
    {
        if (m_selectedWeapon != null)
        {
            m_selectedWeapon.Value.Weapon.UpdateFiringState(false);
            m_selectedWeapon = m_selectedWeapon.PreviousOrLast();
            m_selectedWeapon.Value.Weapon.UpdateFiringState(m_lastFireState);
        }
    }

    #endregion

    #region Monobehavior Methods 

    private void Update()
    {
        //DEBUG
        if(Input.GetMouseButtonDown(0)) 
        {
            LastFireState = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            LastFireState = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            RotateSelectedWeaponForward();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            m_selectedWeapon.Value.Weapon.TryReload();
        }
    }

    public override void Awake()
    {
        base.Awake();
        BC_Weapon[] foundWeapons = transform.GetComponentsInChildren<BC_Weapon>();
        foreach (BC_Weapon weapon in foundWeapons)
        {
            RegisterWeapon(weapon);
        }
         m_selectedWeapon = (PlayerWeapons.Count <= 0) ? null : PlayerWeapons.First;
    }

    #endregion

    #region Start up and Shutdown Logic

    protected override void PreStartUpLogic()
    {

    }

    protected override void PostStartUpLogic()
    {

    }

    protected override void PreShutDownLogic()
    {

    }

    protected override void PostShutDownLogic()
    {

    }

    #endregion

}

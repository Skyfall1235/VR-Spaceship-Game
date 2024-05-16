using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    [field:SerializeField] public List<WeaponSlot> WeaponSlots { get; private set; } = new List<WeaponSlot>();
    public LinkedList<WeaponSlot> PlayerWeapons { get; private set; } = new LinkedList<WeaponSlot>();
    public List<BC_Weapon> GetWeapons() 
    {
        List<BC_Weapon> listToReturn = new List<BC_Weapon>();
        foreach(WeaponSlot weaponSlot in WeaponSlots)
        {
            if(weaponSlot.Weapon != null)
            {
                listToReturn.Add(weaponSlot.Weapon);
            }
        } 
        return listToReturn;
    }
    LinkedListNode<WeaponSlot> _selectedWeapon;
    bool _lastMouseDownStatus = false;

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

    public bool LastMouseDownStatus
    { 
        get 
        { 
            return _lastMouseDownStatus; 
        }
        set 
        {
            _lastMouseDownStatus = value;
            if (_selectedWeapon != null && _selectedWeapon.Value.Weapon != null)
            {
                _selectedWeapon.Value.Weapon.UpdateFiringState(value);

            }
        }
    }

    #region registration, deregistration, creation, and destruction of weapons 

    public bool RegisterWeapon(BC_Weapon weaponToAdd,Transform transform = null, bool autoManageWeapon = true)
    {
        if(transform == null)
        {
            for(int i = 0; i < WeaponSlots.Count; i++)
            {
                if ((int)WeaponSlots[i].WeaponSize >= (int)weaponToAdd.WeaponData.RequiredHardpointSize && WeaponSlots[i].Weapon == null)
                {
                    WeaponSlots[i] = new WeaponSlot(WeaponSlots[i].Transform, WeaponSlots[i].WeaponSize, weaponToAdd);
                    if(WeaponSlots[i].Weapon.WeaponBase != null && autoManageWeapon)
                    {
                        WeaponSlots[i].Weapon.WeaponBase.position = WeaponSlots[i].Transform.position;
                        WeaponSlots[i].Weapon.WeaponBase.parent = WeaponSlots[i].Transform;
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
            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                if(WeaponSlots[i].Transform == transform && WeaponSlots[i].Weapon == null && (int)WeaponSlots[i].WeaponSize >= (int)weaponToAdd.WeaponData.RequiredHardpointSize)
                {
                    WeaponSlots[i] = new WeaponSlot(WeaponSlots[i].Transform, WeaponSlots[i].WeaponSize,weaponToAdd);
                    if (WeaponSlots[i].Weapon.WeaponBase != null && autoManageWeapon)
                    {
                        WeaponSlots[i].Weapon.WeaponBase.position = WeaponSlots[i].Transform.position;
                        WeaponSlots[i].Weapon.WeaponBase.parent = WeaponSlots[i].Transform;
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

    public bool DeregisterWeapon(BC_Weapon weaponToRemove = null, Transform position = null, bool autoManageWeapon = true)
    {
        if (position != null)
        {
            for(int i = 0; i < WeaponSlots.Count; i++)
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

    #endregion

    #region weapon cycling

    void RotateSelectedWeaponForward()
    {
        if(_selectedWeapon != null)
        {
            _selectedWeapon.Value.Weapon.UpdateFiringState(false);
            _selectedWeapon = _selectedWeapon.NextOrFirst();
            _selectedWeapon.Value.Weapon.UpdateFiringState(_lastMouseDownStatus);
        }
    }

    void RotateSelectedWeaponBackward()
    {
        if (_selectedWeapon != null)
        {
            _selectedWeapon.Value.Weapon.UpdateFiringState(false);
            _selectedWeapon = _selectedWeapon.PreviousOrLast();
            _selectedWeapon.Value.Weapon.UpdateFiringState(_lastMouseDownStatus);
        }
    }

    #endregion

    #region Monobehavior Methods 

    private void Update()
    {
        //DEBUG
        if(Input.GetMouseButtonDown(0)) 
        {
            LastMouseDownStatus = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            LastMouseDownStatus = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            RotateSelectedWeaponForward();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            _selectedWeapon.Value.Weapon.UpdateReloadState(true);
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
         _selectedWeapon = (PlayerWeapons.Count <= 0) ? null : PlayerWeapons.First;
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

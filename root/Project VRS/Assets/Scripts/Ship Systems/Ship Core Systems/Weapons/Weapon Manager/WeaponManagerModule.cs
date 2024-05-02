
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    [SerializeField]List<Transform> _weaponSlotPositions = new List<Transform>();
    public LinkedList<WeaponSlot> _weapons { get; private set; } = new LinkedList<WeaponSlot>();
    public List<BC_Weapon> GetWeapons() 
    {
        List<BC_Weapon> listToReturn = new List<BC_Weapon>();
        foreach(WeaponSlot weaponSlot in _weapons)
        {
            listToReturn.Add(weaponSlot.Weapon);
        } 
        return listToReturn;
    }
    LinkedListNode<WeaponSlot> _selectedWeapon;
    bool _lastMouseDownStatus = false;

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
            if (_selectedWeapon.Value.Weapon != null)
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
            for(LinkedListNode<WeaponSlot> node = _weapons.First; node != null; node = node.Next)
            {
                if(node.Value.Weapon == null && (int)node.Value.WeaponSize >= (int)weaponToAdd.WeaponData.RequiredHardpointSize)
                {
                    node.Value = new WeaponSlot(node.Value.Transform, node.Value.WeaponSize, weaponToAdd);
                    if(node.Value.Weapon.WeaponBase != null && autoManageWeapon)
                    {
                        node.Value.Weapon.WeaponBase.position = node.Value.Transform.position;
                        node.Value.Weapon.WeaponBase.parent = node.Value.Transform;
                    }
                    return true;
                }
            }
            return false;
        }
        else
        {
            for (LinkedListNode<WeaponSlot> node = _weapons.First; node != null; node = node.Next)
            {
                if( node.Value.Transform == transform && node.Value.Weapon == null && (int)node.Value.WeaponSize >= (int)weaponToAdd.WeaponData.RequiredHardpointSize)
                {
                    node.Value = new WeaponSlot(node.Value.Transform, node.Value.WeaponSize,weaponToAdd);
                    if (node.Value.Weapon.WeaponBase != null && autoManageWeapon)
                    {
                        node.Value.Weapon.WeaponBase.position = node.Value.Transform.position;
                        node.Value.Weapon.WeaponBase.parent = node.Value.Transform;
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
            for(LinkedListNode<WeaponSlot> node = _weapons.First; node != null; node = node.Next)
            {
                if(node.Value.Transform == position)
                {
                    BC_Weapon removedWeapon = node.Value.Weapon;
                    node.Value = new WeaponSlot(node.Value.Transform, node.Value.WeaponSize, null);
                    if(removedWeapon.WeaponBase != null && autoManageWeapon)
                    {
                        node.Value.Weapon.WeaponBase.parent = transform;
                    }
                    return true;
                }
            }
            return false;
        }
        if(weaponToRemove != null)
        {
            for(LinkedListNode<WeaponSlot> node = _weapons.First; node != null; node = node.Next)
            {
                if(node.Value.Weapon == weaponToRemove)
                {
                    BC_Weapon removedWeapon = node.Value.Weapon;
                    node.Value = new WeaponSlot(position, node.Value.WeaponSize, null);
                    if (removedWeapon.WeaponBase != null && autoManageWeapon)
                    {
                        node.Value.Weapon.WeaponBase.parent = transform;
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
            _selectedWeapon = _selectedWeapon.NextOrFirst();
        }
    }

    void RotateSelectedWeaponBackward()
    {
        if (_selectedWeapon != null)
        {
            _selectedWeapon = _selectedWeapon.PreviousOrLast();
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
    }

    public override void Awake()
    {
        base.Awake();
        foreach(Transform transform in _weaponSlotPositions)
        {
            _weapons.AddLast(new WeaponSlot(transform, SO_WeaponData.WeaponSize.Large));
        }
        BC_Weapon[] foundWeapons = transform.GetComponentsInChildren<BC_Weapon>();
        foreach (BC_Weapon weapon in foundWeapons)
        {
            RegisterWeapon(weapon);
        }
         _selectedWeapon = (_weapons.Count <= 0) ? null : _weapons.First;
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

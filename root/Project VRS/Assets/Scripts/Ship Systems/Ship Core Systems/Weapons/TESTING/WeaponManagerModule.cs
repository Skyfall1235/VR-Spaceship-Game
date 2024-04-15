using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    [SerializeField]List<Transform> _weaponSlotPositions = new List<Transform>();
    public LinkedList<WeaponSlot> _weapons { get; private set; } = new LinkedList<WeaponSlot>();
    public List<Weapon> GetWeapons() 
    {
        List<Weapon> listToReturn = new List<Weapon>();
        foreach(WeaponSlot weaponSlot in _weapons)
        {
            listToReturn.Add(weaponSlot.weapon);
        } 
        return listToReturn;
    }
    LinkedListNode<WeaponSlot> _selectedWeapon;
    bool _lastMouseDownStatus = false;

    public struct WeaponSlot
    {
        public Weapon weapon;
        public Transform transform;
        public WeaponSlot(Transform newPosition, Weapon newWeapon = null)
        {
            weapon = newWeapon;
            transform = newPosition;
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
            if (_selectedWeapon != null)
            {
                _selectedWeapon.Value.weapon.UpdateFiringState(value);

            }
        }
    }

    #region registration, deregistration, creation, and destruction of weapons 

    public bool RegisterWeapon(Weapon weaponToAdd,Transform transform = null, bool autoManageWeapon = true)
    {
        if(transform == null)
        {
            for(LinkedListNode<WeaponSlot> node = _weapons.First; node != null; node = node.Next)
            {
                if(node.Value.weapon == null)
                {
                    node.Value = new WeaponSlot(node.Value.transform, weaponToAdd);
                    if(node.Value.weapon.transform != null && autoManageWeapon)
                    {
                        node.Value.weapon.transform.position = node.Value.transform.position;
                        node.Value.weapon.transform.parent = node.Value.transform;
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
                if( node.Value.transform == transform && node.Value.weapon != null)
                {
                    node.Value = new WeaponSlot(node.Value.transform, weaponToAdd);
                    if (node.Value.weapon.transform != null && autoManageWeapon)
                    {
                        node.Value.weapon.transform.position = node.Value.transform.position;
                        node.Value.weapon.transform.parent = node.Value.transform;
                    }
                    return true;
                }
            }
            return false;
        }
    }

    public bool DeregisterWeapon(Weapon weaponToRemove = null, Transform position = null, bool autoManageWeapon = true)
    {
        if (position != null)
        {
            for(LinkedListNode<WeaponSlot> node = _weapons.First; node != null; node = node.Next)
            {
                if(node.Value.transform == position)
                {
                    Weapon removedWeapon = node.Value.weapon;
                    node.Value = new WeaponSlot(node.Value.transform, null);
                    if(removedWeapon.transform != null && autoManageWeapon)
                    {
                        node.Value.weapon.transform.parent = transform;
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
                if(node.Value.weapon == weaponToRemove)
                {
                    Weapon removedWeapon = node.Value.weapon;
                    node.Value = new WeaponSlot(position, null);
                    if (removedWeapon.transform != null && autoManageWeapon)
                    {
                        node.Value.weapon.transform.parent = transform;
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

    private void Awake()
    {
        foreach(Transform transform in _weaponSlotPositions)
        {
            _weapons.AddLast(new WeaponSlot(transform));
        }
        Weapon[] foundWeapons = transform.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in foundWeapons)
        {
            RegisterWeapon(weapon, null, false);
        }
         _selectedWeapon = (_weapons.Count <= 0) ? null : _weapons.First;
    }

    #endregion

    #region Base class Methods

    public override void ReleaseResources()
    {
        throw new NotImplementedException();
    }

    public override void ShutDown()
    {
        throw new NotImplementedException();
    }

    public override void StartUp()
    {
        throw new NotImplementedException();
    }

    public override void Reboot()
    {
        throw new NotImplementedException();
    }

    #endregion

}

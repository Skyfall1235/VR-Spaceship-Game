using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    LinkedList<Weapon> _weapons = new LinkedList<Weapon>();
    public LinkedList<Weapon> GetWeapons() { return _weapons; }
    LinkedListNode<Weapon> _selectedWeapon;
    bool _lastMouseDownStatus = false;
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
                _selectedWeapon.Value.UpdateFireState(value);
            }
        }
    }
    #region registration, deregistration, creation, and destruction of weapons 
    public void RegisterWeapons(params GameObject[] weapons)
    {
        foreach(GameObject weapon in weapons)
        {
            if(weapon.GetComponent<Weapon>() != null)
            {
                _weapons.AddLast(weapon.gameObject.GetComponent<Weapon>());
            }
        }
    }
    public void RegisterWeapons(params Weapon[] weapons)
    {
        foreach(Weapon weapon in weapons)
        {
            if(weapon.gameObject != null)
            {
                _weapons.AddLast(weapon);
            }
        }
    }
    public void DeregisterWeapons(params GameObject[] weapons)
    {
        foreach (GameObject weapon in weapons)
        {
            if(weapon.GetComponent<Weapon>() != null && _weapons.Contains(weapon.GetComponent<Weapon>()))
            {
                _weapons.Remove(weapon.GetComponent<Weapon>());
            }
        }
    }
    public void DeregisterWeapons(params Weapon[] weapons)
    {
        foreach (Weapon weapon in weapons)
        {
            if ( _weapons.Contains(weapon.GetComponent<Weapon>()))
            {
                _weapons.Remove(weapon);
            }
        }
    }

    #endregion
    #region weapon roatation
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
    private void Update()
    {
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
        if(transform.childCount > 0) 
        {
            for(int i = 0; i < transform.childCount; i++) 
            {
                RegisterWeapons(transform.GetChild(i).gameObject);
            }
        }
         _selectedWeapon = (_weapons.Count <= 0) ? null : _weapons.First;
    }
}

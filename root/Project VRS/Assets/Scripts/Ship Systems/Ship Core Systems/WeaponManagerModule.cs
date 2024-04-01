using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    LinkedList<Weapon> _weapons = new LinkedList<Weapon>();
    public LinkedList<Weapon> GetWeapons() { return _weapons; }

    #region registration and deregistration of weapons 
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
    LinkedListNode<Weapon> _selectedWeapon;
    void RotateSelectedWeaponForward()
    {
        if(_selectedWeapon != null)
        {
            _selectedWeapon = CircularLinkedList.NextOrFirst( _selectedWeapon);
        }
    }
    void RotateSelectedWeaponBackward()
    {
        if (_selectedWeapon != null)
        {
            _selectedWeapon = CircularLinkedList.PreviousOrLast(_selectedWeapon);
        }
    }
    public void Fire()
    {
        if(_selectedWeapon != null) 
        {
            _selectedWeapon.Value.Fire();
        }
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        {
            Fire();
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

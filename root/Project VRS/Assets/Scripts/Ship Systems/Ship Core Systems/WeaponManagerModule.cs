using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerModule : BC_CoreModule
{
    CircularLinkedList<Weapon> _weapons = new CircularLinkedList<Weapon>();
    public CircularLinkedList<Weapon> GetWeapons() { return _weapons; }

    public void RegisterWeapons(params GameObject[] weapons)
    {
        foreach(GameObject weapon in weapons)
        {
            if(weapon.GetComponent<Weapon>() != null)
            {
                _weapons.Add(weapon.gameObject.GetComponent<Weapon>());
            }
        }
    }
    public void RegisterWeapons(params Weapon[] weapons)
    {
        foreach(Weapon weapon in weapons)
        {
            if(weapon.gameObject != null)
            {
                _weapons.Add(weapon);
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

    int _index;
    int WeaponIndex
    {
        get 
        { 
            return _index; 
        }
        set 
        {
            _index = value;
        }
    }

    
    public void IncrementThroughWeapons(int count = 1)
    {
        WeaponIndex += count;
    }
    public void DecrementThroughWeapons(int count = 1)
    {
        WeaponIndex -= count;
    }
    public Weapon SelectedWeapon { get; private set; } = null; 
    public void Fire()
    {
        if(SelectedWeapon != null) 
        {
            SelectedWeapon.Fire();
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
        WeaponIndex = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ShootAndCycle : ControllerInputManager
{
    public TextMeshProUGUI weaponSelectedText;
    public WeaponManagerModule weaponManager;

    private void Awake()
    {
        
    }

    void SetSelectedText()
    {
        string selectedWeaponName = weaponManager.RetrieveSelectedWeapon().name;
        weaponSelectedText.text = selectedWeaponName;
    }

}


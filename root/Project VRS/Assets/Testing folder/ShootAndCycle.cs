using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ShootAndCycle : ControllerInputManager
{
    public TextMeshProUGUI weaponSelectedText;
    public WeaponManagerModule weaponManager;
    public override void PrimaryUpdate(ActionValues? primary)
    {

    }

    public override void SecondaryUpdate(ActionValues? secondary) 
    {
    
    }

    private void DetermineActions(ActionValues? value)
    {

    }

    void SetSelectedText()
    {
        //weaponManager.
    }
}

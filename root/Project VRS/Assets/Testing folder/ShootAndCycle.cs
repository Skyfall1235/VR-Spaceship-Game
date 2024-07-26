using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootAndCycle : MonoBehaviour
{
    public TextMeshProUGUI weaponSelectedText;
    public WeaponManagerModule weaponManager;
    public TwoHandedControllerInputHandler handler;

    private void Update()
    {
        SetSelectedText();
        if (handler.PrimaryValuesProperties.HasValues == false)
        {
            Debug.Log("does not have values");
            return;
        }
        else
        {
            Debug.Log("does have value");
        }
        InputAction action  = handler.PrimaryValuesProperties.PrimaryButtonPressProperty.Value.action;
        InputAction trigger = handler.PrimaryValuesProperties.TriggerPressProperty.Value.action;

        if (action.WasPressedThisFrame())
        {
            Debug.Log("primary button press");
            weaponManager.RotateSelectedWeaponForward();
            
        }
        if(trigger.IsPressed())
        {
            weaponManager.LastFireState = true;
        }
        else
        {
            weaponManager.LastFireState = false;
        }
    }

    void SetSelectedText()
    {
        string selectedWeaponName = weaponManager.RetrieveSelectedWeapon().name;
        weaponSelectedText.text = selectedWeaponName;
    }

}


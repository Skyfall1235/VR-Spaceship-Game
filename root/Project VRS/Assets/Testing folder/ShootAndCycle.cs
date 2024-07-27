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
            return;
        }
        InputAction action  = handler.PrimaryValuesProperties.PrimaryButtonPressProperty.Value.action;
        InputAction trigger = handler.PrimaryValuesProperties.TriggerPressProperty.Value.action;
        Debug.Log(action.name);
        Debug.Log(action.ReadValue<float>());

        if (action.WasPressedThisFrame())
        {
            
            weaponManager.RotateSelectedWeaponForward();
            
        }
        if(trigger.IsPressed())
        {
            weaponManager.LastFireState = true;
        }
        if(trigger.WasReleasedThisFrame()) { weaponManager.LastFireState = false; }
    }

    void SetSelectedText()
    {
        string selectedWeaponName = weaponManager.RetrieveSelectedWeapon().WeaponData.WeaponName;
        weaponSelectedText.text = selectedWeaponName;
    }

}


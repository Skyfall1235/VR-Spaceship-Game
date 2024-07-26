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
        
        if (action.WasPerformedThisFrame())
        {
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


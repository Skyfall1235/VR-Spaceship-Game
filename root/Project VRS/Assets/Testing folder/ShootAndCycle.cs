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
        if ((handler.PrimaryValuesProperties == null))
        {
            return;
        }
        InputAction action  = handler.PrimaryValuesProperties.PrimaryButtonPressProperty.Value.action;
        InputAction trigger = handler.PrimaryValuesProperties.TriggerPressProperty.Value.action;
        if (action.WasPerformedThisFrame())
        {
            weaponManager.RotateSelectedWeaponForward();
            SetSelectedText();
        }
        //if()
    }

    void SetSelectedText()
    {
        string selectedWeaponName = weaponManager.RetrieveSelectedWeapon().name;
        weaponSelectedText.text = selectedWeaponName;
    }

}


using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShootAndCycle : MonoBehaviour
{
    public TextMeshProUGUI weaponSelectedText;
    public Slider throttleSlider;
    public WeaponManagerModule weaponManager;
    public TwoHandedControllerInputHandler handler;
    public PlayerShipInputHandler playerShipInputHandler;

    bool armedStatus = false;


    private void Update()
    {
        SetSelectedText();
        if (handler.PrimaryValuesProperties.HasValues == false)
        {
            return;
        }
        if(armedStatus == false)
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
        if(armedStatus)
        {
            string selectedWeaponName = weaponManager.RetrieveSelectedWeapon().WeaponData.WeaponName;
            weaponSelectedText.text = selectedWeaponName;
        }
        else
        {
            weaponSelectedText.text = "NO ARM";
        }
        throttleSlider.value = (playerShipInputHandler.SecondaryShipJoystick.value.y / 2) + 0.5f;
    }

    public void ArmToggle()
    {
        armedStatus = !armedStatus;
    }

}


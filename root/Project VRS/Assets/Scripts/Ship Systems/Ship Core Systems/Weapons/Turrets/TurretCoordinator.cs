using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurretRotation))]
[RequireComponent(typeof(TargetingComponent))]
public class TurretCoordinator : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField]
    private BC_Weapon m_turretWeapon;

    [SerializeField] 
    private SO_TurretData m_turretData;

    private TurretRotation m_turretRotationController;

    private TargetingComponent m_turretTargetingComponent;

    #region Monobehavior Methods & their dependencies

    private void Awake()
    {
        LinkRequiredComponents();
    }

    private void OnValidate()
    {
        LinkRequiredComponents();
    }

    private void LinkRequiredComponents()
    {
        if (m_turretRotationController == null)
        {
            m_turretRotationController = GetComponent<TurretRotation>();
        }

        if (m_turretTargetingComponent == null)
        {
            m_turretTargetingComponent = GetComponent<TargetingComponent>();
        }
        
        //check for instantiation point as its crucial for calculations and movement
        if (m_turretWeapon == null)
        {
            Debug.LogError($"TurretCoordinator {this.gameObject.name} missing turret Weapon reference, stoppping functionality");
            this.enabled = false;
            return;
        }

        //share the necessary data across the sub components
        m_turretTargetingComponent.WeaponData = m_turretWeapon.WeaponData;
        m_turretRotationController.TurretData = m_turretData;

        //setup instantiation point
        m_turretTargetingComponent.ProjectileInstantiationPoint = m_turretWeapon.InstantiationPoint;
    }



    //see if the rotation is within our gimbal limits
    private bool CheckIfTargetIsWithinGimbalLimits(GameObject targetGameObject)
    {
        return false;
    }

    //retrive info from target handler, and be able to handle if theres nothing in the handler and respond accordingly
    bool ChooseBestTargetIfAvailable(List<TargetData> sortedPriorityTargets, out TargetData bestTarget)
    {
        bestTarget = new TargetData();
        return false;
    }


    //set the rotation for the target if it is within the gimbal limits and we have a target, and also how often should we call it??

    #endregion

    
}

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

    public TargetData? m_priorityTarget;

    bool shouldRotate;

    #region Monobehavior Methods & their dependencies

    private void Awake()
    {
        LinkRequiredComponents();
    }

    private void OnValidate()
    {
        LinkRequiredComponents();
    }

    private void FixedUpdate()
    {
        if(shouldRotate) 
        {
            
        }


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
            Debug.LogError($"TurretCoordinator {this.gameObject.name} missing turret Weapon reference, stopping functionality");
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
    private bool CheckIfTargetIsWithinGimbalLimits(TargetData data)
    {
        return false;
    }

    /// <summary>
    /// Chooses the best target from the provided list if one is available within gimbal limits.
    /// </summary>
    /// <param name="sortedPriorityTargets">A list of target data sorted by priority (highest first).</param>
    /// <param name="bestTarget">The chosen target data (output).</param>
    /// <returns>True if a target was found within gimbal limits, false otherwise.</returns>
    private bool ChooseBestTargetIfAvailable(List<TargetData> sortedPriorityTargets, out TargetData bestTarget)
    {
        if (sortedPriorityTargets == null)
        {
            // No targets provided, set bestTarget to default and return false
            bestTarget = new TargetData();
            return false;
        }

        // Check pre-selected target first (if any)
        if (m_priorityTarget != null)
        {
            bestTarget = (TargetData)m_priorityTarget;
            return true;
        }

        // Loop through sorted targets
        foreach (TargetData currentTarget in sortedPriorityTargets)
        {
            // Check if target is within gimbal limits
            if (CheckIfTargetIsWithinGimbalLimits(currentTarget))
            {
                bestTarget = currentTarget;
                return true;
            }
        }

        // No target within gimbal limits, set bestTarget to default and return false
        bestTarget = new TargetData();
        return false;
    }


    //set the rotation for the target if it is within the gimbal limits and we have a target, and also how often should we call it??

    private void RotateToBestTargetIfApplicable(TargetData bestTarget)
    {

    }

    #endregion

    
}

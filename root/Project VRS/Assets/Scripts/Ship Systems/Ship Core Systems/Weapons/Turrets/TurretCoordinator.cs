using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the overall behavior of a self-firing turret, including target acquisition, rotation, and firing.
/// Requires TurretRotation, TargetingComponent, and TargetHandler components.
/// </summary>
[RequireComponent(typeof(TurretRotation))]
[RequireComponent(typeof(TargetingComponent))]
public class TurretCoordinator : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private BC_Weapon m_turretWeapon; // the weapon for this self firing turret
    [SerializeField] private SO_TurretData m_turretData; //the turrets data fro how fast it should rotate and fire and do other things
    [SerializeField] private TurretRotation m_turretRotationController; //the rotation component
    [SerializeField] private TargetingComponent m_turretTargetingComponent; //the targeting component
    [SerializeField] private TargetHandler targetHandler; //the component in charge of computing target priority
    public bool shouldRotate;
    [SerializeField] float m_maxAngleDeviationFromTarget = 10f;
    internal bool m_isFiring;

    CustomLogger m_logger;

    #region Monobehavior Methods

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
        if (!shouldRotate)
        {
            return;
        }

        //if there are no targets
        if(targetHandler.RegisteredTargetsIncludingOverride.Count == 0)
        {
            //send the turret back to forward
            m_turretRotationController.TurnToLeadPosition(transform.forward);
            return;
        }

        //we use a bool to cut things short if they fail rather than go through the entire sequence
        bool state;
        DetermineTurretActivity(out state);
        if (!state)
        {
            return;
        }
        if (!TurretFireControl())
        {
            return;
        }
    }

    #endregion

    #region Dependencies

    private void DetermineTurretActivity( out bool state)
    {
        TargetData dataToUse;

        //check the override first
        if (targetHandler.targetOverride.IsEmpty)
        {
            //check if is gimballed
            if (m_turretData.IsGimballed)
            {
                //send the turret back to forward
                m_turretRotationController.TurnToLeadPosition(transform.forward);
                Debug.Log("has target set here");
                //m_turretTargetingComponent.m_hasTarget = false;
                state = false;
            }

            //if not gimballed, we just take the highest priority, check,
            //and go down the list til we find one thats with in gimbal limits
            dataToUse = new TargetData(ChooseBestTargetIfAvailable(targetHandler.RegisteredTargetsIncludingOverride));
        }
        else
        {
            dataToUse = new TargetData(targetHandler.targetOverride);
            
        }
        //detemine target data to use
        //found object is our chosen target
        m_turretTargetingComponent.CurrentTargetData = new TargetData(dataToUse);
        //m_turretTargetingComponent.m_hasTarget = true;
        //Debug.Log("has target set here");
        state = true;
    }

    private bool TurretFireControl()
    {
        //begin caluclation for lead
        Vector3 targetPosition = m_turretTargetingComponent.LeadPositionToTarget;
        //Debug.Log(targetPosition);

        if (!m_turretRotationController.CheckInGimbalLimits(targetPosition))
        {
            //send the turret back to forward
            m_turretRotationController.TurnToLeadPosition(transform.forward);
            //m_turretTargetingComponent.m_hasTarget = false;
            //Debug.Log("has target set here");
            return false;
        }
        else
        {
            m_turretRotationController.TurnToLeadPosition(targetPosition);
        }
        //Debug.Log("has target set here");
        //check the angle between the direction the gun is facing and the target lead prediction
        float degreesBetweenAngles = Quaternion.Angle(Quaternion.Euler(m_turretWeapon.InstantiationPoint.transform.forward), Quaternion.Euler(targetPosition));

        //if shallow, check if the firing bool is true,
        //if not start a coroutine with the bullet reload time
        if (degreesBetweenAngles < m_maxAngleDeviationFromTarget)
        {
            if (m_isFiring)
            {
                StartCoroutine(ControlFiringTiming());
            }
        }
        //if angle is steep, stop firing, dont end corotuine
        else
        {
            m_turretWeapon.UpdateFiringState(false);
        }
        return true;
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
            m_logger.Log($"TurretCoordinator {this.gameObject.name} missing turret Weapon reference, stopping functionality", CustomLogger.LogLevel.Error, CustomLogger.LogCategory.Default, this);
            this.enabled = false;
            return;
        }

        //check for turret data to ensure the turret operates
        if (m_turretData == null)
        {
            m_logger.Log($"TurretCoordinator {this.gameObject.name} missing turret data, stopping functionality", CustomLogger.LogLevel.Error, CustomLogger.LogCategory.Default, this);
            this.enabled = false;
            return;
        }

        //share the necessary data across the sub components
        m_turretTargetingComponent.WeaponData = m_turretWeapon.WeaponData;
        m_turretRotationController.TurretData = m_turretData;

        //setup instantiation point
        m_turretTargetingComponent.ProjectileInstantiationPoint = m_turretWeapon.InstantiationPoint;
    }

    private bool CheckIfTargetIsWithinGimbalLimits(TargetData data)
    {
        return m_turretRotationController.CheckInGimbalLimits(data.TargetGameObject.transform.position);
    }

    private TargetData ChooseBestTargetIfAvailable(List<TargetData> sortedPriorityTargets)
    {
        if (sortedPriorityTargets == null)
        {
            // No targets provided, set bestTarget to default and return false
            return new TargetData(true);
        }

        // Check pre-selected target first (if any)
        if (!targetHandler.targetOverride.IsEmpty)
        {
            return targetHandler.targetOverride;
        }

        // Loop through sorted targets
        foreach (TargetData currentTarget in sortedPriorityTargets)
        {

            // Check if target is within gimbal limits
            if (CheckIfTargetIsWithinGimbalLimits(currentTarget))
            {
                return currentTarget;
            }
        }

        // No target within gimbal limits, return false
        return new TargetData(true);
    }

    private IEnumerator ControlFiringTiming()
    {
        m_turretWeapon.UpdateFiringState(true);
        yield return new WaitForSeconds(m_turretWeapon.m_minimumTimeBetweenFiring + 0.05f);
        m_turretWeapon.UpdateFiringState(false);
    }

    #endregion


}

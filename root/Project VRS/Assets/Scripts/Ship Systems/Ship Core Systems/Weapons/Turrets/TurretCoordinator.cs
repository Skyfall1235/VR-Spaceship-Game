using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurretRotation))]
[RequireComponent(typeof(TargetingComponent))]
public class TurretCoordinator : MonoBehaviour
{
    [SerializeField]
    private Weapon m_turretWeapon;

    [SerializeField] 
    private SO_TurretData m_turretData;

    [SerializeField]
    private TurretRotation m_turretRotationController;

    [SerializeField]
    private TargetingComponent m_turretTargetingComponent;

    [SerializeField]
    private GameObject m_projectileInstantiationPoint;

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

        if(m_projectileInstantiationPoint == null)
        {
            m_projectileInstantiationPoint = m_turretWeapon.InstantiationPoint;
        }
        //share the necessary data across the sub components
        m_turretTargetingComponent.WeaponData = m_turretWeapon.WeaponData;
        m_turretRotationController.TurretData = m_turretData;
        //check for instantiation point as its crucial for calculations and movement
        if(m_turretWeapon == null)
        {
            Debug.LogError($"TurretCoordinator {this.gameObject.name} missing turret Weapon reference, stoppping functionality");
            this.enabled = false;
            return;
        }
        m_turretTargetingComponent.ProjectileInstantiationPoint = m_projectileInstantiationPoint;
        m_turretRotationController.ProjectileInstatiationPoint = m_projectileInstantiationPoint;
    }

    #endregion

    
}

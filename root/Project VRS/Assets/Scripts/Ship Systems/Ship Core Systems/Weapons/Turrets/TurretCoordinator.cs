using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TurretRotation))]
[RequireComponent(typeof(TargetingComponent))]
public class TurretCoordinator : MonoBehaviour
{
    [SerializeField]
    private SO_WeaponData m_weaponData;

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
        //share the nessicary data across the sub components
        m_turretTargetingComponent.WeaponData = m_weaponData;
        m_turretRotationController.TurretData = m_turretData;
        //check for instantiation point as its crucial for calculations and movement
        if(m_projectileInstantiationPoint == null)
        {
            Debug.LogError($"TurretCoordinator {this.gameObject.name} missing projectile Instantiation point, stoppping functionality");
            this.enabled = false;
            return;
        }
        m_turretTargetingComponent.ProjectileInstantiationPoint = m_projectileInstantiationPoint;
        m_turretRotationController.ProjectileInstatiationPoint = m_projectileInstantiationPoint;
    }

    #endregion


}

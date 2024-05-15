using UnityEngine;

public class TurretRotation : MonoBehaviour
{
    private SO_TurretData m_turretData;
    public SO_TurretData TurretData
    {
        set 
        { 
            m_turretData = value; 
        }
    }

    [Header("Turret Parts")]
    [SerializeField]
    protected GameObject m_xAxisRotator;

    [SerializeField]
    protected GameObject m_yAxisRotator;

    [SerializeField]
    protected GameObject m_barrel;

    public void TurnToLeadPosition(Vector3 targetPosition)
    {
        Vector3 desiredDirection = (targetPosition - m_barrel.transform.position).normalized;

        Vector3 xAxisRotatorEulerAngles = m_xAxisRotator.transform.rotation.eulerAngles;
        Vector3 yAxisRotatorEulerAngles = m_yAxisRotator.transform.rotation.eulerAngles;

        // Clamp desired direction angles within limits
        float desiredXRotation = Mathf.Clamp(Quaternion.LookRotation(desiredDirection).eulerAngles.x, 
                                                                     m_turretData.ConstraintsOfXTurretAngles.x, 
                                                                     m_turretData.ConstraintsOfXTurretAngles.y);
        float desiredYRotation = Mathf.Clamp(Quaternion.LookRotation(desiredDirection).eulerAngles.y, 
                                                                     m_turretData.ConstraintsOfYTurretAngles.x, 
                                                                     m_turretData.ConstraintsOfYTurretAngles.y);

        Quaternion newXAxisRotation = Quaternion.Euler(desiredXRotation, xAxisRotatorEulerAngles.y, xAxisRotatorEulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotatorEulerAngles.x, desiredYRotation, yAxisRotatorEulerAngles.z);

        m_xAxisRotator.transform.rotation = Quaternion.RotateTowards(m_xAxisRotator.transform.rotation, newXAxisRotation, m_turretData.TurretRotationSpeed.x * Time.deltaTime);
        m_yAxisRotator.transform.rotation = Quaternion.RotateTowards(m_yAxisRotator.transform.rotation, newYAxisRotation, m_turretData.TurretRotationSpeed.y * Time.deltaTime);
    }
}

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

        float desiredXRotation = Quaternion.LookRotation(desiredDirection).eulerAngles.x;
        float desiredYRotation = Quaternion.LookRotation(desiredDirection).eulerAngles.y;
        Quaternion newXAxisRotation = Quaternion.Euler(desiredXRotation, xAxisRotatorEulerAngles.y, xAxisRotatorEulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotatorEulerAngles.x, desiredYRotation, yAxisRotatorEulerAngles.z);
        if (CheckInGimbalLimits(targetPosition))
        {
            m_xAxisRotator.transform.rotation = Quaternion.RotateTowards(m_xAxisRotator.transform.rotation, newXAxisRotation, m_turretData.TurretRotationSpeed.x * Time.deltaTime);
            m_yAxisRotator.transform.rotation = Quaternion.RotateTowards(m_yAxisRotator.transform.rotation, newYAxisRotation, m_turretData.TurretRotationSpeed.y * Time.deltaTime);
        }
    }

    public bool CheckInGimbalLimits(Vector3 targetPosition)
    {
        Vector3 desiredDirection = (targetPosition - m_barrel.transform.position).normalized;

        Vector3 xAxisRotatorEulerAngles = m_xAxisRotator.transform.rotation.eulerAngles;
        Vector3 yAxisRotatorEulerAngles = m_yAxisRotator.transform.rotation.eulerAngles;
        float desiredXRotation = Quaternion.LookRotation(desiredDirection).eulerAngles.x;
        float desiredYRotation = Quaternion.LookRotation(desiredDirection).eulerAngles.y;
        Quaternion xForward = Quaternion.Euler(transform.rotation.eulerAngles.x, m_xAxisRotator.transform.rotation.eulerAngles.y, m_xAxisRotator.transform.rotation.eulerAngles.z);
        Quaternion yForward = Quaternion.Euler(m_yAxisRotator.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, m_yAxisRotator.transform.rotation.eulerAngles.z);
        Quaternion newXAxisRotation = Quaternion.Euler(desiredXRotation, xAxisRotatorEulerAngles.y, xAxisRotatorEulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotatorEulerAngles.x, desiredYRotation, yAxisRotatorEulerAngles.z);
        if (Quaternion.Angle(xForward, newXAxisRotation) <= m_turretData.ConstraintsOfXTurretAngles.y && Quaternion.Angle(yForward, newYAxisRotation) <= m_turretData.ConstraintsOfYTurretAngles.y)
        {
            return true;
        }
        return false;
    }
}

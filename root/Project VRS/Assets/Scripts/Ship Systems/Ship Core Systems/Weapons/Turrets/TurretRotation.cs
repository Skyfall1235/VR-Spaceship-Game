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

    private GameObject m_projectileInstatiationPoint;
    public GameObject ProjectileInstatiationPoint
    {
        set
        {
            m_projectileInstatiationPoint = value;
        }
    }

    public void TurnToLeadPosition(Vector3 targetPosition)
    {
        Vector3 currentDirection = m_barrel.transform.up;
        Vector3 desiredDirection = (targetPosition - m_barrel.transform.position).normalized;

        Vector3 xAxisRotatorEulerAngles = m_xAxisRotator.transform.rotation.eulerAngles;
        Vector3 yAxisRotatorEulerAngles = m_yAxisRotator.transform.rotation.eulerAngles;

        Quaternion newXAxisRotation = Quaternion.Euler(Quaternion.LookRotation(desiredDirection).eulerAngles.x, xAxisRotatorEulerAngles.y, xAxisRotatorEulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotatorEulerAngles.x, Quaternion.LookRotation(desiredDirection).eulerAngles.y, yAxisRotatorEulerAngles.z);

        m_xAxisRotator.transform.rotation = Quaternion.RotateTowards(m_xAxisRotator.transform.rotation, newXAxisRotation, m_turretData.TurretRotationSpeed.x * Time.deltaTime);
        m_yAxisRotator.transform.rotation = Quaternion.RotateTowards(m_yAxisRotator.transform.rotation, newYAxisRotation, m_turretData.TurretRotationSpeed.y * Time.deltaTime);
    }
}

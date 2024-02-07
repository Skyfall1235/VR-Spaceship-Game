using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRCS : MonoBehaviour
{
    [Header("Flight Path Viability")]

    [SerializeField]
    private Vector3 m_targetVector;

    [SerializeField]
    private Vector3 m_currentVector;

    [SerializeField]
    [Range(1.0f, 90.0f)]
    private float m_flightPathMaxVectorDeviation = 60.0f;

    [SerializeField]
    [Range(.0f, 90.0f)]
    private float m_flightPathDeadZoneVectorDeviation = 10.0f;

    public float DetermineRCSApplicationValue()
    {

        return 0f;
    }

    public void ApplyRCSToVector(Rigidbody rb, Vector3 targetVector)
    {
        //have a deadzone of acceptable movement vectors
    }

    void OnDrawGizmosSelected()
    {
        var angleStartPoint = transform.position;
        const float k_AngleLength = 6f;

        Gizmos.color = Color.green;
        var axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(m_flightPathMaxVectorDeviation, 0.0f, 0.0f) * Vector3.forward) * k_AngleLength;
        var axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(-m_flightPathMaxVectorDeviation, 0.0f, 0.0f) * Vector3.forward) * k_AngleLength;
        Gizmos.DrawLine(angleStartPoint, axisPoint1);
        Gizmos.DrawLine(angleStartPoint, axisPoint2);

        if (m_flightPathDeadZoneVectorDeviation > 0.0f)
        {
            Gizmos.color = Color.red;
            axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(m_flightPathDeadZoneVectorDeviation, 0.0f, 0.0f) * Vector3.forward) * k_AngleLength;
            axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(-m_flightPathDeadZoneVectorDeviation, 0.0f, 0.0f) * Vector3.forward) * k_AngleLength;
            Gizmos.DrawLine(angleStartPoint, axisPoint1);
            Gizmos.DrawLine(angleStartPoint, axisPoint2);
        }

        Gizmos.color = Color.green;
        var axisPoint3 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, m_flightPathMaxVectorDeviation, 0.0f) * Vector3.forward) * k_AngleLength;
        var axisPoint4 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, -m_flightPathMaxVectorDeviation, 0.0f) * Vector3.forward) * k_AngleLength;
        Gizmos.DrawLine(angleStartPoint, axisPoint3);
        Gizmos.DrawLine(angleStartPoint, axisPoint4);

        if (m_flightPathDeadZoneVectorDeviation > 0.0f)
        {
            Gizmos.color = Color.red;
            axisPoint3 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, m_flightPathDeadZoneVectorDeviation, 0.0f) * Vector3.forward) * k_AngleLength;
            axisPoint4 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, -m_flightPathDeadZoneVectorDeviation, 0.0f) * Vector3.forward) * k_AngleLength;
            Gizmos.DrawLine(angleStartPoint, axisPoint3);
            Gizmos.DrawLine(angleStartPoint, axisPoint4);
        }
    }

    void OnValidate()
    {
        m_flightPathDeadZoneVectorDeviation = Mathf.Min(m_flightPathDeadZoneVectorDeviation, m_flightPathMaxVectorDeviation * 0.9f);
    }
}

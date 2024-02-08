using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ShipRCS : MonoBehaviour
{
    [Header("Flight Path Viability")]

    [SerializeField]
    private readonly Vector3 m_targetVector;//normailzed vector3.forward with the thrust value as the x

    [SerializeField]
    private readonly Vector3 m_currentVector; // just the rigidbodies current velocity

    [SerializeField]
    private float m_breakingForce;

    [SerializeField]
    [Range(1.0f, 90.0f)]
    private float m_flightPathMaxVectorDeviation = 60.0f;

    [SerializeField]
    [Range(.0f, 90.0f)]
    private float m_flightPathDeadZoneVectorDeviation = 10.0f;


    private void Start()
    {

    }

    public float DetermineRCSLinearValue()
    {
        float applicationPercent = 0f;
        //applicationPercent

        float magnitude = m_currentVector.magnitude;

        if (magnitude <= m_flightPathDeadZoneVectorDeviation)
        {
            return 0f; // Return zero if within deadzone
        }
        if (magnitude >= m_flightPathMaxVectorDeviation)
        {
            return 1f; // Return zero if within deadzone
        }

        //we dont need the full spectrum of floats, just go to the hundredths place
        Vector3 averageVector = (m_targetVector + m_currentVector) / 2; //since the target vector can be extrmemely small, even 0, the current vector will do most of the work
        Vector3 normalizedAverageVector = averageVector.normalized; //the direction the craft is currently going, irrespective of its target vector or velocity

        //get the angle from the target velocity to the normalized average vector
        float angleOfVectors = Vector3.Angle(m_targetVector.normalized, normalizedAverageVector);

        applicationPercent = Remap(angleOfVectors, 0, 90, 0, 1);

        return applicationPercent;
    }

    public void ApplyRCSLinearToVector(Rigidbody rb, Vector3 targetVector, Vector3[] applicationLocation)
    {
        //have a deadzone of acceptable movement vectors

        //invert the normalizedAverageVector to find the opposide direction, and apply force to slow down the craft

        //use the application locations to find the best locations to apply the braking force
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
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

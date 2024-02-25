using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torquetest : MonoBehaviour
{
    public Rigidbody rb;

    public float torqueVal = 0f;

    public Vector3 dir = Vector3.zero;

    public enum TorqueType
    {
        relative,
        normal
    }
    public TorqueType type = TorqueType.normal;


    private void Update()
    {
        switch (type)
        {
            case TorqueType.relative:
                rb.AddRelativeTorque(dir, ForceMode.Force);
                break;
            case TorqueType.normal:
                rb.AddTorque(dir, ForceMode.Force);
                break;
        }

        Vector3 cross = FindAxisForRotation(dir, transform);
        //Debug.Log(cross);

    }


    private Vector3 FindAxisForRotation(Vector2 axis, Transform localTransform)
    {
        Vector3 crossAxis = Vector3.Cross(axis, localTransform.up);
        return crossAxis.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        // Calculate the end point of the line in world space
        Vector3 endPoint = transform.position + new Vector3(2.74f, 6.25f, -3.46f) * 5f;
        Vector3 test = FindAxisForRotation(Vector3.forward, transform);
        //Debug.Log(test);// Adjust the scaling factor as needed
        // Draw the line
        Gizmos.DrawLine(transform.position, endPoint);
    }
}

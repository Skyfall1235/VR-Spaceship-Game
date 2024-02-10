using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

[RequireComponent(typeof(ShipInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class ShipMovementController : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The input handler of the entire ship
    /// </summary>
    private ShipInputHandler m_shipInputHandler
    {
        get { return GetComponent<ShipInputHandler>(); }
    }

    /// <summary>
    /// the rigidbody component of the entire ship
    /// </summary>
    private Rigidbody m_shipRigidbody
    {
        get { return GetComponent<Rigidbody>(); }
    }
    [Range(0f, 10f)]
    public float maxAcceleration = 10f;
    [Range(0f, 10f)]
    public float maxDeceleration = 5f;

    public float currentSpeed;

    public float rateOfTurn = 5f;

    [Range(0f, 1000f)]
    public const float maxSpeed = 100f;
    const float lowerJoystickInputBounds = -2f;
    const float UpperJoystickInputBounds = 2f;

    [SerializeField] Logger logger;

    





    #endregion

    //apply new thrsut value
    void ApplyLinearMotionValue(float rawThrottleInput)
    {
        
        float appliedThrustValue = Remap(rawThrottleInput, lowerJoystickInputBounds, maxAcceleration, UpperJoystickInputBounds, maxDeceleration);

    }
    //apply a change in turn
    void ApplyRotationOnAxis(Vector3 axis, float rotationSpeed)
    {

    }

    void ApplyStrafe(float rawYawInput)
    {
        
    }



    //calculate the rotation axis for a turn
    private Vector3 FindAxisForRotation(Vector2 axis, Transform localTransform)
    {
        Vector3 crossAxis = Vector3.Cross(axis, localTransform.up);
        return crossAxis.normalized;
    }

    public static float Remap(float value, float originalMin, float originalMax, float newMin, float newMax)
    {
        // Normalize value to the range [0, 1]
        float normalizedValue = (value - originalMin) / (originalMax - originalMin);

        // Rescale normalized value to the new range
        float rescaledValue = normalizedValue * (newMax - newMin) + newMin;
        return rescaledValue;
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.magenta;

        // Get the input vector (assuming it's a Vector2)
        //Vector2 inputVector;

        // Calculate the desired direction in world space based on transform.forward
        //Vector3 desiredDirection = transform.forward + new Vector3(inputVector.y, inputVector.x, 0f); // Set Z to 0

        // Calculate the end point of the line in world space
        //Vector3 endPoint = transform.position + desiredDirection * 4f; // Adjust the scaling factor as needed

        // Draw the line
        //Gizmos.DrawLine(transform.position, endPoint);
    }

    private void OnValidate()
    {
        if(maxDeceleration + 0.1f >= maxAcceleration)
        {
            maxDeceleration = maxAcceleration - 0.1f;
        }
    }

}



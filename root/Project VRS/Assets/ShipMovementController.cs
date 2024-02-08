using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private Vector3 m_formattedShipInput;

    public float maxAcceleration = 10f;
    public float maxDeceleration = 5f;

    public float currentSpeed;
    public const float maxSpeed = 100f;

    public float rateOfTurn = 5f;

    [SerializeField]
    private Vector3 m_targetVector;//normailzed vector3.forward with the thrust value as the x

    [SerializeField]
    private Vector3 m_currentVector
    {
        get { return GetComponent<Rigidbody>().velocity; }
    }// just the rigidbodies current velocity



    [SerializeField] private ShipRCS m_shipRCS;

    #endregion

    private void ApplyVectoredThrust(ShipInput input)
    {

    }
    //use fixed update to better deal with physics
    private void FixedUpdate()
    {
        //use the input
        Vector2 inputVector = m_shipInputHandler.PlayerInputValue;
        //m_targetVector = transform.forward + new Vector3(inputVector.y, inputVector.x, 0f);
        ApplyThrust(inputVector.x);
        currentSpeed = m_shipRigidbody.velocity.magnitude;
    }


    //assuming balanaced input, the trust should be stable and from the center mass
    private void ApplyThrust(float inputValue)
    {

        m_shipRigidbody.AddForce(transform.forward * inputValue * 5, ForceMode.Acceleration);
    }

    //we will use vector3.cross to find th axis to rotate the craft around, along the crafts PRIAMRY transform
    private void RotateCraft(Vector3 axis1, Vector3 axis2, float rotationSpeed)
    {
        //the axis we want to rotate around
        Vector3 rotationAxis = Vector3.Cross(axis1, axis2);

        float theta = Mathf.Asin(rotationAxis.magnitude);                      // Calculate rotation angle based on axis magnitude
        Vector3 RotationBeforeSpeedValue = rotationAxis.normalized * theta / Time.fixedDeltaTime;

        m_shipRigidbody.AddTorque(rotationAxis, ForceMode.Force);
    }















    #region Stillusable but maybe not anymore
    //create a method that created a normalized vector3 infront of a transform.position based on a vector2 input
    private Vector3 GetForwardInputVector(Vector2 direction2D)
    {
        // 1. Get the object's current forward direction in world space
        Vector3 forward = transform.forward;

        // 2. Calculate the angle of rotation based on the 2D direction
        //    - Use Mathf.Atan2 to get the angle in radians
        //    - Convert it to degrees for clarity
        float angle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;

        // 3. Create a Quaternion representing the rotation needed
        //    - Use Euler angles with 0 for X and Z, and the calculated angle for Y
        //    - This rotates around the object's Y-axis (assuming forward is along Z)
        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        // 4. Rotate the forward vector and normalize it
        //    - Multiply the Quaternion with the forward vector to apply the rotation
        //    - Normalize the result to ensure a unit vector (magnitude of 1)
        Vector3 desiredDirection = rotation * forward;
        return desiredDirection.normalized;
    }

    private void CalculateAndApplyTorqueTowardDesiredRotation()
    {
        // Overall goal: Calculate a torque to apply to a rigidbody, for rotation control.

        // 1. Calculate the desired change in angular velocity:
        Vector3 x = Vector3.Cross(m_currentVector, m_targetVector);  // Cross product to get the axis of rotation
        float theta = Mathf.Asin(x.magnitude);                      // Calculate rotation angle based on axis magnitude
        Vector3 w = x.normalized * theta / Time.fixedDeltaTime;     // Normalize axis and scale by angle and time step

        // 2. Account for object's rotation and inertia:
        Quaternion q = transform.rotation * m_shipRigidbody.inertiaTensorRotation; // Combine rotation and inertia tensor
        Vector3 turn = Vector3.Scale(m_shipRigidbody.inertiaTensor, Quaternion.Inverse(q) * w); // Adjust for inertia

        // 3. Apply the calculated torque to the rigidbody:
        m_shipRigidbody.AddTorque(turn);
    }

    #endregion


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        // Get the input vector (assuming it's a Vector2)
        Vector2 inputVector = m_shipInputHandler.PlayerInputValue;

        // Calculate the desired direction in world space based on transform.forward
        Vector3 desiredDirection = transform.forward + new Vector3(inputVector.y, inputVector.x, 0f); // Set Z to 0

        // Calculate the end point of the line in world space
        Vector3 endPoint = transform.position + desiredDirection * 4f; // Adjust the scaling factor as needed

        // Draw the line
        Gizmos.DrawLine(transform.position, endPoint);
    }

}



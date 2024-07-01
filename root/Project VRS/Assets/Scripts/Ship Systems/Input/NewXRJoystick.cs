using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NewXRJoystick : MonoBehaviour
{
    [Header("Joystick Data")]
    // Assign the hand when it grabs/interacts with this joystick, and unassign on release.
    [SerializeField] Transform hand;
    [SerializeField] float maxRotationAngle = 45f;
    [SerializeField] float returnDuration = 0.25f;  // Duration for the joystick to return to the initial rotation

    private Quaternion initialRotation;
    private Vector2 _joystickVector;  // Backing field for the read-only property
    // Read-only property for joystickVector
    public Vector2 JoystickVector => _joystickVector;

    private float returnTimer = 0f;  // Timer to keep track of the return duration
    private bool isReturning = false;  // Flag to check if we are currently returning to the initial rotation



    //FEATURES TO BE ADDED IN
    public bool ResetAllOnRelease = true;
    public bool ResetXOnRelease = true;
    //[SerializeField] CustomLogger logger;
    //PLEASE USE m_
    //rename joystick vector to the value
    //make methods for firing off changes to the input
    //add in the twisitng readings

    [Serializable]
    public class ValueChangeEvent : UnityEvent<float> { }

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's x value changes")]
    ValueChangeEvent m_OnValueChangeX = new ValueChangeEvent();

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's y value changes")]
    ValueChangeEvent m_OnValueChangeY = new ValueChangeEvent();

    //from 04
    //also you just have to clamp the yaw rotation and use that with the initial yaw to get a float for that value.similar to what i did for the joystick vector
    public float TwistValue;

    #region Monobehavior Messages

    void Start()
    {
        // Store the initial rotation
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (hand != null)
        {
            // Extract the hand’s Y-axis rotation
            Quaternion yRotation = Quaternion.Euler(0, hand.eulerAngles.y, 0);

            // Apply the rotation
            transform.rotation = GetTargetRotation() * yRotation;

            if (isReturning)
            {
                isReturning = false;
                returnTimer = 0f;
            }
        }
        else
        {
            // Start the timer if the hand is not found
            if (!isReturning)
            {
                isReturning = true;
                returnTimer = 0f;
            }
            // Update the timer and calculate the interpolation factor
            returnTimer += Time.deltaTime;
            float t = Mathf.Clamp01(returnTimer / returnDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, t);
        }

        // Update the rotational vector
        _joystickVector = GetJoyStickVector();

        Debug.Log(_joystickVector);
    }

    #endregion

    #region XR hookups

    public void SetHand(SelectEnterEventArgs args)
    {
        hand = args.interactorObject.transform;
    }
    public void RemoveHand(SelectExitEventArgs args)
    {
        hand = null;
    }

    #endregion

    #region Math drivers

    private Quaternion GetTargetRotation()
    {
        // Calculate the direction to hand
        Vector3 direction = hand.position - transform.position;

        // Create a rotation that points the Y-axis towards the target direction
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

        // Calculate the angle between the initial rotation and the target rotation
        float angle = Quaternion.Angle(initialRotation, targetRotation);

        // Clamp the rotation angle if necessary
        if (angle > maxRotationAngle)
        {
            // Interpolate towards the target rotation with clamping
            targetRotation = Quaternion.RotateTowards(initialRotation, targetRotation, maxRotationAngle);
        }

        return targetRotation;
    }

    private Vector2 GetJoyStickVector()
    {
        // Calculate the current rotation relative to the initial rotation
        Quaternion relativeRotation = Quaternion.Inverse(initialRotation) * transform.rotation;

        // Extract the forward vector of the relative rotation
        Vector3 forward = relativeRotation * Vector3.up;

        // Create a Vector2 representing the rotational direction in the X-Z plane
        Vector2 rotationalVector = new Vector2(forward.x, forward.z);

        // Normalize the rotational vector
        return rotationalVector.normalized * Mathf.Clamp01(Quaternion.Angle(initialRotation, transform.rotation) / maxRotationAngle);
    }

    #endregion

    

}

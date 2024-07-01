using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NewXRJoystick : MonoBehaviour
{
    [Header("Joystick Data")]
    // Assign the hand when it grabs/interacts with this joystick, and unassign on release.
    Transform m_hand;
    [SerializeField] float m_maxRotationAngle = 45f;
    [SerializeField] float m_returnDuration = 0.25f;  // Duration for the joystick to return to the initial rotation

    private Quaternion m_initialRotation;
    [SerializeField]  private Vector2 m_joystickVector;  // Backing field for the read-only property
    //from 04
    //also you just have to clamp the yaw rotation and use that with the initial yaw to get a float for that value.similar to what i did for the joystick vector
    public float TwistValue;
    // Read-only property for joystickVector
    public Vector2 JoystickVector => m_joystickVector;

    private float m_returnTimer = 0f;  // Timer to keep track of the return duration
    private bool m_isReturning = false;  // Flag to check if we are currently returning to the initial rotation



    //FEATURES TO BE ADDED IN
    public bool m_resetAllOnRelease = true;
    public bool m_resetXOnRelease = true;
    public float m_deadZoneAngle;
    //[SerializeField] CustomLogger logger;
    //PLEASE USE m_
    //rename joystick vector to the value
    //make methods for firing off changes to the input
    //add in the twisitng readings

    [Serializable]
    public class ValueChangeEvent : UnityEvent<float> { }

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's x value changes")]
    ValueChangeEvent m_onValueChangeX = new ValueChangeEvent();

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's y value changes")]
    ValueChangeEvent m_onValueChangeY = new ValueChangeEvent();

    

    #region Monobehavior Messages

    void Start()
    {
        // Store the initial rotation
        m_initialRotation = transform.rotation;
    }

    void Update()
    {
        if (m_hand != null)
        {
            // Extract the hand’s Y-axis rotation
            Quaternion yRotation = Quaternion.Euler(0, m_hand.eulerAngles.y, 0);

            // Apply the rotation
            transform.rotation = GetTargetRotation() * yRotation;

            if (m_isReturning)
            {
                m_isReturning = false;
                m_returnTimer = 0f;
            }
        }
        else
        {
            // Start the timer if the hand is not found
            if (!m_isReturning)
            {
                m_isReturning = true;
                m_returnTimer = 0f;
            }
            // Update the timer and calculate the interpolation factor
            m_returnTimer += Time.deltaTime;
            float t = Mathf.Clamp01(m_returnTimer / m_returnDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_initialRotation, t);
        }

        // Update the rotational vector
        m_joystickVector = new Vector2();
        if((m_joystickVector.x + m_joystickVector.y) > 0.2f) 
        {
            m_joystickVector = GetJoyStickVector();
        }
        TwistValue = GetjoystickTwistValue();

        Debug.Log(m_joystickVector);
    }

    #endregion

    #region XR hookups

    public void SetHand(SelectEnterEventArgs args)
    {
        m_hand = args.interactorObject.transform;
    }
    public void RemoveHand(SelectExitEventArgs args)
    {
        m_hand = null;
    }

    #endregion

    #region Math drivers

    private Quaternion GetTargetRotation()
    {
        // Calculate the direction to hand
        Vector3 direction = m_hand.position - transform.position;

        // Create a rotation that points the Y-axis towards the target direction
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

        // Calculate the angle between the initial rotation and the target rotation
        float angle = Quaternion.Angle(m_initialRotation, targetRotation);

        // Clamp the rotation angle if necessary
        if (angle > m_maxRotationAngle)
        {
            // Interpolate towards the target rotation with clamping
            targetRotation = Quaternion.RotateTowards(m_initialRotation, targetRotation, m_maxRotationAngle);
        }

        return targetRotation;
    }

    private Vector2 GetJoyStickVector()
    {
        // Calculate the current rotation relative to the initial rotation
        Quaternion relativeRotation = Quaternion.Inverse(m_initialRotation) * transform.rotation;

        // Extract the forward vector of the relative rotation
        Vector3 forward = relativeRotation * Vector3.up;

        // Create a Vector2 representing the rotational direction in the X-Z plane
        Vector2 rotationalVector = new Vector2(forward.x, forward.z);

        // Normalize the rotational vector
        return rotationalVector.normalized * Mathf.Clamp01(Quaternion.Angle(m_initialRotation, transform.rotation) / m_maxRotationAngle);
    }

    private float GetjoystickTwistValue()
    {
        // Calculate the current rotation relative to the initial rotation
        Quaternion relativeRotation = Quaternion.Inverse(m_initialRotation) * transform.rotation;

        // Extract the forward vector of the relative rotation
        Vector3 forward = relativeRotation * Vector3.up;

        // Create a Vector2 representing the rotational direction in the X-Z plane
        float rotationalVector =  Mathf.Clamp(forward.y, -45f, 45f);

        return rotationalVector;
    }

    #endregion

    

}

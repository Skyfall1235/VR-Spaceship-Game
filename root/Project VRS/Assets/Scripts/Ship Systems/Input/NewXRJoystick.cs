using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NewXRJoystick : MonoBehaviour
{
    [Header("Joystick Data")]
    // Assign the hand when it grabs/interacts with this joystick, and unassign on release.
    private Transform m_hand;
    public JoystickOrdinal Ordinal;
    [SerializeField] float m_maxJoystickAngle = 45f;
    [SerializeField] float m_maxYawAngle = 30f;  // Maximum yaw twist angle in degrees
    [SerializeField] float m_returnDuration = 0.25f;  // Duration for the joystick to return to the initial rotation
    [SerializeField]  private Vector2 m_joystickValue;  // Backing field for the read-only property
    private float m_joystickTwistValue;  // Backing field for the read-only property

    #region publics
    // Read-only property for joystickVector
    public Vector2 value => m_joystickValue;
    // Read-only property for twistFloat
    public float TwistValue => m_joystickTwistValue;
    #endregion

    #region Internal execution

    private float m_returnTimer = 0f;  // Timer to keep track of the return duration
    private bool m_isReturning = false;  // Flag to check if we are currently returning to the initial rotation
    private Quaternion m_initialRotation;
    private Quaternion m_initialHandRotation;

    #endregion

    #region Events

    [Serializable]
    public class ValueChangeEvent : UnityEvent<float> { }

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's x value changes")]
    private ValueChangeEvent m_onValueChangeX = new ValueChangeEvent();

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's y value changes")]
    private ValueChangeEvent m_onValueChangeY = new ValueChangeEvent();

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's y value changes")]
    private ValueChangeEvent m_onValueChangeZ = new ValueChangeEvent();

    #endregion



    //FEATURES TO BE ADDED IN
    [SerializeField] private bool m_resetXOnRelease = true;
    [SerializeField] private bool m_resetYOnRelease = true;
    [SerializeField] private float m_deadZoneAngle;
    //[SerializeField] CustomLogger logger;
    //rename joystick vector to the value
    //make methods for firing off changes to the input





    #region Monobehavior Messages

    private void Start()
    {
        // Store the initial rotation
        m_initialRotation = transform.rotation;

        // Store the initial hand rotation
        if (m_hand != null)
        {
            m_initialHandRotation = m_hand.rotation;
        }
    }

    private void Update()
    {
        if (m_hand != null)
        {
            // Update the initial hand rotation if it's the first frame the hand is detected
            if (m_initialHandRotation == Quaternion.identity)
            {
                m_initialHandRotation = m_hand.rotation;
            }

            // Apply the rotation
            transform.rotation = GetTargetRotation() * GetTwistRotation();

            if (m_isReturning)
            {
                m_isReturning = false;
                m_returnTimer = 0f;
            }
        }
        else
        {
            // Reset the initial hand rotation
            m_initialHandRotation = Quaternion.identity;

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
        m_joystickValue = GetJoyStickVector();
        // Update the twistFloat
        m_joystickTwistValue = GetYawTwistFloat();

        print(GetJoyStickVector() + " : " + GetYawTwistFloat());
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

    #region Input Updaters

    private void SetValues()
    {
        m_onValueChangeX.Invoke(m_joystickValue.x);
        m_onValueChangeY.Invoke(m_joystickValue.y);
        m_onValueChangeZ.Invoke(m_joystickTwistValue);
    }

    #endregion

    #region Math drivers

    private Quaternion GetTwistRotation()
    {
        // Get the initial hand yaw angle at the start
        float initialHandYaw = m_initialHandRotation.eulerAngles.y;

        // Calculate the current hand yaw angle
        float currentHandYaw = m_hand.eulerAngles.y;

        // Calculate the deltaYaw between the hand’s current yaw and the initial hand yaw
        float deltaYaw = Mathf.DeltaAngle(initialHandYaw, currentHandYaw);

        // Clamp the deltaYaw to be within -maxYawAngle and maxYawAngle
        float clampedYaw = Mathf.Clamp(deltaYaw, -m_maxYawAngle, m_maxYawAngle);

        // Return the clamped twist rotation
        return Quaternion.Euler(0, clampedYaw, 0);
    }

    private float GetYawTwistFloat()
    {
        // Calculate the current yaw angle relative to the initial rotation
        float currentYaw = Mathf.DeltaAngle(m_initialRotation.eulerAngles.y, transform.eulerAngles.y);

        // Normalize the yaw twist value to be between -1 and 1
        float normalizedYawTwist = Mathf.Clamp(currentYaw / m_maxYawAngle, -1f, 1f);
        Debug.DrawLine(transform.position, normalizedYawTwist * transform.forward);  

        return normalizedYawTwist;
    }

    private Quaternion GetTargetRotation()
    {
        // Calculate the direction to hand
        Vector3 direction = m_hand.position - transform.position;

        // Create a rotation that points the Y-axis towards the target direction
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);

        // Calculate the angle between the initial rotation and the target rotation
        float angle = Quaternion.Angle(m_initialRotation, targetRotation);

        // Clamp the rotation angle if necessary
        if (angle > m_maxJoystickAngle)
        {
            // Interpolate towards the target rotation with clamping
            targetRotation = Quaternion.RotateTowards(m_initialRotation, targetRotation, m_maxJoystickAngle);
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
        return rotationalVector.normalized * Mathf.Clamp01(Quaternion.Angle(m_initialRotation, transform.rotation) / m_maxJoystickAngle);
    }

    #endregion

    public enum JoystickOrdinal
    {
        Primary, Secondary
    }

}

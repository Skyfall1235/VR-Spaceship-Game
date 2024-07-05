using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NewXRJoystick : XRBaseInteractable
{
    [Header("Joystick Data")]
    // Assign the m_hand when it grabs/interacts with this joystick, and unassign on release.
    private Transform m_hand;
    public Transform m_handle;
    public JoystickOrdinal Ordinal;
    [SerializeField] float m_maxJoystickAngle = 45f;
    [SerializeField] float m_maxYawAngle = 30f;  // Maximum yaw twist angle in degrees
    [SerializeField] float m_returnDuration = 0.25f;  // Duration for the joystick to return to the initial rotation
    [SerializeField] private Vector2 m_joystickValue;  // Backing field for the read-only property
    [SerializeField] private float m_joystickTwistValue;  // Backing field for the read-only property

    //FEATURES TO BE ADDED IN
    [SerializeField] private bool m_resetXOnRelease = true;
    [SerializeField] private bool m_resetYOnRelease = true;
    [SerializeField] private float m_deadZoneAngle;
    [SerializeField] CustomLogger logger;

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

    const float k_MaxDeadZonePercent = 0.9f;

    #endregion

    #region Events

    [Serializable]
    public class XRJoystickEvents
    {
        [Serializable]
        public class ValueChangeEvent : UnityEvent<float> { }

        [SerializeField]
        [Tooltip("Events to trigger when the joystick's x value changes")]
        public ValueChangeEvent m_onValueChangeX = new ValueChangeEvent();

        [SerializeField]
        [Tooltip("Events to trigger when the joystick's y value changes")]
        public ValueChangeEvent m_onValueChangeY = new ValueChangeEvent();

        [SerializeField]
        [Tooltip("Events to trigger when the joystick's y value changes")]
        public ValueChangeEvent m_onValueChangeZ = new ValueChangeEvent();
    }
    [SerializeField] public XRJoystickEvents m_joystickEvents;

    #endregion

    #region Monobehavior Messages

    void Start()
    {
        // Store the initial rotation
        m_initialRotation = m_handle.localRotation;

        // Store the initial m_hand rotation relative to the parent
        if (m_hand != null)
        {
            m_initialHandRotation = Quaternion.Inverse(transform.rotation) * m_hand.rotation;
        }
    }

    void Update()
    {
        if (m_hand != null)
        {
            // Set the initial m_hand rotation only once when the m_hand first interacts
            if (m_initialHandRotation == Quaternion.identity)
            {
                m_initialHandRotation = Quaternion.Inverse(transform.rotation) * m_hand.rotation;
            }

            // Apply the rotation
            m_handle.localRotation = m_initialRotation * GetTargetRotation() * GetTwistRotation();

            if (m_isReturning)
            {
                m_isReturning = false;
                m_returnTimer = 0f;
            }
        }
        else
        {
            // Reset the initial m_hand rotation
            m_initialHandRotation = Quaternion.identity;

            // Start the timer if the m_hand is not found
            if (!m_isReturning)
            {
                m_isReturning = true;
                m_returnTimer = 0f;
            }

            // Update the timer and calculate the interpolation factor
            m_returnTimer += Time.deltaTime;
            float t = Mathf.Clamp01(m_returnTimer / m_returnDuration);
            m_handle.localRotation = Quaternion.Slerp(transform.localRotation, m_initialRotation, t);
        }

        // Update the rotational vector
        m_joystickValue = GetJoyStickVector();
        // Update the twistFloat
        m_joystickTwistValue = GetYawTwistFloat();

        //print(GetJoyStickVector() + " : " + GetYawTwistFloat());
    }

    #endregion

    #region XR hookups

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(SetHand);
        selectExited.AddListener(RemoveHand);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        selectEntered.AddListener(SetHand);
        selectExited.AddListener(RemoveHand);
    }

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
        m_joystickEvents.m_onValueChangeX.Invoke(m_joystickValue.x);
        m_joystickEvents.m_onValueChangeY.Invoke(m_joystickValue.y);
        m_joystickEvents.m_onValueChangeZ.Invoke(m_joystickTwistValue);
    }

    #endregion

    #region Math drivers

    private Quaternion GetTwistRotation()
    {
        // Get the initial m_hand yaw relative to the joystick's parent
        float initialHandYaw = m_initialHandRotation.eulerAngles.y;

        // Calculate the current m_hand yaw relative to the joystick's parent
        Quaternion handLocalRotation = Quaternion.Inverse(transform.rotation) * m_hand.rotation;
        float currentHandYaw = handLocalRotation.eulerAngles.y;

        // Calculate the deltaYaw between the m_hand�s current yaw and the initial m_hand yaw
        float deltaYaw = Mathf.DeltaAngle(initialHandYaw, currentHandYaw);

        // Clamp the deltaYaw to be within -m_maxYawAngle and m_maxYawAngle
        float clampedYaw = Mathf.Clamp(deltaYaw, -m_maxYawAngle, m_maxYawAngle);

        // Return the clamped twist rotation
        return Quaternion.Euler(0, clampedYaw, 0);
    }


    private float GetYawTwistFloat()
    {
        // Calculate the current yaw angle relative to the initial rotation in local space
        float currentYaw = Mathf.DeltaAngle(m_initialRotation.eulerAngles.y, transform.localRotation.eulerAngles.y);

        // Normalize the yaw twist value to be between -1 and 1
        float normalizedYawTwist = Mathf.Clamp(currentYaw / m_maxYawAngle, -1f, 1f);

        return normalizedYawTwist;
    }

    private Quaternion GetTargetRotation()
    {
        // Calculate the direction to m_hand in local space
        Vector3 handLocalPosition = transform.InverseTransformPoint(m_hand.position);
        Vector3 direction = handLocalPosition - m_handle.localPosition;

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
        Quaternion relativeRotation = Quaternion.Inverse(m_initialRotation) * m_handle.localRotation;

        // Extract the forward vector of the relative rotation
        Vector3 forward = relativeRotation * Vector3.up;

        // Create a Vector2 representing the rotational direction in the X-Z plane
        Vector2 rotationalVector = new Vector2(forward.x, forward.z);

        // Normalize the rotational vector
        return rotationalVector.normalized * Mathf.Clamp01(Quaternion.Angle(m_initialRotation, m_handle.localRotation) / m_maxJoystickAngle);
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        //get the base of the line
        var angleStartPoint = transform.position;

        //save our known angle length
        const float k_AngleLength = 0.25f;

        //Null check before proceeding
        if (m_handle != null)
        {
            angleStartPoint = m_handle.position;
        }

        //green is the outer bounds of the joystick
        Gizmos.color = Color.green;
        DrawLines(new Vector3(m_maxJoystickAngle, 0.0f, 0.0f));

        //red is the deadzone angle, where inputs arent updated
        if (m_deadZoneAngle > 0.0f)
        {
            Gizmos.color = Color.red;
            DrawLines(new Vector3(m_deadZoneAngle, 0.0f, 0.0f));
        }

        //green is the outer bounds of the joystick
        Gizmos.color = Color.green;
        DrawLines(new Vector3(0.0f, 0.0f, m_maxJoystickAngle));

        //red is the deadzone angle, where inputs arent updated
        if (m_deadZoneAngle > 0.0f)
        {
            Gizmos.color = Color.red;
            DrawLines(new Vector3(0.0f, 0.0f, m_deadZoneAngle));
        }

        //method in method as a throwaway for complex logic that doesnt belong out of callback
        void DrawLines(Vector3 unit)
        {
            //create the axis points
            var axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(unit) * Vector3.up) * k_AngleLength;
            var axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(-unit) * Vector3.up) * k_AngleLength;
            //draw the lines with whatever color the gizmo is currently
            Gizmos.DrawLine(angleStartPoint, axisPoint1);
            Gizmos.DrawLine(angleStartPoint, axisPoint2);
        }
    }

    void OnValidate()
    {
        m_deadZoneAngle = Mathf.Min(m_deadZoneAngle, m_maxJoystickAngle * k_MaxDeadZonePercent);
    }

    /// <summary>
    /// This is used to tell the ship controller what type of joystick this is
    /// </summary>
    public enum JoystickOrdinal
    {
        Primary, Secondary, tertiary
    }

}

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class NewXRJoystick : XRBaseInteractable
{
    // Assign the m_hand when it grabs/interacts with this joystick, and unassign on release.
    private Transform m_hand;
    [Header("Joystick Data")]
    [SerializeField] private Transform m_handle;
    [SerializeField] private JoystickOrdinal m_ordinal; //allows specification of a left or right handed joystick, or center dpeending on needed usage
    [SerializeField] private JoystickResetType m_resetType; //the type of resetting rotations permitted for the joystick
    public JoystickOrdinal Ordinal => m_ordinal;

    [Header("Joystick Values")]
    [SerializeField] private Vector2 m_joystickValue;  // Backing field for the read-only property
    [SerializeField] private float m_joystickTwistValue;  // Backing field for the read-only property

    [Header("Joystick Movement Control")]
    [SerializeField] private float m_maxJoystickAngle = 45f;
    [SerializeField] private float m_maxYawAngle = 30f;  // Maximum yaw twist angle in degrees
    [SerializeField] private float m_returnDuration = 0.25f;  // Duration for the joystick to return to the initial rotation
    

    //FEATURES TO BE ADDED IN
    [Header("Deadzone Data")]
    [Range(0f, 0.9f)]
    [SerializeField] private float m_deadZoneJoystickValue;
    [SerializeField] private float m_deadZoneYawAngle;
    [SerializeField] private CustomLogger m_logger;


    public void SetStickDeadzone(float val)
    {
        m_deadZoneJoystickValue = val;
    }
    public void SetTwistDeadzone(float val) 
    { 
        m_deadZoneYawAngle = val;
    }


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

    const float MaxDeadZonePercent = 0.9f;

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
            // Reset the initial hand rotation
            m_initialHandRotation = Quaternion.identity;

            // Start the timer if the hand is not found
            if (!m_isReturning)
            {
                m_isReturning = true;
                m_returnTimer = 0f;
            }

            // Update the timer and calculate the interpolation factor
            //dont bother keeping track of time past 5 seconds
            if(m_returnTimer < 5f)
            {
                m_returnTimer += Time.deltaTime;
            }
            float t = Mathf.Clamp01(m_returnTimer / m_returnDuration);

            // Slerp the rotation back to the initial rotation
            Quaternion slerpedRotation = Quaternion.Slerp(m_handle.localRotation, m_initialRotation, t);

            // Extract the X component from the current rotation
            float currentX = m_handle.localRotation.eulerAngles.x;

            // Extract the Z component from the current rotation
            float currentZ = m_handle.localRotation.eulerAngles.z;

            //based on the joystick reset type, we can selectively set the slerp rotations to be normal rotation or to keep its current orientation.
            //this sets the angles to the SLERPED angles, overwriting the set angles.
            switch (m_resetType)
            {
                case JoystickResetType.None:
                    currentX = slerpedRotation.eulerAngles.x;
                    currentZ = slerpedRotation.eulerAngles.z;
                    break;
                case JoystickResetType.X:
                    currentX = slerpedRotation.eulerAngles.x; //overwrite only the X value to be the slerped angle

                    break;
                case JoystickResetType.Y:
                    currentZ = slerpedRotation.eulerAngles.z; //overwrite only the y value to be the slerped angle
                    break;
                case JoystickResetType.XY:
                    //none, because we want the angles to stay where they are
                    break;
            }          

            // Combine the slerped X and Y with the preserved Z component
            Quaternion combinedRotation = Quaternion.Euler(currentX, slerpedRotation.eulerAngles.y, currentZ);

            //set the nadle to be the rotation required
            m_handle.transform.localRotation = combinedRotation;
        }

        //get the values from the joystick
        Vector2 joystickVector = GetJoyStickVector();
        float joystickTwist = GetYawTwistFloat();

        m_joystickTwistValue = MathF.Abs(joystickTwist) > (m_deadZoneYawAngle / 180) ? joystickTwist : 0.0f;
        //set deadzone by allowing the value to be updated if its value would exceed the deadzone threshhold
        m_joystickValue = joystickVector.AbsOfVector2AsFloat() > (m_deadZoneJoystickValue * 2) ? joystickVector : Vector2.zero;
        

        //set the values
        SetValues();
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

        // Calculate the deltaYaw between the m_hand’s current yaw and the initial m_hand yaw
        float deltaYaw = Mathf.DeltaAngle(initialHandYaw, currentHandYaw);

        // Clamp the deltaYaw to be within -m_maxYawAngle and m_maxYawAngle
        float clampedYaw = Mathf.Clamp(deltaYaw, -m_maxYawAngle, m_maxYawAngle);

        // Return the clamped twist rotation
        return Quaternion.Euler(0, clampedYaw, 0);
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

    private float GetYawTwistFloat()
    {
        // Calculate the current yaw angle relative to the initial rotation in local space
        float currentYaw = Mathf.DeltaAngle(m_initialRotation.eulerAngles.y, m_handle.localRotation.eulerAngles.y);

        // Normalize the yaw twist value to be between -1 and 1
        float normalizedYawTwist = Mathf.Clamp(currentYaw / m_maxYawAngle, -1f, 1f);

        return normalizedYawTwist;
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

    private void SlerpTransform(float t, Vector3 LockoutDirection)
    {
        // Extract the Z component from the current rotation
        float currentZ = transform.localRotation.eulerAngles.z;

        // Slerp the rotation back to the initial rotation
        Quaternion slerpedRotation = Quaternion.Slerp(transform.localRotation, m_initialRotation, t);

        // Combine the slerped X and Y with the preserved Z component
        Quaternion combinedRotation = Quaternion.Euler(slerpedRotation.eulerAngles.x, slerpedRotation.eulerAngles.y, currentZ);

        transform.localRotation = combinedRotation;
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        //get the base of the line
        Vector3 angleStartPoint = transform.position;

        //save our known angle length
        const float AngleLength = 0.25f;

        //green is the outer bounds of the joystick
        Gizmos.color = Color.green;
        DrawLines(new Vector3(m_maxJoystickAngle, 0.0f, 0.0f));
        DrawLines(new Vector3(0.0f, 0.0f, m_maxJoystickAngle));


        //red is the deadzone angle, where inputs arent updated
        if (m_deadZoneJoystickValue > 0.0f)
        {
            //NOT FINISHED
            Gizmos.color = Color.red;
            //DrawLines(new Vector3(0.0f, 0.0f, m_deadZoneJoystickValue));
            //DrawLines(new Vector3(m_deadZoneJoystickValue, 0.0f, 0.0f));
        }
        if(m_deadZoneYawAngle > 0.0f)
        {
            Gizmos.color = Color.magenta;
            Vector3 axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(new Vector3(0f, m_deadZoneYawAngle, 0f)) * Vector3.forward) * AngleLength;
            Vector3 axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(new Vector3(0f, -m_deadZoneYawAngle, 0f)) * Vector3.forward) * AngleLength;
            Gizmos.DrawLine(angleStartPoint, axisPoint1);
            Gizmos.DrawLine(angleStartPoint, axisPoint2);
        }

        //method in method as a throwaway for complex logic that doesnt belong out of callback
        void DrawLines(Vector3 unit)
        {
            //create the axis points
            Vector3 axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(unit) * Vector3.up) * AngleLength;
            Vector3 axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(-unit) * Vector3.up) * AngleLength;
            //draw the lines with whatever color the gizmo is currently
            Gizmos.DrawLine(angleStartPoint, axisPoint1);
            Gizmos.DrawLine(angleStartPoint, axisPoint2);
        }
    }

    void OnValidate()
    {
        m_deadZoneJoystickValue = Mathf.Min(m_deadZoneJoystickValue, m_maxJoystickAngle * MaxDeadZonePercent);
        //m_deadZoneYawAngle = Mathf.Min(m_maxYawAngle, m_maxYawAngle * MaxDeadZonePercent);
    }

    /// <summary>
    /// Represents the Joystick in relation to operation selection
    /// </summary>
    [Serializable]
    public enum JoystickOrdinal
    {
        /// <summary>
        /// The primary joystick.
        /// </summary>
        Primary,
        /// <summary>
        /// The secondary joystick.
        /// </summary>
        Secondary,
        /// <summary>
        /// The tertiary joystick (if applicable).
        /// </summary>
        Tertiary
    }

    /// <summary>
    /// Defines the type of reset to be performed on a joystick axis.
    /// </summary>
    [Serializable]
    private enum JoystickResetType
    {
        /// <summary>
        /// No reset is performed.
        /// </summary>
        None,
        /// <summary>
        /// Resets the X axis only.
        /// </summary>
        X,
        /// <summary>
        /// Resets the Y axis only.
        /// </summary>
        Y,
        /// <summary>
        /// Resets both the X and Y axes.
        /// </summary>
        XY
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class JoystickWithYaw : XRBaseInteractable
{
    #region Joystick
    [Header("Joystick")]
    
    const float k_MaxDeadZonePercent = 0.9f;

    public enum JoystickType
    {
        BothCircle,
        BothSquare,
        FrontBack,
        LeftRight,
    }

    [Serializable]
    public class ValueChangeEvent : UnityEvent<float> { }

    [Tooltip("Controls how the joystick moves")]
    [SerializeField]
    JoystickType m_JoystickMotion = JoystickType.BothCircle;

    [SerializeField]
    [Tooltip("The object that is visually grabbed and manipulated")]
    Transform m_Handle = null;

    [SerializeField]
    [Tooltip("The value of the joystick")]
    Vector2 m_Value = Vector2.zero;

    [SerializeField]
    [Tooltip("If true, the joystick will return to center on release")]
    bool m_RecenterOnRelease = true;

    [SerializeField]
    [Tooltip("Maximum angle the joystick can move")]
    [Range(1.0f, 90.0f)]
    float m_MaxAngle = 60.0f;

    [SerializeField]
    [Tooltip("Minimum amount the joystick must move off the center to register changes")]
    [Range(1.0f, 90.0f)]
    float m_DeadZoneAngle = 10.0f;

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's x value changes")]
    ValueChangeEvent m_OnValueChangeX = new ValueChangeEvent();

    [SerializeField]
    [Tooltip("Events to trigger when the joystick's y value changes")]
    ValueChangeEvent m_OnValueChangeY = new ValueChangeEvent();

    IXRSelectInteractor m_Interactor;

    /// <summary>
    /// Controls how the joystick moves
    /// </summary>
    public JoystickType joystickMotion
    {
        get => m_JoystickMotion;
        set => m_JoystickMotion = value;
    }

    /// <summary>
    /// The object that is visually grabbed and manipulated
    /// </summary>
    public Transform handle
    {
        get => m_Handle;
        set => m_Handle = value;
    }

    /// <summary>
    /// The value of the joystick
    /// </summary>
    public Vector2 JoystickValue
    {
        get => m_Value;
        set
        {
            if (!m_RecenterOnRelease)
            {
                SetJoystickValue(value);
                SetHandleAngle(value * m_MaxAngle);
            }
        }
    }

    /// <summary>
    /// If true, the joystick will return to center on release
    /// </summary>
    public bool recenterOnRelease
    {
        get => m_RecenterOnRelease;
        set => m_RecenterOnRelease = value;
    }

    /// <summary>
    /// Maximum angle the joystick can move
    /// </summary>
    public float maxAngle
    {
        get => m_MaxAngle;
        set => m_MaxAngle = value;
    }

    /// <summary>
    /// Minimum amount the joystick must move off the center to register changes
    /// </summary>
    public float deadZoneAngle
    {
        get => m_DeadZoneAngle;
        set => m_DeadZoneAngle = value;
    }

    /// <summary>
    /// Events to trigger when the joystick's x value changes
    /// </summary>
    public ValueChangeEvent onValueChangeX => m_OnValueChangeX;

    /// <summary>
    /// Events to trigger when the joystick's y value changes
    /// </summary>
    public ValueChangeEvent onValueChangeY => m_OnValueChangeY;
    #endregion

    #region Knob
    [Header("knob Section")]
    const float k_ModeSwitchDeadZone = 0.1f; // Prevents rapid switching between the different rotation tracking modes

    /// <summary>
    /// Helper class used to track rotations that can go beyond 180 degrees while minimizing accumulation error
    /// </summary>
    struct TrackedRotation
    {
        /// <summary>
        /// The anchor rotation we calculate an offset from
        /// </summary>
        float m_BaseAngle;

        /// <summary>
        /// The target rotate we calculate the offset to
        /// </summary>
        float m_CurrentOffset;

        /// <summary>
        /// Any previous offsets we've added in
        /// </summary>
        float m_AccumulatedAngle;

        /// <summary>
        /// The total rotation that occurred from when this rotation started being tracked
        /// </summary>
        public float totalOffset => m_AccumulatedAngle + m_CurrentOffset;

        /// <summary>
        /// Resets the tracked rotation so that total offset returns 0
        /// </summary>
        public void Reset()
        {
            m_BaseAngle = 0.0f;
            m_CurrentOffset = 0.0f;
            m_AccumulatedAngle = 0.0f;
        }

        /// <summary>
        /// Sets a new anchor rotation while maintaining any previously accumulated offset
        /// </summary>
        /// <param name="direction">The XZ vector used to calculate a rotation angle</param>
        public void SetBaseFromVector(Vector3 direction)
        {
            // Update any accumulated angle
            m_AccumulatedAngle += m_CurrentOffset;

            // Now set a new base angle
            m_BaseAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            m_CurrentOffset = 0.0f;
        }

        public void SetTargetFromVector(Vector3 direction)
        {
            // Set the target angle
            var targetAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            // Return the offset
            m_CurrentOffset = ShortestAngleDistance(m_BaseAngle, targetAngle, 360.0f);

            // If the offset is greater than 90 degrees, we update the base so we can rotate beyond 180 degrees
            if (Mathf.Abs(m_CurrentOffset) > 90.0f)
            {
                m_BaseAngle = targetAngle;
                m_AccumulatedAngle += m_CurrentOffset;
                m_CurrentOffset = 0.0f;
            }
        }
    }

    [SerializeField]
    [Tooltip("The value of the knob")]
    [Range(0.0f, 1.0f)]
    float m_KnobValue = 0.5f;

    [SerializeField]
    [Tooltip("Whether this knob's rotation should be clamped by the angle limits")]
    bool m_KnobClampedMotion = true;

    [SerializeField]
    [Tooltip("Rotation of the knob at value '1'")]
    float m_KnobMaxAngle = 90.0f;

    [SerializeField]
    [Tooltip("Rotation of the knob at value '0'")]
    float m_KnobMinAngle = -90.0f;

    [SerializeField]
    [Tooltip("Angle increments to support, if greater than '0'")]
    float m_KnobAngleIncrement = 0.0f;

    [SerializeField]
    [Tooltip("The position of the interactor controls rotation when outside this radius")]
    float m_KnobPositionTrackedRadius = 0.1f;

    [SerializeField]
    [Tooltip("How much controller rotation ")]
    float m_KnobTwistSensitivity = 1.5f;

    [SerializeField]
    [Tooltip("Events to trigger when the knob is rotated")]
    ValueChangeEvent m_OnValueChange = new ValueChangeEvent();

    bool m_PositionDriven = false;
    bool m_UpVectorDriven = false;

    TrackedRotation m_PositionAngles = new TrackedRotation();
    TrackedRotation m_UpVectorAngles = new TrackedRotation();
    TrackedRotation m_ForwardVectorAngles = new TrackedRotation();

    float m_BaseKnobRotation = 0.0f;


    /// <summary>
    /// The value of the knob
    /// </summary>
    public float Knobvalue
    {
        get => m_KnobValue;
        set
        {
            SetKnobValue(value);
            SetKnobRotation(ValueToRotation());
        }
    }

    /// <summary>
    /// Whether this knob's rotation should be clamped by the angle limits
    /// </summary>
    public bool KnobclampedMotion
    {
        get => m_KnobClampedMotion;
        set => m_KnobClampedMotion = value;
    }

    /// <summary>
    /// Rotation of the knob at value '1'
    /// </summary>
    public float KnobmaxAngle
    {
        get => m_MaxAngle;
        set => m_MaxAngle = value;
    }

    /// <summary>
    /// Rotation of the knob at value '0'
    /// </summary>
    public float KnobminAngle
    {
        get => m_KnobMinAngle;
        set => m_KnobMinAngle = value;
    }

    /// <summary>
    /// The position of the interactor controls rotation when outside this radius
    /// </summary>
    public float KnobpositionTrackedRadius
    {
        get => m_KnobPositionTrackedRadius;
        set => m_KnobPositionTrackedRadius = value;
    }

    /// <summary>
    /// Events to trigger when the knob is rotated
    /// </summary>
    public ValueChangeEvent onValueChange => m_OnValueChange;
    #endregion

    void Start()
    {
        //joystick components
        if (m_RecenterOnRelease)
        {
            SetHandleAngle(Vector2.zero);
        }
        SetJoystickValue(m_Value);

        //knob components
        SetKnobValue(m_KnobValue);
        SetKnobRotation(ValueToRotation());
    }

    #region XR specific methods

    protected override void OnEnable()
    {
        base.OnEnable();
        selectEntered.AddListener(StartGrab);
        selectExited.AddListener(EndGrab);
    }

    protected override void OnDisable()
    {
        selectEntered.RemoveListener(StartGrab);
        selectExited.RemoveListener(EndGrab);
        base.OnDisable();
    }

    private void StartGrab(SelectEnterEventArgs args)
    {
        m_Interactor = args.interactorObject;

        m_PositionAngles.Reset();
        m_UpVectorAngles.Reset();
        m_ForwardVectorAngles.Reset();

        UpdateBaseKnobRotation();
        UpdateRotation(true);
    }

    private void EndGrab(SelectExitEventArgs arts)
    {
        UpdateValue();

        if (m_RecenterOnRelease)
        {
            SetHandleAngle(Vector2.zero);
            SetJoystickValue(Vector2.zero);
        }

        m_Interactor = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
            {
                UpdateValue();
                UpdateRotation();
            }
        }
    }

    #endregion

    #region JoyStick Methods
    Vector3 GetLookDirection()
    {
        Vector3 direction = m_Interactor.GetAttachTransform(this).position - m_Handle.position;
        direction = transform.InverseTransformDirection(direction);
        switch (m_JoystickMotion)
        {
            case JoystickType.FrontBack:
                direction.x = 0;
                break;
            case JoystickType.LeftRight:
                direction.z = 0;
                break;
        }

        direction.y = Mathf.Clamp(direction.y, 0.01f, 1.0f);
        return direction.normalized;
    }

    void UpdateValue()
    {
        var lookDirection = GetLookDirection();

        // Get up/down angle and left/right angle
        var upDownAngle = Mathf.Atan2(lookDirection.z, lookDirection.y) * Mathf.Rad2Deg;
        var leftRightAngle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;

        // Extract signs
        var signX = Mathf.Sign(leftRightAngle);
        var signY = Mathf.Sign(upDownAngle);

        upDownAngle = Mathf.Abs(upDownAngle);
        leftRightAngle = Mathf.Abs(leftRightAngle);

        var stickValue = new Vector2(leftRightAngle, upDownAngle) * (1.0f / m_MaxAngle);

        // Clamp the stick value between 0 and 1 when doing everything but circular stick motion
        if (m_JoystickMotion != JoystickType.BothCircle)
        {
            stickValue.x = Mathf.Clamp01(stickValue.x);
            stickValue.y = Mathf.Clamp01(stickValue.y);
        }
        else
        {
            // With circular motion, if the stick value is greater than 1, we normalize
            // This way, an extremely strong value in one direction will influence the overall stick direction
            if (stickValue.magnitude > 1.0f)
            {
                stickValue.Normalize();
            }
        }

        // Rebuild the angle values for visuals
        leftRightAngle = stickValue.x * signX * m_MaxAngle;
        upDownAngle = stickValue.y * signY * m_MaxAngle;

        // Apply deadzone and sign back to the logical stick value
        var deadZone = m_DeadZoneAngle / m_MaxAngle;
        var aliveZone = (1.0f - deadZone);
        stickValue.x = Mathf.Clamp01((stickValue.x - deadZone)) / aliveZone;
        stickValue.y = Mathf.Clamp01((stickValue.y - deadZone)) / aliveZone;

        // Re-apply signs
        stickValue.x *= signX;
        stickValue.y *= signY;

        SetHandleAngle(new Vector2(leftRightAngle, upDownAngle));
        SetJoystickValue(stickValue);
    }

    void SetJoystickValue(Vector2 value)
    {
        m_Value = value;
        m_OnValueChangeX.Invoke(m_Value.x);
        m_OnValueChangeY.Invoke(m_Value.y);
    }

    void SetHandleAngle(Vector2 angles)
    {
        if (m_Handle == null)
            return;

        var xComp = Mathf.Tan(angles.x * Mathf.Deg2Rad);
        var zComp = Mathf.Tan(angles.y * Mathf.Deg2Rad);
        var largerComp = Mathf.Max(Mathf.Abs(xComp), Mathf.Abs(zComp));
        var yComp = Mathf.Sqrt(1.0f - largerComp * largerComp);

        m_Handle.up = (transform.up * yComp) + (transform.right * xComp) + (transform.forward * zComp); //THIS MIGHT B THE PROBLEM ABOUT THE POSITION NOT UDATING PROPERLY
    }
    #endregion

    #region Knob Methods
    void UpdateRotation(bool freshCheck = false)
    {
        // Are we in position offset or direction rotation mode?
        var interactorTransform = m_Interactor.GetAttachTransform(this);

        // We cache the three potential sources of rotation - the position offset, the forward vector of the controller, and up vector of the controller
        // We store any data used for determining which rotation to use, then flatten the vectors to the local xz plane
        var localOffset = transform.InverseTransformVector(interactorTransform.position - m_Handle.position);
        localOffset.y = 0.0f;
        var radiusOffset = transform.TransformVector(localOffset).magnitude;
        localOffset.Normalize();

        var localForward = transform.InverseTransformDirection(interactorTransform.forward);
        var localY = Math.Abs(localForward.y);
        localForward.y = 0.0f;
        localForward.Normalize();

        var localUp = transform.InverseTransformDirection(interactorTransform.up);
        localUp.y = 0.0f;
        localUp.Normalize();


        if (m_PositionDriven && !freshCheck)
            radiusOffset *= (1.0f + k_ModeSwitchDeadZone);

        // Determine when a certain source of rotation won't contribute - in that case we bake in the offset it has applied
        // and set a new anchor when they can contribute again
        if (radiusOffset >= m_KnobPositionTrackedRadius)
        {
            if (!m_PositionDriven || freshCheck)
            {
                m_PositionAngles.SetBaseFromVector(localOffset);
                m_PositionDriven = true;
            }
        }
        else
            m_PositionDriven = false;

        // If it's not a fresh check, then we weight the local Y up or down to keep it from flickering back and forth at boundaries
        if (!freshCheck)
        {
            if (!m_UpVectorDriven)
                localY *= (1.0f - (k_ModeSwitchDeadZone * 0.5f));
            else
                localY *= (1.0f + (k_ModeSwitchDeadZone * 0.5f));
        }

        if (localY > 0.707f)
        {
            if (!m_UpVectorDriven || freshCheck)
            {
                m_UpVectorAngles.SetBaseFromVector(localUp);
                m_UpVectorDriven = true;
            }
        }
        else
        {
            if (m_UpVectorDriven || freshCheck)
            {
                m_ForwardVectorAngles.SetBaseFromVector(localForward);
                m_UpVectorDriven = false;
            }
        }

        // Get angle from position
        if (m_PositionDriven)
            m_PositionAngles.SetTargetFromVector(localOffset);

        if (m_UpVectorDriven)
            m_UpVectorAngles.SetTargetFromVector(localUp);
        else
            m_ForwardVectorAngles.SetTargetFromVector(localForward);

        // Apply offset to base knob rotation to get new knob rotation
        var knobRotation = m_BaseKnobRotation - ((m_UpVectorAngles.totalOffset + m_ForwardVectorAngles.totalOffset) * m_KnobTwistSensitivity) - m_PositionAngles.totalOffset;

        // Clamp to range
        if (m_KnobClampedMotion)
            knobRotation = Mathf.Clamp(knobRotation, m_KnobMinAngle, m_MaxAngle);

        SetKnobRotation(knobRotation);

        // Reverse to get value
        var knobValue = (knobRotation - m_KnobMinAngle) / (m_MaxAngle - m_KnobMinAngle);
        SetKnobValue(knobValue);
    }

    void SetKnobRotation(float angle)
    {
        if (m_KnobAngleIncrement > 0)
        {
            var normalizeAngle = angle - m_KnobMinAngle;
            angle = (Mathf.Round(normalizeAngle / m_KnobAngleIncrement) * m_KnobAngleIncrement) + m_KnobMinAngle;
        }

        if (m_Handle != null)
            m_Handle.localEulerAngles = new Vector3(0.0f, angle, 0.0f);
    }

    void SetKnobValue(float tempvalue)
    {
        if (m_KnobClampedMotion)
            tempvalue = Mathf.Clamp01(tempvalue);

        if (m_KnobAngleIncrement > 0)
        {
            var angleRange = m_MaxAngle - m_KnobMinAngle;
            var angle = Mathf.Lerp(0.0f, angleRange, Knobvalue);
            angle = Mathf.Round(angle / m_KnobAngleIncrement) * m_KnobAngleIncrement;
            tempvalue = Mathf.InverseLerp(0.0f, angleRange, angle);
        }

        m_KnobValue = tempvalue;
        m_OnValueChange.Invoke(m_KnobValue);
    }

    float ValueToRotation()
    {
        return m_KnobClampedMotion ? Mathf.Lerp(m_KnobMinAngle, m_MaxAngle, m_KnobValue) : Mathf.LerpUnclamped(m_KnobMinAngle, m_MaxAngle, m_KnobValue);
    }

    void UpdateBaseKnobRotation()
    {
        m_BaseKnobRotation = Mathf.LerpUnclamped(m_KnobMinAngle, m_MaxAngle, m_KnobValue);
    }

    static float ShortestAngleDistance(float start, float end, float max)
    {
        var angleDelta = end - start;
        var angleSign = Mathf.Sign(angleDelta);

        angleDelta = Math.Abs(angleDelta) % max;
        if (angleDelta > (max * 0.5f))
            angleDelta = -(max - angleDelta);

        return angleDelta * angleSign;
    }

    #endregion

    #region Other Monobehavior Methods

    void OnDrawGizmosSelected()
    {
        var angleStartPoint = transform.position;

        if (m_Handle != null)
            angleStartPoint = m_Handle.position;

        const float k_AngleLength = 0.25f;

        if (m_JoystickMotion != JoystickType.LeftRight)
        {
            Gizmos.color = Color.green;
            var axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(m_MaxAngle, 0.0f, 0.0f) * Vector3.up) * k_AngleLength;
            var axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(-m_MaxAngle, 0.0f, 0.0f) * Vector3.up) * k_AngleLength;
            Gizmos.DrawLine(angleStartPoint, axisPoint1);
            Gizmos.DrawLine(angleStartPoint, axisPoint2);

            if (m_DeadZoneAngle > 0.0f)
            {
                Gizmos.color = Color.red;
                axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(m_DeadZoneAngle, 0.0f, 0.0f) * Vector3.up) * k_AngleLength;
                axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(-m_DeadZoneAngle, 0.0f, 0.0f) * Vector3.up) * k_AngleLength;
                Gizmos.DrawLine(angleStartPoint, axisPoint1);
                Gizmos.DrawLine(angleStartPoint, axisPoint2);
            }
        }

        if (m_JoystickMotion != JoystickType.FrontBack)
        {
            Gizmos.color = Color.green;
            var axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, 0.0f, m_MaxAngle) * Vector3.up) * k_AngleLength;
            var axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, 0.0f, -m_MaxAngle) * Vector3.up) * k_AngleLength;
            Gizmos.DrawLine(angleStartPoint, axisPoint1);
            Gizmos.DrawLine(angleStartPoint, axisPoint2);

            if (m_DeadZoneAngle > 0.0f)
            {
                Gizmos.color = Color.red;
                axisPoint1 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, 0.0f, m_DeadZoneAngle) * Vector3.up) * k_AngleLength;
                axisPoint2 = angleStartPoint + transform.TransformDirection(Quaternion.Euler(0.0f, 0.0f, -m_DeadZoneAngle) * Vector3.up) * k_AngleLength;
                Gizmos.DrawLine(angleStartPoint, axisPoint1);
                Gizmos.DrawLine(angleStartPoint, axisPoint2);
            }
        }



        const int k_CircleSegments = 16;
        const float k_SegmentRatio = 1.0f / k_CircleSegments;

        // Nothing to do if position radius is too small
        if (m_KnobPositionTrackedRadius <= Mathf.Epsilon)
            return;

        // Draw a circle from the handle point at size of position tracked radius
        var circleCenter = transform.position;

        if (m_Handle != null)
            circleCenter = m_Handle.position;

        var circleX = transform.right;
        var circleY = transform.forward;

        Gizmos.color = Color.green;
        var segmentCounter = 0;
        while (segmentCounter < k_CircleSegments)
        {
            var startAngle = (float)segmentCounter * k_SegmentRatio * 2.0f * Mathf.PI;
            segmentCounter++;
            var endAngle = (float)segmentCounter * k_SegmentRatio * 2.0f * Mathf.PI;

            Gizmos.DrawLine(circleCenter + (Mathf.Cos(startAngle) * circleX + Mathf.Sin(startAngle) * circleY) * m_KnobPositionTrackedRadius,
                circleCenter + (Mathf.Cos(endAngle) * circleX + Mathf.Sin(endAngle) * circleY) * m_KnobPositionTrackedRadius);
        }
    }

    void OnValidate()
    {
        m_DeadZoneAngle = Mathf.Min(m_DeadZoneAngle, m_MaxAngle * k_MaxDeadZonePercent);
    }
    #endregion
}


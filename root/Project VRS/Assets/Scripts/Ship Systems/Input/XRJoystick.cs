using System;
using System.Numerics;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// An interactable joystick that can move side to side, and forward and back by a direct interactor
    /// </summary>
    public class XRJoystick : XRGrabInteractable
    {
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

        #region Variables for Control

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
        [Tooltip("If true, the joystick will return to (0, 0) on release")]
        bool m_RecenterOnRelease = true;

        [SerializeField]
        [Tooltip("If true, the joystick will return to center not accounting for the Y value on release.\nNote : recenter will always take priority")]
        bool m_RecenterXOnRelease = true;

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

        Vector3 m_localRotationOnGrab;
        Vector3 m_lastInteractorRotation;

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
        public Vector2 value
        {
            get => m_Value;
            set
            {
                if (!m_RecenterOnRelease)
                {
                    SetValue(value);
                    SetHandleAngle(value * m_MaxAngle);
                }
            }
        }

        /// <summary>
        /// If true, the joystick will return to dead center on release
        /// </summary>
        public bool recenterOnRelease
        {
            get => m_RecenterOnRelease;
            set => m_RecenterOnRelease = value;
        }

        /// <summary>
        /// If true, the joysticks X value will return to center on release
        /// </summary>
        public bool recenterXOnRelease
        {
            get => m_RecenterXOnRelease;
            set => m_RecenterXOnRelease = value;
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

        #endregion 

        //if the controller is grabbed, on the start, save the Y rot of it
        //on release, reset it to zero
        //display it as a rot value
        //cast interactor as hand interactor, then get inputs from it, and save it whenever it changes


        /// <summary>
        /// Events to trigger when the joystick's x value changes
        /// </summary>
        public ValueChangeEvent onValueChangeX => m_OnValueChangeX;

        /// <summary>
        /// Events to trigger when the joystick's y value changes
        /// </summary>
        public ValueChangeEvent onValueChangeY => m_OnValueChangeY;

        #region Monobehaviors and VR stuff

        void Start()
        {
            if (m_RecenterOnRelease)
                SetHandleAngle(Vector2.zero);
        }

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
            m_localRotationOnGrab = GetTwistValue();
        }

        private void EndGrab(SelectExitEventArgs args)
        {
            UpdateValue();

            //if (m_RecenterXOnRelease)
            //{
            //    Vector2 centeredX = new Vector2(0f, m_Value.y);
            //    SetHandleAngle(new Vector2(0f, currentUpDownAngle));
            //    SetValue(centeredX);
            //}

            if (m_RecenterOnRelease)
            {
                SetHandleAngle(Vector2.zero);
                SetValue(Vector2.zero);
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
                }
            }
        }

        #endregion

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

        Vector3 GetTwistValue()
        {
            Vector3 currentTwistRot = m_Interactor.GetAttachTransform(this).localEulerAngles;
            return currentTwistRot;
        }

        float currentUpDownAngle;
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

            //injected
            currentUpDownAngle = upDownAngle;

            m_lastInteractorRotation = GetTwistValue();

            SetHandleAngle(new Vector2(leftRightAngle, upDownAngle));
            SetValue(stickValue);
        }

        void SetValue(Vector2 value)
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

            m_Handle.up = (transform.up * yComp) + (transform.right * xComp) + (transform.forward * zComp);

            //retrive original rotation
            Quaternion Swing, Twist;
            DecomposeSwingTwist(m_Handle.rotation, m_Handle.up, out Swing, out Twist);
            //Debug.Log(Twist);

            //make new rotation

            //set rot
        }

        void OnDrawGizmosSelected()
        {
            //get the base of the line
            var angleStartPoint = transform.position;

            //save our known angle length
            const float k_AngleLength = 0.25f;

            //Null check before proceeding
            if (m_Handle != null)
            {
                angleStartPoint = m_Handle.position;
            }

            //behavior for the LeftRight joystick type
            //this only draws the lines for the left and right lines
            if (m_JoystickMotion != JoystickType.LeftRight)
            {
                //green is the outer bounds of the joystick
                Gizmos.color = Color.green;
                DrawLines(new Vector3(m_MaxAngle, 0.0f, 0.0f));

                //red is the deadzone angle, where inputs arent updated
                if (m_DeadZoneAngle > 0.0f)
                {
                    Gizmos.color = Color.red;
                    DrawLines(new Vector3(m_DeadZoneAngle, 0.0f, 0.0f));
                }
            }

            //behavior for the FrontBack joystick type
            //this only draws the lines for the forward and backward lines
            if (m_JoystickMotion != JoystickType.FrontBack)
            {
                //green is the outer bounds of the joystick
                Gizmos.color = Color.green;
                DrawLines(new Vector3(0.0f, 0.0f, m_MaxAngle));

                //red is the deadzone angle, where inputs arent updated
                if (m_DeadZoneAngle > 0.0f)
                {
                    Gizmos.color = Color.red;
                    DrawLines(new Vector3(0.0f, 0.0f, m_DeadZoneAngle));  
                }
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
            m_DeadZoneAngle = Mathf.Min(m_DeadZoneAngle, m_MaxAngle * k_MaxDeadZonePercent);
        }

        public static void DecomposeSwingTwist(Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
        {
            Vector3 r = new Vector3(q.x, q.y, q.z);

            // singularity: rotation by 180 degree
            if (r.sqrMagnitude < float.Epsilon)
            {
                Vector3 rotatedTwistAxis = q * twistAxis;
                Vector3 swingAxis =
                  Vector3.Cross(twistAxis, rotatedTwistAxis);

                if (swingAxis.sqrMagnitude > float.Epsilon)
                {
                    float swingAngle =
                      Vector3.Angle(twistAxis, rotatedTwistAxis);
                    swing = Quaternion.AngleAxis(swingAngle, swingAxis);
                }
                else
                {
                    // more singularity: 
                    // rotation axis parallel to twist axis
                    swing = Quaternion.identity; // no swing
                }

                // always twist 180 degree on singularity
                twist = Quaternion.AngleAxis(180.0f, twistAxis);
                return;
            }

            // meat of swing-twist decomposition
            Vector3 p = Vector3.Project(r, twistAxis);
            twist = new Quaternion(p.x, p.y, p.z, q.w);
            twist = Quaternion.Normalize(twist);
            swing = q * Quaternion.Inverse(twist);
        }

    }
}



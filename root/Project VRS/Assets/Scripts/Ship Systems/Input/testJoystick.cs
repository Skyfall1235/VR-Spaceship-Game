using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class testJoystick : XRBaseInteractable
{
    // Assign the m_hand when it grabs/interacts with this joystick, and unassign on release.
    public Transform m_hand;
    public Transform m_handle;

    [SerializeField] float m_maxJoystickAngle = 45f;
    [SerializeField] float m_maxYawAngle = 30f;  // Maximum yaw twist angle in degrees
    [SerializeField] float m_returnDuration = 0.25f;  // Duration for the joystick to return to the initial rotation

    private Vector2 m_joystickVector;  // Backing field for the read-only property
    private float m_twistFloat;  // Backing field for the read-only property

    private Quaternion m_initialRotation;
    private Quaternion m_initialHandRotation;

    private float m_returnTimer = 0f;  // Timer to keep track of the return duration
    private bool m_isReturning = false;  // Flag to check if we are currently returning to the initial rotation

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
        m_joystickVector = GetJoyStickVector();
        // Update the twistFloat
        m_twistFloat = GetYawTwistFloat();

        print(GetJoyStickVector() + " : " + GetYawTwistFloat());
    }

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

    // Read-only property for joystickVector
    public Vector2 JoystickVector => m_joystickVector;
    // Read-only property for twistFloat
    public float TwistFloat => m_twistFloat;
}

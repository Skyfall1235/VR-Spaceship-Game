using UnityEngine;
using UnityEngine.Events;

public class ForwardHUDDisplayer : MonoBehaviour
{
    [SerializeField]
    private Transform m_targetObject; //the HUD / THE HMD

    [SerializeField]
    private TransformType m_transformSelection;

    [Range(0f, 90f)]
    [SerializeField]
    private float m_maximumOffset = 20f; // The maximum allowed offset in degrees

    [SerializeField]
    bool m_isLookingAtTarget;

    private void SetisLookingAtTarget(bool value)
    {
        if (value != m_isLookingAtTarget)
        {
            m_isLookingAtTarget = value;
            OnView.Invoke(value);
        }
    }

    [SerializeField]
    public OnLookEvent OnView = new OnLookEvent();

    [System.Serializable]
    public class OnLookEvent : UnityEvent<bool> { }

    private void FixedUpdate()
    {
        SetisLookingAtTarget(IsPointingAtTarget());
    }

    private bool IsPointingAtTarget()
    {
        // Calculate the vector from this object to the target object
        Vector3 targetDirection = m_targetObject.position - transform.position;
        targetDirection.Normalize();

        // Calculate the local transform direction based on the selected TransformType
        Vector3 localTransformDirection = PerformTransform(m_transformSelection);

        // Transform the local transform direction into the global direction
        Vector3 globalDirection = transform.TransformDirection(localTransformDirection);

        // Calculate the angle between the global direction and the target direction
        float angle = Vector3.Angle(globalDirection, targetDirection);

        Debug.DrawRay(transform.position, globalDirection, Color.red);
        Debug.DrawRay(transform.position, targetDirection, Color.green);

        // Check if the angle is within the allowed maximum offset
        if (angle <= m_maximumOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector3 PerformTransform(TransformType type)
    {
        switch (type)
        {
            case TransformType.X:
                return Vector3.right;

            case TransformType.Y:
                return Vector3.up;

            case TransformType.Z:
                return Vector3.forward;

            case TransformType.negX:
                return Vector3.left;

            case TransformType.negY:
                return Vector3.down;

            case TransformType.negZ:
                return Vector3.back;

            default:
                Debug.LogWarning("Invalid transform type!");
                return Vector3.zero;
        }
    }

    private enum TransformType
    {
        X,
        Y,
        Z,
        negX,
        negY,
        negZ
    }
}

using UnityEngine;
using UnityEngine.Events;

public class DetermineLookAtToTarget : MonoBehaviour
{
    public Transform TargetObject; // The object to check if it's pointing towards
    [SerializeField]
    private bool m_useOptionalBaseReference;
    [SerializeField]
    private Transform optionalBaseReference;
    [SerializeField]
    private float maximumOffset = 20f; // The maximum allowed offset in degrees
    [SerializeField]
    private TransformType transformSelection;

    public UnityEvent OnLookAtTarget = new UnityEvent();

    public UnityEvent OnLookAwayFromTarget = new UnityEvent();



    #region Data Structures and Management

    [SerializeField]
    private enum TransformType
    { 
        X,
        Y,
        Z,
        negX,
        negY,
        negZ
    }
    private Vector3 DetermineOptionalBaseReference()
    {
        if (optionalBaseReference != null && m_useOptionalBaseReference)
        {
            return optionalBaseReference.transform.position;
        }
        else
        {
            return transform.position;
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

    #endregion

    private void LateUpdate()
    {
        if (IsPointingAtTarget())
        {
            OnLookAtTarget.Invoke();
        }
        else
        {
            OnLookAwayFromTarget.Invoke();
        }
    }

    /// <summary>
    /// Checks if the object is currently pointing at a target object within a specified tolerance angle.
    /// </summary>
    /// <returns>
    /// True if the object is pointing at the target within the allowed angle offset, false otherwise.
    /// </returns>
    private bool IsPointingAtTarget()
    {
        // Calculate the vector from this object to the target object
        Vector3 targetDirection = TargetObject.position - DetermineOptionalBaseReference();
        targetDirection.Normalize();

        // Calculate the local transform direction based on the selected TransformType
        Vector3 localTransformDirection = PerformTransform(transformSelection);

        // Transform the local transform direction into the global direction
        Vector3 globalDirection = transform.TransformDirection(localTransformDirection);

        // Calculate the angle between the global direction and the target direction
        float angle = Vector3.Angle(globalDirection, targetDirection);

        Debug.DrawRay(DetermineOptionalBaseReference(), globalDirection, Color.red);
        Debug.DrawRay(DetermineOptionalBaseReference(), targetDirection, Color.green);

        // Check if the angle is within the allowed maximum offset
        if (angle <= maximumOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

}

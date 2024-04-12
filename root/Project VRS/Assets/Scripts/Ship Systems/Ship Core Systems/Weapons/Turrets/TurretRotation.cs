using UnityEngine;

public class TurretRotation : MonoBehaviour
{
    [SerializeField]
    readonly SO_TurretData turretData;


    [Header("Turret Axi")]
    [SerializeField]
    protected GameObject xAxisRotator;
    [SerializeField]
    protected GameObject yAxisRotator;
    [SerializeField]
    protected GameObject barrel;
    [SerializeField]
    protected GameObject projectileInstatiationPoint;
    [Tooltip("The maximum speed Fat which the turret rotates in degrees per second")]
    [SerializeField]
    protected Vector2 turretRotationSpeed = new Vector2(20, 20);





    private void TurnToLeadPosition(Vector3 targetPosition)
    {

        Vector3 currentDirection = barrel.transform.up;
        Debug.DrawRay(barrel.transform.position, currentDirection);
        Vector3 desiredDirection = (targetPosition - barrel.transform.position).normalized;
        Debug.DrawRay(barrel.transform.position, desiredDirection);

        Vector3 xAxisRotatorEulerAngles = xAxisRotator.transform.localRotation.eulerAngles;
        Vector3 yAxisRotatorEulerAngles = yAxisRotator.transform.localRotation.eulerAngles;

        Quaternion newXAxisRotation = Quaternion.Euler(Quaternion.LookRotation(desiredDirection, xAxisRotator.transform.up).eulerAngles.x, xAxisRotatorEulerAngles.y, xAxisRotatorEulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotatorEulerAngles.x, Quaternion.LookRotation(desiredDirection, yAxisRotator.transform.up).eulerAngles.y, yAxisRotatorEulerAngles.z);
        xAxisRotator.transform.localRotation = Quaternion.RotateTowards(xAxisRotator.transform.localRotation, newXAxisRotation, turretRotationSpeed.x * Time.deltaTime);
        yAxisRotator.transform.localRotation = Quaternion.RotateTowards(yAxisRotator.transform.localRotation, newYAxisRotation, turretRotationSpeed.y * Time.deltaTime);
    }
}

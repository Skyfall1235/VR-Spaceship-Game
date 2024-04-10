using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class TurretTest : TargetingWeapon
{
    public GameObject TARGET;
    public Rigidbody targetRB;
    TargetData newData;
    public GameObject prefabBullet;
    public Transform firePoint;
    private void Awake()
    {
        targetLeadDataStorage = new NativeArray<float3>(1, Allocator.Persistent);
        newData = new TargetData(TARGET, targetRB);
    }

    protected override void Fire()
    {
        base.Fire();
        Instantiate(prefabBullet, firePoint.position, firePoint.transform.rotation);
    }

    public override void Reload()
    {
        throw new System.NotImplementedException();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(!CoroutineIsFinished)
        {
            return;
        }

        //Start job

        StartCoroutine(ComputeTargetLead(barrel.transform.position, newData, projectileSpeed));

        TurnToLeadPosition(LeadPosition);
    }

    private void TurnToLeadPosition(Vector3 targetPosition)
    {
        Vector3 currentDirection = barrel.transform.up;
        //Debug.DrawRay(barrel.transform.position, currentDirection);
        Vector3 desiredDirection = (targetPosition - barrel.transform.position).normalized;
        //Debug.DrawRay(barrel.transform.position, desiredDirection);

        Vector3 xAxisRotatorEulerAngles = xAxisRotator.transform.localRotation.eulerAngles;
        Vector3 yAxisRotatorEulerAngles = yAxisRotator.transform.localRotation.eulerAngles;

        Quaternion newXAxisRotation = Quaternion.Euler(Quaternion.LookRotation(desiredDirection, xAxisRotator.transform.up).eulerAngles.x, xAxisRotatorEulerAngles.y, xAxisRotatorEulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotatorEulerAngles.x, Quaternion.LookRotation(desiredDirection, yAxisRotator.transform.up).eulerAngles.y, yAxisRotatorEulerAngles.z);
        xAxisRotator.transform.localRotation = Quaternion.RotateTowards(xAxisRotator.transform.localRotation, newXAxisRotation, turretRotationSpeed.x * Time.deltaTime);
        yAxisRotator.transform.localRotation = Quaternion.RotateTowards(yAxisRotator.transform.localRotation, newYAxisRotation, turretRotationSpeed.y * Time.deltaTime);
    }



    private void OnDestroy()
    {
        KillTracking();
    }

    private void OnDrawGizmos()
    {
        //draw a ray in direction of the guidnce command from the missile
        DrawLine(transform.position, LeadPosition, Color.green);
        //DrawLine(barrel.transform.position, barrel.transform.up, Color.red);

        void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawLine(start, end, color);
        }
    }
}

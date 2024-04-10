using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretTest : TargetingWeapon
{
    public GameObject TARGET;
    public Rigidbody targetRB;
    [SerializeField] GameObject xAxisRotator;
    [SerializeField] GameObject yAxisRotator;
    [SerializeField] GameObject barrel;
    TargetData newData;
    public float projectileSpeed = 100f;
    [Tooltip("The maximum speed at which the turret rotates in degrees per second")]
    [SerializeField] Vector2 turretRotationSpeed = new Vector2(20, 20);
    private void Awake()
    {
        targetLeadDataStorage = new NativeArray<float3>(1, Allocator.Persistent);
        newData = new TargetData(TARGET, targetRB);
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
        Debug.DrawRay(barrel.transform.position, currentDirection);
        Vector3 desiredDirection = (targetPosition - barrel.transform.position).normalized;
        Debug.DrawRay(barrel.transform.position, desiredDirection);

        Quaternion newXAxisRotation = Quaternion.Euler(Quaternion.LookRotation(desiredDirection, xAxisRotator.transform.up).eulerAngles.x, xAxisRotator.transform.localRotation.eulerAngles.y, xAxisRotator.transform.localRotation.eulerAngles.z);
        Quaternion newYAxisRotation = Quaternion.Euler(yAxisRotator.transform.localRotation.x, Quaternion.LookRotation(desiredDirection, yAxisRotator.transform.up).eulerAngles.y, yAxisRotator.transform.localRotation.eulerAngles.z);
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
        Vector3 startPoint = transform.position;
        Vector3 endPoint = LeadPosition;
        Debug.DrawLine(startPoint, endPoint, Color.green);
    }
}

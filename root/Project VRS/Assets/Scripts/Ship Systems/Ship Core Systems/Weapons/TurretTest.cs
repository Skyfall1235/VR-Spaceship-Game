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
    public GameObject xAxis;
    public GameObject yAxis;
    public GameObject Barrel;
    TargetData newData;
    public float projectileSpeed = 100f;

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

        StartCoroutine(ComputeTargetLead(Barrel.transform.position, newData, projectileSpeed));

        TurnToLeadPosition(LeadPosition);
    }

    private void TurnToLeadPosition(Vector3 position)
    {
        // Calculate target direction (excluding Y-axis)
        Vector3 targetDirection = position;
        targetDirection.y = 0; // Ignore Y-axis for horizontal rotation

        // Get forward direction of the turret base (normalized)
        Vector3 baseForward = transform.forward;

        // Calculate rotation for horizontal axis (turret base)
        Quaternion baseRotation = Quaternion.FromToRotation(baseForward, targetDirection.normalized);

        // Rotate the turret base
        yAxis.transform.rotation = baseRotation;

        // Now calculate rotation for vertical axis (gun barrel) relative to base
        Vector3 relativeTargetDirection = transform.InverseTransformDirection(targetDirection);

        // Get the up direction of the gun barrel (normalized)
        Vector3 barrelUp = transform.up;

        // Calculate rotation for vertical axis (gun barrel)
        Quaternion barrelRotation = Quaternion.FromToRotation(barrelUp, relativeTargetDirection.normalized);

        // Apply rotation to the gun barrel (child object)
        xAxis.transform.localRotation = barrelRotation;
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

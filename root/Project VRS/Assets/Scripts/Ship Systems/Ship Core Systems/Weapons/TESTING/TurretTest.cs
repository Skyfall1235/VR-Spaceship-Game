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

        //TurnToLeadPosition(LeadPosition);
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

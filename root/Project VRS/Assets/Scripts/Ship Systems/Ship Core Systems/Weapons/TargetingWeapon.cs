using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;

public abstract class TargetingWeapon : Weapon
{
    protected Vector3 LeadPosition = Vector3.zero;
    protected JobHandle ComputeTargetLeadJob;
    //target leading algorithm



    //ideally, this is where a unity job would go to give the angle for the next frame projection.


    //basically, we need a way for the turret to reach out to the targeter, get back a unity job, wiat for the job to finish , and then use the result.
    //since only the targeting system knows the locationdf of the objects, the weapons will need to interact with it in a dynamic by a non modifying method

    IEnumerator ComputeTargetLead(Vector3 gunPosition, TargetData targetData, float projectileSpeed)
    {
        NativeArray<float3> dataStorage = new NativeArray<float3>(1, Allocator.TempJob);
        try
        {
            //schedule the job
            Vector3 targetPosition = targetData.TargetGameObject.transform.position;
            Vector3 targetVelocity = targetData.TargetRB.velocity;

            ComputeTargetLeadJob = FindTargetLead(gunPosition, targetPosition, targetVelocity, projectileSpeed, dataStorage);
            yield return ComputeTargetLeadJob;
        }
        finally
        {
            //dispose and force completion
            ComputeTargetLeadJob.Complete();
        }
        LeadPosition = CalculateTargetLeadJob.Float3ToVector3(dataStorage[0]);
        dataStorage.Dispose();
    }






    protected JobHandle FindTargetLead(Vector3 playerPosition, Vector3 targetPosition, float3 targetAcceleration, float projectileSpeed, NativeArray<float3> nativeArrayRef)
    {
        CalculateTargetLeadJob job = new CalculateTargetLeadJob(playerPosition, targetPosition, targetAcceleration, projectileSpeed, nativeArrayRef);
        return job.Schedule();
    }




    [BurstCompile]
    public struct CalculateTargetLeadJob : IJob
    {

        readonly float3 PlayerPosition;
        readonly float3 TargetPosition;
        readonly float3 TargetAcceleration;
        readonly float ProjectileSpeed;
        //[0] should be the output
        [WriteOnly] NativeArray<float3> NativeArray;

        public CalculateTargetLeadJob(Vector3 playerPosition, Vector3 targetPosition,  float3 targetAcceleration, float projectileSpeed, NativeArray<float3> nativeArrayRef)
        {
            this.PlayerPosition = Vector3ToFloat3(playerPosition);
            this.TargetPosition = Vector3ToFloat3(targetPosition);
            this.TargetAcceleration = Vector3ToFloat3(targetAcceleration);
            this.ProjectileSpeed = projectileSpeed;
            this.NativeArray = nativeArrayRef;
        }

        public void Execute()
        {
            float3 LeadPositionRelativeToPosition = CalulcateLeadPosition(TargetPosition, TargetAcceleration, ProjectileSpeed);
            NativeArray[0] = LeadPositionRelativeToPosition;
        }

        float3 CalulcateLeadPosition(float3 targetPosition, float3 targetAcceleration, float projectileSpeed)
        {
            // Calculate lead position
            float distance = Distance(PlayerPosition, targetPosition);
            float travelTime = distance / projectileSpeed;
            float3 leadPosition = targetPosition + targetAcceleration * (travelTime * travelTime) / 2; // Consider constant acceleration
            return leadPosition;
        }

        #region basic math funcs

        float Distance(float3 a, float3 b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        public static float3 Vector3ToFloat3(Vector3 from)
        {
            return new float3(from.x, from.y, from.z);
        }

        public static Vector3 Float3ToVector3(float3 from)
        {
            return new Vector3(from.x, from.y, from.z);
        }

        #endregion
    }
}

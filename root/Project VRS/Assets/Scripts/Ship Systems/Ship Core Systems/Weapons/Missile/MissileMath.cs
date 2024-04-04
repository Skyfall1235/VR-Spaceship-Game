using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public partial class MissileBehavior 
{
    Coroutine m_guidanceCommandLoop;
    bool m_CorotuineFinishFlag = true;
    NativeArray<float3> memoryAllocation;
    JobHandle m_guidanceCommandJob;
    Vector3 guidanceVector = Vector3.zero;
    Rigidbody targetRB;


    private JobHandle ComputeGuidanceCommand(NativeArray<float3> NativeArrayReference)
    {
        //get the values in advance from the target and the missile itself
        Vector3 missilePosition = transform.position;
        Vector3 targetPosition = m_target.transform.position;
        if(targetRB == null)
        {
            targetRB = m_target.GetComponent<Rigidbody>();
        }
        Vector3 targetVelocity = targetRB.velocity;

        //create the new job struct with the parameters
        ComputeGuidanceCommandJob job = new ComputeGuidanceCommandJob(targetPosition, missilePosition, targetVelocity, NavigationGain, Time.deltaTime, NativeArrayReference);
        return job.Schedule();
    }

    //we should try to run this in fixed update so it stays consistent across devices
    private IEnumerator ComputeAndExecuteGuidanceCommand()
    {
        try
        {
            if(m_target == null)
            {
                yield break;
            }
            //schedule the job
            m_guidanceCommandJob = ComputeGuidanceCommand(memoryAllocation);
            yield return m_guidanceCommandJob;
        }
        finally
        {
            //dispose and force completion
            m_guidanceCommandJob.Complete();
        }


        guidanceVector = ComputeGuidanceCommandJob.Float3ToVector3(memoryAllocation[0]);

        //notify the fixed update that the corotuine is finished and the next job can be scheduled
        m_CorotuineFinishFlag = true;
    }

    

    private void KillGuidance()
    {
        StopCoroutine(m_guidanceCommandLoop);
        //force a completion of the last step, then kill the native array AFTER. (i think this avoids a memory leak?)
        m_guidanceCommandJob.Complete();
        memoryAllocation.Dispose();
    }

}

[BurstCompile]
public struct ComputeGuidanceCommandJob : IJob
{
    readonly float3 targetPosition;
    readonly float3 missilePosition;
    readonly float3 targetVelocity;
    readonly float navigationGain;
    readonly float deltaTime;
    //the location that we need to story the memory
    //should write ap ersistant 2 slots.
    //[0] is the guidance output;
    //[1] is the old distance
    //[2] is the previous target velocity
    [WriteOnly] NativeArray<float3> guidanceVariables;

    //creation struct to setup the job
    public ComputeGuidanceCommandJob
        (
        Vector3 TargetPosition, 
        Vector3 MissilePosition, 
        Vector3 TargetVelocity, 
        float NavigationalGain, 
        float DeltaTime, 
        NativeArray<float3> nativeArrayReference
        )
    {
        this.targetPosition = Vector3ToFloat3(TargetPosition);
        this.missilePosition = Vector3ToFloat3(MissilePosition);
        this.targetVelocity = Vector3ToFloat3(TargetVelocity);
        this.navigationGain = NavigationalGain;
        this.deltaTime = DeltaTime;
        this.guidanceVariables = nativeArrayReference;
    }

    public void Execute()
    {
        float3 guidanceCommand = CalculateGuidanceCommand();
        guidanceVariables[0] = guidanceCommand;
    }


    private float3 CalculateGuidanceCommand()
    {
        //Augmented Proportional Navigation(APN)

        float3 LOS = targetPosition - missilePosition;

        float3 LOSDelta = LOS - guidanceVariables[1];
        float LOSRate = GetFloat3Magnitude(LOSDelta);
        //needed
        float invertedLOSRate = -LOSRate;
        //this should be a non zero number?
        float3 accelerationCompensation = CalculateAccelerationCompensation(LOS, CalculateAcceleration());

        //THE SECRET FORMULA
        //A_cmd = (N * Vc * LOS_Rate) + ((N * Nt) / 2)
        
        float3 guidanceCommand = (navigationGain * invertedLOSRate * LOSRate) + ((navigationGain * accelerationCompensation) * 0.5f);
        return guidanceCommand;

        //current position of the target
        //the missile                                           needed for new distance
        //the navigation gain                                   N
        //the old RTM                                           old distance 
        //the acceleration (if 0 set to gravity / at least 1)   Nt
        //the negative LOSRate                                  Vc
        //the LOS delta                                         new distance - old distance
        //LOS rate                                              delta magnitude
        //RTM IS DISTANCE FROM LOCATION TO LOCATION
        //EDIT: some names have changed regarding the formula above


        //an = N*Vc*(dλ / dt)
        //the original formula from the paper by L.M., for reference
    }


    float3 CalculateAccelerationCompensation(float3 LOS, float3 targetAcceleration)
    {
        // Project target's acceleration onto LOS vector
        float projectionMagnitude = DotProduct(targetAcceleration, LOS);
        float3 projection = projectionMagnitude * NormalizeVector(LOS);

        // Calculate the component of acceleration perpendicular to LOS
        float3 Nt = targetAcceleration - projection;
        return Nt;
    }

    float3 CalculateAcceleration()
    {
        //get the acceleration using he up to date velocity and previous velocity
        float3 acceleration = (targetVelocity - guidanceVariables[2]) / deltaTime;
        //depricate the traget velocity so that it is now the old velocity
        guidanceVariables[2] = targetVelocity;
        return acceleration;
    }

    float DotProduct(float3 a, float3 b)
    {
        float product = a.x * b.x + a.y * b.y + a.z * b.z;
        return product;
    }

    float GetFloat3Magnitude(float3 floatVal)
    {
        return (float)Mathf.Sqrt(floatVal.x * floatVal.x + floatVal.y * floatVal.y + floatVal.z * floatVal.z);
    }

    float3 NormalizeVector(float3 vector)
    {
        float magnitude = GetFloat3Magnitude(vector);
        if (magnitude > 0)
        {
            return new float3(vector.x / magnitude, vector.y / magnitude, vector.z / magnitude);
        }
        else
        {
            return float3.zero;
        }
    }

    public static float3 Vector3ToFloat3(Vector3 from)
    {
        return new float3(from.x, from.y, from.z);
    }

    public static Vector3 Float3ToVector3(float3 from)
    {
        return new Vector3(from.x, from.y, from.z);
    }
}
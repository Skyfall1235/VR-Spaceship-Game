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


    private JobHandle ComputeGuidanceCommand(NativeArray<float3> NativeArrayReference)
    {
        //get the values in advance from the target and the missile itself
        Vector3 missilePosition = transform.position;
        Vector3 targetPosition = m_target.transform.position;
        Vector3 missileVelocity = transform.GetComponent<Rigidbody>().velocity;
        Vector3 targetVelocity = m_target.GetComponent<Rigidbody>().velocity;

        //create the new job struct with the parameters
        ComputeGuidanceCommandJob job = new ComputeGuidanceCommandJob(missilePosition, targetPosition, missileVelocity, targetVelocity, NavigationGain, NativeArrayReference);
        return job.Schedule();
    }

    //we should try to run this in fixed update so it stays consistent across devices
    private IEnumerator ComputeAndExecuteGuidanceCommand()
    {
        //setup
        float3 output = float3.zero;
        memoryAllocation = new NativeArray<float3>(1, Allocator.TempJob);
        //wait for execution to finish
        try
        {
            m_guidanceCommandJob = ComputeGuidanceCommand(memoryAllocation);
            yield return m_guidanceCommandJob;
        }
        finally
        {
            //force completion
            m_guidanceCommandJob.Complete();
            //if it is finished, pull the values out to a usable data form and start doing work with it
            output = memoryAllocation[0];
            memoryAllocation.Dispose();
        }

        //apply guidance to trajectory
        guidanceVector = ComputeGuidanceCommandJob.Float3ToVector3(output);
        ApplyGuidanceCommand(guidanceVector);

        //notify the fixed update that the corotuine is finished and the next job can be scheduled
        m_CorotuineFinishFlag = true;
    }

    

    private void KillGuidance()
    {
        StopCoroutine(m_guidanceCommandLoop);
        //force a completion of the last step, then kill the native array AFTER. (i think this avoids a memory leak?)
        m_guidanceCommandJob.Complete();
    }

}

[BurstCompile]
public struct ComputeGuidanceCommandJob : IJob
{
    //needed parameters for the mathematics
    readonly float3 missilePosition;
    readonly float3 targetPosition;
    readonly float3 missileVelocity;
    readonly float3 targetVelocity;
    //the gain of the course correction
    readonly float navGain;
    //the location that we need to story the memory
    [WriteOnly] NativeArray<float3> guidanceCommand;

    //creation struct to setup the job
    public ComputeGuidanceCommandJob(Vector3 mP, Vector3 tP, Vector3 mV, Vector3 tV, float nG, NativeArray<float3> nativeArrayReference)
    {
        this.missilePosition = Vector3ToFloat3(mP);
        this.targetPosition = Vector3ToFloat3(tP);
        this.missileVelocity = Vector3ToFloat3(mV);
        this.targetVelocity = Vector3ToFloat3(tV);
        this.navGain = nG;
        this.guidanceCommand = nativeArrayReference;
        //we need the value, but we plan to fill it later so dont bother with it right now
    }

    public void Execute()
    {
        //get the value
        float3 computedCommand = CalculateGuidanceCommand();
        //save it to the output
        guidanceCommand[0] = computedCommand;
    }

    private float3 CalculateGuidanceCommand()
    {
        float3 relativePosition = targetPosition - missilePosition;
        //fix in a moment
        float3 relativeVelovicity = targetVelocity - missileVelocity;

        // 3. Line-Of-Sight (LOS) calculations

        float3 LOSRate = CalculateLOSRate(relativePosition, relativeVelovicity);

        // 4. Target Acceleration Estimation (omitted for simplicity)
        //EDIT: this can be implemented, but doing so would hurt my brain, so we will continue t omit until nessicary

        // 5. APN Guidance Command
        float3 propTerm = navGain * LOSRate;
        float3 guidanceCommand = propTerm; // No target acceleration estimation in this example
        return guidanceCommand;
    }

    private Vector3 CalculateLOSRate(Vector3 relativePosition, Vector3 relativeVelocity)
    {
        // This function needs to be implemented based on calculus or numerical differentiation 
        //EDIT: we will use calculus because its more precise and generally avoids rounding errors
        float3 LOS = relativePosition.normalized;
        float3 LOSRate = Vector3.Cross(relativeVelocity, LOS) / Mathf.Pow(relativePosition.magnitude, 2);

        //  to find the time derivative of the LOS vector.
        return LOSRate;
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



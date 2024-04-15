using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Burst;
using Unity.Mathematics;

//FOR THIS SCRIPT TO WORK, WE HAVE T START USING TAGS OR LAYERS.
//LAYERS WILL BE USES FOR PHYSICS AND TAGS WILL BE USED TO DIFFERENCIATE OBJECTS IN SCENE

[RequireComponent(typeof(Collider))]
public class TargetHandler : MonoBehaviour
{
    //list of all known targets
    public int PriorityTarget = 0;
    public string EnemyTag = "Enemy";
    public SphereCollider DetectionCollider;
    public float ColliderRadius = 10f;
    public List<TargetData> RegisteredTargets = new List<TargetData>();
    public UnityEvent<TargetData> OverridePriorityTarget;

    //register and deregister targets

    private void RegisterNewTarget(GameObject target)
    {
        RegisteredTargets.Add(new TargetData(target, target.GetComponent<Rigidbody>(), false));
    }
    private void UnregisterTarget(GameObject target)
    {
        bool targetRemoved = false;  // Flag to track removal

        // Loop through the list
        for (int i = RegisteredTargets.Count - 1; i >= 0; i--)
        {
            // Check if target GameObject matches the TargetData's transform
            if (RegisteredTargets[i].TargetGameObject == target)
            {
                // Remove the TargetData from the list at the current index
                RegisteredTargets.RemoveAt(i);
                targetRemoved = true;
                break;  // Exit the loop after removal
            }
        }

        // Handle scenario where target wasn't found
        if (!targetRemoved)
        {
            Debug.LogWarning($"Target {target.name} not found in RegisteredTargets");
        }
    }

    /// <summary>
    /// Sorts through a list of targets, determines the priority of that target then sort the list based on that priority. This is a multithreaded task
    /// </summary>
    private IEnumerator SortPriorityTargets()
    {
        if(RegisteredTargets.Count <= 0)
        {
            yield break;
        }
        //create handle list and variable arrays to pass into the job system
        NativeList<JobHandle> jobHandles = new NativeList<JobHandle>(Allocator.Temp);
        NativeArray<float> scoreResults = new NativeArray<float>(RegisteredTargets.Count, Allocator.TempJob);
        NativeArray<float3> targetGameObjectPositions = new NativeArray<float3>(RegisteredTargets.Count, Allocator.TempJob);
        NativeArray<float3> targetGameObjectVelocities = new NativeArray<float3>(RegisteredTargets.Count, Allocator.TempJob);
        //Define a delegate for checking whether all the jobs are complete
        Func<bool> CheckCalculateScoreJobCompleted = delegate ()
        {
            int totalJobs = jobHandles.Length;
            int currentJobs = 0;
            foreach (JobHandle job in jobHandles)
            {
                if (job.IsCompleted)
                {
                    currentJobs++;
                }
            }
            if (currentJobs >= totalJobs)
            {
                return true;
            }
            else
            {
                return false;
            }
        };

        //fill out the incoming parameter lists
        for (int i = 0; i < RegisteredTargets.Count; i++)
        {
            targetGameObjectPositions[i] = RegisteredTargets[i].TargetGameObject.transform.position;
            targetGameObjectVelocities[i] = RegisteredTargets[i].TargetRB.velocity;            
        }
        //Create handles for jobs
        for(int i = 0; i < Mathf.CeilToInt((float)RegisteredTargets.Count / (float)SystemInfo.processorCount); i++)
        {
            JobHandle jobHandle = CalculateScoreJob(scoreResults, transform.position, targetGameObjectPositions, targetGameObjectVelocities, ColliderRadius);
            jobHandles.Add(jobHandle);
        }
        //start completing all jobs and return control to the main thread until the jobs are completed
        JobHandle.CompleteAll(jobHandles.AsArray());
        yield return new WaitUntil(CheckCalculateScoreJobCompleted);
        for (int i = 0; i < RegisteredTargets.Count -1 ; i++)
        {
            TargetData dataCopy = new TargetData(RegisteredTargets[i].TargetGameObject, RegisteredTargets[i].TargetRB, RegisteredTargets[i].IsEmpty, RegisteredTargets[i].TargetScore);
            RegisteredTargets[i] = new TargetData(dataCopy.TargetGameObject, dataCopy.TargetRB, dataCopy.IsEmpty, scoreResults[i]);
        }
        //dispose of everything
        scoreResults.Dispose();
        targetGameObjectPositions.Dispose();
        targetGameObjectVelocities.Dispose();
        //Look upon my sins. This is the easiest way I could find to sort the target priorites. TODO: Find a less jank looking solution
        RegisteredTargets.Sort(delegate (TargetData target1, TargetData target2)
        {

            if(target1.TargetScore < target2.TargetScore)
            {
                return -1;
            }
            else if (target1.TargetScore > target2.TargetScore)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
    }
    /// <summary>
    /// Schedules a CalculateScore job and returns the handle
    /// </summary>
    /// <param name="scores">Incoming NativeArray of scores</param>
    /// <param name="shipPosition">The current position of the ship</param>
    /// <param name="targetPositions">A NativeArray containing the positions of all the targets</param>
    /// <param name="targetVelocities">A NativeArray containing the velocities of all the targets</param>
    /// <param name="colliderRadius">The radius of the detection collider</param>
    /// <returns></returns>
    private JobHandle CalculateScoreJob(NativeArray<float> scores, Vector3 shipPosition, NativeArray<float3> targetPositions, NativeArray<float3> targetVelocities, float colliderRadius)
    {
        CalculateScore score =new CalculateScore(scores, shipPosition.Vector3ToFloat3(), targetPositions, targetVelocities, colliderRadius);
        return score.Schedule(targetPositions.Length, Mathf.CeilToInt((float)targetPositions.Length / (float)SystemInfo.processorCount));
    }

    [BurstCompile]
    public struct CalculateScore : IJobParallelFor
    {
        NativeArray<float> scores;

        float3 shipPosition;
        readonly NativeArray<float3> targetPosition;
        readonly NativeArray<float3> targetVelocity;

        float colliderRadius;
        const int distanceHeuristicMultiplier = 1;
        const int facingHeuristicMultiplier = 1;
        public CalculateScore(NativeArray<float> scores, float3 shipPosition, NativeArray<float3> targetPosition, NativeArray<float3> targetVelocity, float colliderRadius)
        {
            this.shipPosition = shipPosition;
            this.targetPosition = targetPosition;
            this.targetVelocity = targetVelocity;
            this.colliderRadius = colliderRadius;      
            this.scores = scores;
        }
        public void Execute(int index)
        {
            //Adds a number between 1 and 0 representing how close the velocity of the target is to being aimed directly at the ship 
            scores[index] += ExtensionMethods.DotProduct(targetVelocity[index].NormalizeVector(), (shipPosition - targetPosition[index] ).NormalizeVector()).Remap(-1, 1, 0, 1) * facingHeuristicMultiplier;
            //Adds a number between 1 and 0 representing the distance from the ship
            scores[index] += Vector3.Distance(shipPosition, targetPosition[index]).Remap(colliderRadius, 0, 0, 1) * distanceHeuristicMultiplier;
        }
        
    }



    public int FindBestTargetForPriority(GameObject turretGO)
    {
        //take into account the position of the turret

        //comapare and see which targets are in the field of view

        //see which targets are closest

        //attempt to take the closest one that is in view relative to the turret

        //if no target is in the FOV, target the one closest to entering the FOV

        //we are retuning an int because that the index of he targe in the target list

        return 0;
    }

    private void CompareForEnemyAndRunAction(GameObject target, Action<GameObject> action)
    {
        if (target.CompareTag(EnemyTag))
        {
            action(target);
            StartCoroutine(SortPriorityTargets());
        }
    }

    #region Monobehavior Methods

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        CompareForEnemyAndRunAction(target, RegisterNewTarget);
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject target = other.gameObject;
        CompareForEnemyAndRunAction(target, UnregisterTarget);
    }

    private void OnValidate()
    {
        DetectionCollider.radius = ColliderRadius;
    }

    #endregion
}

[Serializable]
public struct TargetData
{
    public GameObject TargetGameObject;
    public Rigidbody TargetRB;
    public bool IsEmpty;
    public float TargetScore;

    public TargetData(GameObject targetGameObject, Rigidbody targetRB, bool isEmpty = true, float targetScore = 0)
    {
        this.TargetGameObject = targetGameObject;
        this.TargetRB = targetRB;
        this.IsEmpty = isEmpty;
        this.TargetScore = targetScore;
    }
}

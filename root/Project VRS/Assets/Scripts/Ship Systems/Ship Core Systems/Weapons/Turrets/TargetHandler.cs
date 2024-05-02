using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.Burst;
using Unity.Mathematics;
using static UnityEngine.GraphicsBuffer;

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

    JobHandle m_jobHandle;
    NativeList<float> m_scoreResults;
    NativeList<float3> m_targetGameObjectPositions;
    NativeList<float3> m_targetGameObjectVelocities;
    bool m_routineComplete = true;

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
        if(RegisteredTargets == null || RegisteredTargets.Count <= 0 || m_routineComplete == false)
        {
            yield break;
        }
        m_routineComplete = false;
        m_scoreResults.Clear();
        m_targetGameObjectPositions.Clear();
        m_targetGameObjectVelocities.Clear();
        m_scoreResults.Resize(RegisteredTargets.Count, NativeArrayOptions.ClearMemory);
        m_targetGameObjectPositions.Resize(RegisteredTargets.Count, NativeArrayOptions.ClearMemory);
        m_targetGameObjectVelocities.Resize(RegisteredTargets.Count, NativeArrayOptions.ClearMemory);
        NativeArray<float> scoreResults = m_scoreResults.AsArray();
        NativeArray<float3> targetGameObjectPositions = m_targetGameObjectPositions.AsArray();
        NativeArray<float3> targetGameObjectVelocities = m_targetGameObjectVelocities.AsArray();
        //create handle list and variable arrays to pass into the job system

        //fill out the incoming parameter lists
        for (int i = 0; i < RegisteredTargets.Count; i++)
        {
            targetGameObjectPositions[i] = RegisteredTargets[i].TargetGameObject.transform.position;
            targetGameObjectVelocities[i] = RegisteredTargets[i].TargetRB.velocity;       
        }
        //Create handles for jobs

        m_jobHandle = CalculateScoreJob(scoreResults, transform.position, targetGameObjectPositions, targetGameObjectVelocities, ColliderRadius);

        //start completing all jobs and return control to the main thread until the jobs are completed
       
        yield return new WaitUntil(() => m_jobHandle.IsCompleted);
        m_jobHandle.Complete();
        for (int i = 0; i < RegisteredTargets.Count -1 ; i++)
        {
            TargetData dataCopy = new TargetData(RegisteredTargets[i].TargetGameObject, RegisteredTargets[i].TargetRB, RegisteredTargets[i].IsEmpty, RegisteredTargets[i].TargetScore);
            RegisteredTargets[i] = new TargetData(dataCopy.TargetGameObject, dataCopy.TargetRB, dataCopy.IsEmpty, scoreResults[i]);
        }

        //Sort the list
        RegisteredTargets.Sort((TargetData target1, TargetData target2) => target1.TargetScore.CompareTo(target2.TargetScore));
        m_routineComplete = true;
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
        const int batchSize = 32;
        CalculateScore score =new CalculateScore(scores, shipPosition.Vector3ToFloat3(), targetPositions, targetVelocities, colliderRadius);
        return score.Schedule(targetPositions.Length, batchSize);
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

    private void CompareForEnemyAndRunAction(GameObject target, Action<GameObject> action)
    {
        if (target.CompareTag(EnemyTag))
        {
            action(target);
        }
    }

    private void KillTargetingJob()
    {
        m_jobHandle.Complete();
        m_scoreResults.Dispose();
        m_scoreResults = default;
        m_targetGameObjectPositions.Dispose();
        m_targetGameObjectPositions = default;
        m_targetGameObjectVelocities.Dispose();
        m_targetGameObjectVelocities = default;

    }

    #region Monobehavior Methods

    private void Awake()
    {
        m_scoreResults = new NativeList<float>(Allocator.Persistent);
        m_targetGameObjectPositions = new NativeList<float3>(Allocator.Persistent);
        m_targetGameObjectVelocities = new NativeList<float3>(Allocator.Persistent);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject target = other.gameObject;
        CompareForEnemyAndRunAction(target, RegisterNewTarget);
        if (target.CompareTag(EnemyTag))
        {
            StartCoroutine(SortPriorityTargets());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject target = other.gameObject;
        if (target.CompareTag(EnemyTag))
        {
            StartCoroutine(SortPriorityTargets());
        }
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

    public void OnDestroy()
    {
        KillTargetingJob();
    }

    private void OnApplicationQuit()
    {
        KillTargetingJob();
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


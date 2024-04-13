using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    /// <summary>
    /// Sorts through a list of targets, determines the priority of that target then sort the list based on that priority
    /// </summary>
    /// <param name="listToSort">The list of targets that will be sorted</param>
    private void SortPriorityTargets(ref List<TargetData> listToSort)
    {
        //Calculate priority for each list entry
        for(int i = 0; i < listToSort.Count; i++)
        {
            if (!listToSort[i].isEmpty)
            {
                listToSort[i] = new TargetData(listToSort[i].targetGameObject, listToSort[i].targetRB, false, CalculateScore(listToSort[i]));
            }
        }
        //Look upon my sins. This is the easiest way I could find to sort the target priorites. TODO: Find a less jank looking solution
        listToSort.Sort(delegate (TargetData target1, TargetData target2)
        {
            if(target1.targetScore == null && target2.targetScore == null)
            {
                return 0;
            }
            else if(target1.targetScore != null && target2.targetScore == null)
            {
                return 1;
            }
            else if (target1.targetScore == null && target2.targetScore != null)
            {
                return -1;
            }
            else if(target1.targetScore < target2.targetScore)
            {
                return -1;
            }
            else if (target1.targetScore > target2.targetScore)
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
    /// Calculates the priority score of a given target
    /// </summary>
    /// <param name="targetToClaculateScoreFor">The target to calculate the score for</param>
    /// <returns>The calculated score as an integer</returns>
    private float CalculateScore(TargetData targetToCalculateScoreFor)
    {
        float calculatedScore = 0;
        Vector3 shipPosition = transform.root.position;
        Vector3 targetPosition = targetToCalculateScoreFor.targetGameObject.transform.position;
        //Adds a number between 1 and 0 representing how close the velocity of the target is to being aimed directly at the ship 
        calculatedScore += Vector3.Dot(targetToCalculateScoreFor.targetRB.velocity.normalized, (targetPosition - shipPosition).normalized).Remap(-1, 1, 0, 1);
        //Adds a number between 1 and 0 representing the distance from the ship
        calculatedScore += Vector3.Distance(shipPosition, targetPosition).Remap(0, ColliderRadius, 0, 1);
        return calculatedScore;
    }

    private void UnregisterTarget(GameObject target)
    {
        bool targetRemoved = false;  // Flag to track removal

        // Loop through the list
        for (int i = RegisteredTargets.Count - 1; i >= 0; i--)
        {
            // Check if target GameObject matches the TargetData's transform
            if (RegisteredTargets[i].targetGameObject == target)
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
    public GameObject targetGameObject;
    public Rigidbody targetRB;
    public bool isEmpty;
    public float? targetScore;

    public TargetData(GameObject targetGameObject, Rigidbody targetRB, bool isEmpty = true, float? targetScore = null)
    {
        this.targetGameObject = targetGameObject;
        this.targetRB = targetRB;
        this.isEmpty = isEmpty;
        this.targetScore = targetScore;
    }
}

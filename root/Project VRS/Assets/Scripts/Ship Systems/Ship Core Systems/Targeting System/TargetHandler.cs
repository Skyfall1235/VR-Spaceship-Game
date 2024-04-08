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
    public Collider DetectionCollider;
    public List<TargetData> RegisteredTargets = new List<TargetData>();
    public UnityEvent<TargetData> OverridePriorityTarget;


    //register and deregister targets

    private void RegisterNewTarget(GameObject target)
    {
        RegisteredTargets.Add(new TargetData(target, target.GetComponent<Rigidbody>()));
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

    public void SetTargetAsPriority(int targetIndex)
    {

    }

    private void CompareForEnemyAndRunAction(GameObject target, Action<GameObject> action)
    {
        if (target.CompareTag(EnemyTag))
        {
            action(target);
        }
    }

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
}

public struct TargetData
{
    public GameObject TargetGameObject;
    public Rigidbody TargetRB;

    public TargetData(GameObject targetGameObject, Rigidbody targetRB)
    {
        this.TargetGameObject = targetGameObject;
        this.TargetRB = targetRB;
    }
}

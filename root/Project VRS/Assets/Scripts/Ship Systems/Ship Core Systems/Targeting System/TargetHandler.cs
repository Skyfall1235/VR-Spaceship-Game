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
    public float ColliderSize = 10f;
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
        DetectionCollider.radius = ColliderSize;
    }

    #endregion
}

[Serializable]
public struct TargetData
{
    public GameObject TargetGameObject;
    public Rigidbody TargetRB;
    public bool isEmpty;

    public TargetData(GameObject targetGameObject, Rigidbody targetRB, bool isEmpty = true)
    {
        this.TargetGameObject = targetGameObject;
        this.TargetRB = targetRB;
        this.isEmpty = isEmpty;
    }
}

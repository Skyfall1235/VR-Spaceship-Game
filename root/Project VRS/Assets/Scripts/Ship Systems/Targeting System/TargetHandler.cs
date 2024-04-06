using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    //list of all known targets
    public TargetData PriorityTarget;

    public List<TargetData> RegisteredTargets = new List<TargetData>();

    //register and deregister targets

    private void RegisterNewTarget(GameObject target)
    {
        RegisteredTargets.Add(new TargetData(target, target.transform, target.GetComponent<Rigidbody>()));
    }

    private void UnregisterNewTarget(GameObject target)
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

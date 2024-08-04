using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a scanner component capable of performing sphere and ray casts within a specified view distance and layers.
/// </summary>
[Serializable]
public class BC_Scanner
{
    [SerializeField]
    private float viewDistance = 10f;

    [SerializeField]
    private float sphereCastRadius = 1f;

    [SerializeField]
    private float scanTimeout = 5f;

    [SerializeField]
    private Transform scanLocation;

    [SerializeField]
    private LayerMask scanLayers;

    /// <summary>
    /// Scanner Query for what types of colliders the scanner can hit
    /// </summary>
    public QueryTriggerInteraction interactionQuery = QueryTriggerInteraction.Ignore;

    /// <summary>
    /// Event invoked when a target is found during a scan.
    /// </summary>
    public UnityEvent<GameObject> OnFindTargetDuringScan = new UnityEvent<GameObject>();

    /// <summary>
    /// Performs a sphere cast and returns the hit object if found, otherwise returns null.
    /// </summary>
    /// <returns>The hit object or null if no object was found.</returns>
    public GameObject PerformSphereCast()
    {
        RaycastHit hit;
        if (Physics.SphereCast(new Ray(), sphereCastRadius, out hit, viewDistance, scanLayers, interactionQuery))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Performs a raycast and returns the hit object if found, otherwise returns null.
    /// </summary>
    /// <returns>The hit object or null if no object was found.</returns>
    public GameObject PerformRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(scanLocation.position, scanLocation.forward, out hit, viewDistance, scanLayers, interactionQuery))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Continuously performs a scan using the provided raycast action until a target is found or the timeout is reached.
    /// </summary>
    /// <param name="RaycastAction">The action to perform a raycast.</param>
    /// <returns>An IEnumerator to be used with StartCoroutine.</returns>
    public IEnumerator ScanOverTime(Func<GameObject> RaycastAction)
    {
        float startTime = Time.time;

        while (true)
        {
            GameObject foundObject = RaycastAction();
            if (foundObject != null)
            {
                OnFindTargetDuringScan.Invoke(foundObject);
                yield break;
            }

            if (Time.time - startTime > scanTimeout)
            {
                Debug.LogWarning("ScanOverTime timed out");
                yield break;
            }

            yield return null;
        }
    }
}

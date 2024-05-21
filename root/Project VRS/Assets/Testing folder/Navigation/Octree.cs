using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octree : MonoBehaviour
{
    OctreeNode _root;
    [SerializeField] Collider[] worldGameObjects;
    int maxDepth = 5;
    float time;
    private void Start()
    {
        DateTime before = DateTime.Now;
        _root = new OctreeNode(CalculateOctreeRootBounds(worldGameObjects), 1, maxDepth);
        _root.DivideAndAddNode(worldGameObjects);
        DateTime after = DateTime.Now;
        TimeSpan duration = after.Subtract(before);
        Debug.Log("Duration in milliseconds: " + duration.Milliseconds);
    }
    Bounds CalculateOctreeRootBounds(Collider[] gameObjects)
    {
        Bounds bounds = new Bounds();
        foreach (Collider gameObject in gameObjects)
        {
            bounds.Encapsulate(gameObject.GetComponent<Collider>().bounds);
        }
        float MaxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        Vector3 sizeVector = new Vector3(MaxSize, MaxSize, MaxSize) * 0.5f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);
        return bounds;
    }
    private void OnDrawGizmos()
    {
        if (_root != null) 
        {
            _root.DrawNode();
        }
    }
}

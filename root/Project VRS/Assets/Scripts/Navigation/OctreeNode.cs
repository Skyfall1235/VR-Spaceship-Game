using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeNode 
{
    Bounds _nodeBounds;
    int _nodeDepth;
    int _maxDepth;
    List <OctreeNode> children = new List <OctreeNode>();
    public OctreeNode(Bounds nodeBounds, int nodeDepth, int maxDepth)
    {
        _nodeBounds = nodeBounds;
        _nodeDepth = nodeDepth;
        _maxDepth = maxDepth;
    }   
    public void DivideAndAddNode(Collider[] gameObjectsToCheck)
    {
        List<Bounds> childBoundsToCheck = GetChildBounds();
        foreach(Bounds childBounds in childBoundsToCheck)
        {
            if(_nodeDepth < _maxDepth)
            {
                List<Collider> collidersToCheckInChild = new List<Collider>();
                foreach (Collider collider in gameObjectsToCheck)
                {
                    if(childBounds.Intersects(collider.bounds)) 
                    {
                        collidersToCheckInChild.Add(collider);
                    }
                }
                if (collidersToCheckInChild.Count > 0)
                {
                    OctreeNode newChild =  new OctreeNode(childBounds, _nodeDepth + 1, _maxDepth);
                    newChild.DivideAndAddNode(collidersToCheckInChild.ToArray());
                    children.Add(newChild);
                }
            }
        }
        
    }
    public void DrawNode()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_nodeBounds.center, _nodeBounds.size);
        if(children.Count > 0 )
        {
            foreach(OctreeNode child in children)
            {
                child.DrawNode();
            }
        }
    }
    public List<Bounds> GetChildBounds()
    {
        List<Bounds> childBounds = new List <Bounds>();
        Vector3 quarterOffsetX = new Vector3(_nodeBounds.size.x / 4, _nodeBounds.size.y / 4, _nodeBounds.size.z / 4);
        Vector3 childSize = new Vector3(_nodeBounds.size.x / 2, _nodeBounds.size.y / 2, _nodeBounds.size.z / 2);
        for (int i = 0; i < 8; i++)
        {
            string childPositionAsBinary = Convert.ToString(i, 2);
            while (childPositionAsBinary.Length < 3)
            {
                childPositionAsBinary = 0 + childPositionAsBinary;
            }
            Vector3 offset = new Vector3
            (
                childPositionAsBinary[0] == '0' ? quarterOffsetX.x : -quarterOffsetX.x,
                childPositionAsBinary[1] == '0' ? quarterOffsetX.y : -quarterOffsetX.y,
                childPositionAsBinary[2] == '0' ? quarterOffsetX.z : -quarterOffsetX.z
            );
            childBounds.Add(new Bounds(_nodeBounds.center + offset, childSize));
        }
        return childBounds;
    }
}

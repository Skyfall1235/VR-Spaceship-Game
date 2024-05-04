using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Chair Movement Node Data")]
[Serializable]
public class SO_NodeData : ScriptableObject
{
    //the node storage
    [SerializeField]
    private NodeData[] m_nodes;

    public NodeData[] Nodes
    {
        get
        {
            return m_nodes;
        }
        set 
        {
            m_nodes = value;
        }

    }

    

    public void BakeNodeData(List<RawGameobjectPositions> rawData)
    {
        //all ships should have an origin point, as the most common denominator for this origin in the pilot seat.
        NodeData[] newNodeList = new NodeData[rawData.Count];
        Debug.Log("Baking list data");
        for(int i = 0; i < newNodeList.Length; i++)
        {
            Debug.Log("Baking single datum");
            NodeData newNode = CreateNodesFromList(rawData[i]);
            newNodeList[i] = newNode;
        }

        //finally, save the data to the SO.
        m_nodes = newNodeList;
        Debug.Log($"Baking Finished");
    }

    public void ClearNodeData()
    {
        m_nodes = null;
    }

    NodeData CreateNodesFromList(RawGameobjectPositions rawData)
    {
        //get the main node
        Vector3 MainNodePosition = ConvertRawDataToPositionalData(rawData.Node);

        Debug.Log($"Baking Sub Nodes");
        //get the sub nodes in a loop
        List<SubNode> subNode = new();
        foreach (GameObject rawSubNode in rawData.subNode)
        {
            Vector3 positionalData = ConvertRawDataToPositionalData(rawSubNode);
            SubNode newSubNode = new SubNode(positionalData);
            subNode.Add(newSubNode);
        }

        //now, create constructor
        NodeData newNode = new NodeData(MainNodePosition, subNode);
        return newNode;
    }

    Vector3 ConvertRawDataToPositionalData(GameObject rawData)
    {
        //convert local position into a vector3
        Vector3 newNodePosition = rawData.transform.localPosition;
        return newNodePosition;
    }

    #region Data structures

    //this is class because seat positions can change on any given frame so we trade memory for fewer instructions to model the behavior
    [Serializable]
    public class NodeData
    {
        //baked before hand
        [SerializeField]
        private Vector3 m_nodePosition;
        [SerializeField]
        private List<SubNode> m_subNode;
        [SerializeField]
        public int currentSubNodeIndex;
        //unity event here? maybe somewhere else?? idk

        public NodeData(Vector3 nodePositionDatum, List<SubNode> subNodeData)
        {
            //this shouldnt change after being set
            this.m_nodePosition = nodePositionDatum;
            this.m_subNode = subNodeData;
        }
    }

    [Serializable]
    public class SubNode
    {
        [SerializeField]
        private Vector3 subNodePosition;

        public Vector3 CustomSeatPositionAdjustment;
        public float SeatRotationAdjustment;

        public SubNode(Vector3 position)
        {
            this.subNodePosition = position;
            this.CustomSeatPositionAdjustment = Vector3.zero;
            this.SeatRotationAdjustment = 0f;
        }
    }

    [Serializable]
    public struct RawGameobjectPositions
    {
        public GameObject Node;
        public List<GameObject> subNode;
    }

    #endregion
}

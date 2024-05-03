using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SO_NodeData;

// oh this one is gonna be funny
public class InterStationMovement : MonoBehaviour
{
    [Header("Overview and Data Management")]
    //struct of the nodes and sub nodes, as well as a settings struct inside it
    //seat adsjustment data can just be a vector3 from the local origin
    public List<SO_NodeData.RawGameobjectPositions> gameObjectPositions;

    //this is the scriptable object associated for whatever shi pthis is. this is where we will store and access our baked positions
    [SerializeField]
    private SO_NodeData m_savedNodeData;
    public SO_NodeData SavedNodeData
    { get { return m_savedNodeData; } }

    [SerializeField]
    private NodeData m_currentNode;
    public NodeData CurrentNode
    { get { return m_currentNode; } }
    public int CurrentNodeIndex
    {
        get
        {
            return Array.IndexOf(m_savedNodeData.Nodes, CurrentNode);
        }
    }

    //this is used internally and externally to confirm if we can act on the data within
    public bool UseNodeData
    {
        get
        {
            if (m_savedNodeData.Nodes != null)
            {
                return false;
            }
            return true;
        }
    }

    [Header("Movement Data")]

    [SerializeField]
    private GameObject PlayerGroup;//the chair and all of the player related things

    [SerializeField]
    private float seatMovementSpeed;
    //animation curse for a speed curve?



    //we will need some unity event to fire things off when we get to a node



    [SerializeField]
    CustomLogger logger;



    //space for future actual stuff

    #region Input Handling

    private void InitializeNode()
    {
        m_currentNode = m_savedNodeData.Nodes[0];
    }




    #endregion

    #region Control over Movement

    IEnumerator MovementSequence(NodeData currentNode, NodeData SelectedNode)
    {
        yield return null;
    }


    #endregion

    #region Movement between Nodes

    IEnumerator MoveToPrimaryNode(int PrimaryNodePosition)
    {
        yield return null;
    }

    IEnumerator MoveToSubNode(int subNodePosition)
    {
        yield return null;
    }

    #endregion

    #region Custom Editor Items

    public void ClearGameObjectList()
    {
        gameObjectPositions.Clear();
    }

    #endregion






}
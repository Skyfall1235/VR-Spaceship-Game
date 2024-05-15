using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    //the node we are already on
    //the current node holds the sub index we are on
    [SerializeField]
    private NodeData m_currentNode;
    public NodeData CurrentNode
    { get { return m_currentNode; } }
    public StationIndex CurrentNodeIndex
    {
        get
        {
            return new StationIndex(Array.IndexOf(SavedNodeData.Nodes, CurrentNode), CurrentNode.currentSubNodeIndex);
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
    private GameObject m_playerGroup;//the chair and all of the player related things

    [SerializeField]
    private float m_seatMaxMovementSpeed; // the maxspeed of the seat adjustment

    [SerializeField]
    private float m_chairSmoothTIme;
    //animation curse for a speed curve?

    Coroutine movementCoroutine;


    //we will need some unity event to fire things off when we get to a node
    public UnityEvent OnArrivalToNewPosition;



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

    IEnumerator MovementSequence(NodeData SelectedNode)
    { 
        //check to see if same primary node
        if(CheckIfPrimaryNode(SelectedNode))
        {
            //iff its the same node, we only need to move to the new sub index
            MoveToSubNode(SelectedNode.currentSubNodeIndex);
            
        }
        else if (CheckIfSubNode(SelectedNode))
        {
            //if its not in the same node, we move out, then move to the new node primary index, and then move to the new sub index
            IEnumerator CrossNodeMovement()
            {
                //mpve to the primary Nodes poisition
                //move to the new primary nodes position
                //move to the new subnode position
                yield return null;
            }
        }

        yield return null;
    }

    #region Boolean Logic Gates

    /// <summary>
    /// Checks if the provided `selectedNode` is the currently selected primary node.
    /// </summary>
    /// <param name="selectedNode">The node data to check.</param>
    /// <returns>True if the provided node is the currently selected primary node, otherwise false.</returns>
    private bool CheckIfPrimaryNode(NodeData selectedNode)
    {
        // Find the index of the selected node within the SavedNodeData.Nodes array.
        int indexOfSelectedNode = Array.IndexOf(SavedNodeData.Nodes, selectedNode);

        // Get the index of the current primary node.
        int indexOfCurrentNode = CurrentNodeIndex.primaryIndex;

        // Check if the indices are equal, indicating the nodes are the same.
        if (indexOfCurrentNode == indexOfSelectedNode)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the provided `selectedNode` is the currently selected sub-node.
    /// </summary>
    /// <param name="selectedNode">The node data to check.</param>
    /// <returns>True if the provided node is the currently selected sub-node, otherwise false.</returns>
    private bool CheckIfSubNode(NodeData selectedNode)
    {
        // Get the index of the current sub-node within the selected node's sub-nodes.
        int indexOfSelectedNode = selectedNode.currentSubNodeIndex;

        // Get the index of the current secondary node (assuming it represents the sub-node).
        int indexOfCurrentNode = CurrentNodeIndex.secondaryIndex;

        // Check if the indices are equal, indicating the nodes are the same.
        if (indexOfCurrentNode == indexOfSelectedNode)
        {
            return true;
        }

        return false;
    }

    #endregion

    #endregion

    #region Movement between Nodes

    /// <summary>
    /// Initiates a coroutine to move the player group to the specified primary node position.
    /// </summary>
    /// <param name="PrimaryNodePosition">The index of the primary node in the SavedNodeData.Nodes array.</param>
    void MoveToPrimaryNode(int PrimaryNodePosition)
    {
        // Get the position of the primary node from the SavedNodeData
        Vector3 targetPosition = SavedNodeData.Nodes[PrimaryNodePosition].m_nodePosition;

        // Start a coroutine to move towards the target position
        movementCoroutine = StartCoroutine(MoveToPosition(targetPosition));
    }

    /// <summary>
    /// Initiates a coroutine to move the player group to the specified sub-node position.
    /// </summary>
    /// <param name="subNodePosition">The index of the sub-node within the current node's sub-nodes array.</param>
    void MoveToSubNode(int subNodePosition)
    {
        // Get the position of the sub-node from the current node
        Vector3 targetPosition = CurrentNode.m_subNode[subNodePosition].subNodePosition;

        // Start a coroutine to move towards the target position
        movementCoroutine = StartCoroutine(MoveToPosition(targetPosition));
    }

    // Stores the player's current velocity (used for smooth movement)
    Vector3 playerVelocity = Vector3.zero;

    /// <summary>
    /// Coroutine that smoothly moves the player group towards a target position.
    /// </summary>
    /// <param name="newPosition">The target position to move towards.</param>
    /// <returns>An IEnumerator object for coroutine execution.</returns>
    IEnumerator MoveToPosition(Vector3 newPosition)
    {
        // Minimum distance threshold for considering arrival
        const float minimumDistance = 0.01f;

        // Loop until we reach the target position within the minimum distance
        while (Vector3.Distance(transform.position, newPosition) > minimumDistance)
        {
            // Calculate the distance to the target position
            float distanceToNewPosition = Vector3.Distance(transform.position, newPosition);

            // Smoothly move the player group towards the target
            m_playerGroup.transform.position = Vector3.SmoothDamp(m_playerGroup.transform.position, newPosition, ref playerVelocity, m_chairSmoothTIme);

            // Wait for the next fixed update before continuing
            yield return new WaitForFixedUpdate();
        }
        OnArrivalToNewPosition.Invoke();
    }

    #endregion

    #region Custom Editor Items

    public void ClearGameObjectList()
    {
        gameObjectPositions.Clear();
    }

    #endregion

    //i should comment why this is here
    [Serializable]
    public struct StationIndex
    {
        public int primaryIndex;
        public int secondaryIndex;

        public StationIndex(int primary, int secondary)
        {
            this.primaryIndex = primary;
            this.secondaryIndex = secondary;
        }
    }

    //NEEDED FOR SMOOTHDAMP
    Vector3 dampVelocity = Vector3.zero;

    public IEnumerator MoveToPosition(Vector3 TargetPosition, GameObject objectToMove, float speed)
    {
        const float minimumDistance = 0.01f;
        //calculate the speed 


        while(Vector3.Distance(objectToMove.transform.position, TargetPosition) > minimumDistance)
        {
            yield break;//TEMP
        }
    }



}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// oh this one is gonna be funny
public class InterStationMovement : MonoBehaviour
{
    //struct of the nodes and sub nodes, as well as a settings struct inside it
    //seat adsjustment data can just be a vector3 from the local origin
    public List<SO_NodeData.RawGameobjectPositions> gameObjectPositions;

    //this is the scriptable object associated for whatever shi pthis is. this is where we will store and access our baked positions
    [SerializeField]
    private SO_NodeData m_savedNodeData;
    public SO_NodeData SavedNodeData
    { get { return m_savedNodeData; } }

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

    [SerializeField]
    CustomLogger logger;

    

    //space for future actual stuff








}
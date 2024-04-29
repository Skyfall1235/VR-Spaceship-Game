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
    public List<SavedNodeData.RawGameobjectPositions> gameObjectPositions;

    //this is the scriptable object associated for whatever shi pthis is. this is where we will store and access our baked positions
    [SerializeField]
    private SavedNodeData m_savedNodeData;
    public SavedNodeData SavedNodeData
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

    

    //space for future actual stuff








}

[CreateAssetMenu(menuName = "New Chair Movement Node Data")]
[Serializable]
public class SavedNodeData : ScriptableObject
{
    //the node storage
    [SerializeField]
    private List<NodeData> m_nodes;
    public List<NodeData> Nodes
    { get { return m_nodes; } }

    //our current node that we are going to. probably need a bool to check if we are here, and if so to make this null?
    //idk ill think of it in class probably
    [SerializeField]
    private NodeData m_currentNode;
    public NodeData currentNode
    { get { return m_currentNode; } }

    public void BakeNodeData(List<RawGameobjectPositions> rawData)
    {
        //this converts the gameobject references to the more performant struct "nodeData"

    }


    public class NodeData//this is class because seat positions can change on any given frame so we trade memory for fewer instructions to model the behavior
    {
        public Vector3 NodePosition;
        public List<Vector3> SubNodePosition;
        public Vector3 CustomSeatAdjustment;
        //unity event here? maybe somewhere else?? idk
    }
    //this is only needed in the So and like the class
    [Serializable]
    public struct RawGameobjectPositions
    {
        public GameObject Node;
        public List<GameObject> subNode;
    }
}


//im doing this entire baking process because i want a reason to learn how to use the editor stuff.
// yes i am still going to go into thins with a performance mindset.
//i bet you will read this colin, i have no idea what im doing but the unity learn stuff is surpisingly good at showing me
//anyway, all this stuff will get its own script in due time, this is a prototype script.
//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

[CustomEditor(typeof(InterStationMovement))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InterStationMovement myScript = (InterStationMovement)target;
        if (GUILayout.Button("Build Object"))
        {
            if(myScript.SavedNodeData != null)
            {
                myScript.SavedNodeData.BakeNodeData(myScript.gameObjectPositions);
            }

        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static InterStationMovement;

public class InterStationMovement : MonoBehaviour
{
    //struct of the nodes and sub nodes, as well as a settings struct inside it
    //seat adsjustment data can just be a vector3 from the local origin
    public List<RawGameobjectPositions> gameObjectPositions;

    [SerializeField]
    private SavedNodeData m_savedNodeData;
    public SavedNodeData SavedNodeData
    { get { return m_savedNodeData; } }

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

    [Serializable]
    public struct RawGameobjectPositions
    { 
        public GameObject Node;
        public List<GameObject> subNode;
    }








}

[CreateAssetMenu(menuName = "New Chair Movement Node Data")]
[Serializable]
public class SavedNodeData : ScriptableObject
{
    [SerializeField]
    private List<NodeData> m_nodes;
    public List<NodeData> Nodes
    { get { return m_nodes; } }

    public void BakeNodeData(List<RawGameobjectPositions> rawData)
    {
        //this converts the gameobject references to the more performant struct "nodeData"

    }


    public class NodeData
    {
        public Vector3 NodePosition;
        public List<Vector3> SubNodePosition;
        public Vector3 CustomSeatAdjustment;
    }
}



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

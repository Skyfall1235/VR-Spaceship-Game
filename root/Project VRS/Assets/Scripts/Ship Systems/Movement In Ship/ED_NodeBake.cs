using UnityEditor;
using UnityEngine;

//im doing this entire baking process because i want a reason to learn how to use the editor stuff.
// yes i am still going to go into thins with a performance mindset.
//i bet you will read this colin, i have no idea what im doing but the unity learn stuff is surpisingly good at showing me
//anyway, all this stuff will get its own script in due time, this is a prototype script.
//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

[CustomEditor(typeof(InterStationMovement))]
public class ED_NodeBake : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InterStationMovement myScript = (InterStationMovement)target;
        if (GUILayout.Button("Build Object"))
        {
            if (myScript.SavedNodeData != null)
            {
                myScript.SavedNodeData.BakeNodeData(myScript.gameObjectPositions);
            }

        }
        if (GUILayout.Button("Clear Nodes"))
        {
            if (myScript.SavedNodeData != null)
            {
                myScript.SavedNodeData.ClearNodeData();
            }

        }
    }
}

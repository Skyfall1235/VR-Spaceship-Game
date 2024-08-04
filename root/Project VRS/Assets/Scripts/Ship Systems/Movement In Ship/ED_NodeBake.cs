using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
//im doing this entire baking process because i want a reason to learn how to use the editor stuff.
// yes i am still going to go into thins with a performance mindset.
[CustomEditor(typeof(InterStationMovement))]
public class ED_NodeBake : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        InterStationMovement myScript = (InterStationMovement)target;
        if (GUILayout.Button("Bake Nodes"))
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
        if (GUILayout.Button("Clear Game Object List"))
        {
            if (myScript.gameObjectPositions != null)
            {
                myScript.ClearGameObjectList();
            }
        }
    }
}
#endif

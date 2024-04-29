using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterStationMovement : MonoBehaviour
{
    //struct of the nodes and sub nodes, as well as a settings struct inside it
    //seat adsjustment data can just be a vector3 from the local origin

    struct NodeData
    {
        public Vector3 NodePosition;
        public List<Vector3> SubNodePosition;
        public Vector3 CustomSeatAdjustment;
    }

}

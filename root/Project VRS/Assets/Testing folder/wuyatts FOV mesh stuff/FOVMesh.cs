using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FOVMesh : MonoBehaviour
{
    void OnEnable()
    {
        var mesh = new Mesh
        {
            name = "Procedural Mesh"
        };
        mesh.vertices = new Vector3[] {
            Vector3.zero, Vector3.right, Vector3.up
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}


    

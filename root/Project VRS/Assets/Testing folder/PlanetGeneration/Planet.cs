using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int Resolution = 10;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool ShapeSettingsFoldout;
    [HideInInspector]
    public bool ColorSettingsFoldout;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;
    public Material material;

    ShapeGenerator generator;

    private void InitializeMeshes()
    {
        //generate the shape
        generator = new ShapeGenerator(shapeSettings);

        if(meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for(int i = 0; i < 6; i++) 
        {
            if (meshFilters[i] == null)
            {
                //create the object
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                //setup the meah filters array with objects have give them a material
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = material;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();

                //set the mesh initially
                meshFilters[i].sharedMesh = new Mesh();
            }
            //set the faces
            terrainFaces[i] = new TerrainFace(generator, meshFilters[i].sharedMesh, Resolution, directions[i]);
        }
    }

    

    public void OnColorSettingsUpdated()
    {
        InitializeMeshes();
        GenerateColors();
    }
    public void OnShapeSettingsUpdated()
    {
        InitializeMeshes();
        GenerateMesh();
    }

    public void GeneratePlanet()
    {
        InitializeMeshes();
        GenerateMesh();
        GenerateColors();
    }

    void GenerateColors()
    {
        foreach(MeshFilter filter in meshFilters)
        {
            filter.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.color;
        }
    }
    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }
}

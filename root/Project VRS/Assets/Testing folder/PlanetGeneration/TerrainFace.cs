using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator generator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator generator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.generator = generator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        this.axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        this.axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        //setup
        int savedResolution = resolution - 1;

        Vector3[] verts = new Vector3[resolution * resolution];
        int[] tris = new int[(savedResolution * savedResolution) * 6]; //((r01)^2))*6 is the formula for vert count

        int triIndex = 0;
        //creating of the mesh
        for (int y = 0; y < resolution; y++) 
        { 
            for (int x = 0; x < resolution; x++)
            {
                //setting position
                //define thr position of where we are
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / savedResolution;
                Vector3 pointOnCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnCube.normalized;
                verts[i] = generator.CalculatePointOnPlanet(pointOnUnitSphere);

                //tris time
                if(x != savedResolution && y != savedResolution)
                {
                    //first triangle drawing
                    tris[triIndex] = i;
                    tris[triIndex + 1] = i + resolution + 1;
                    tris[triIndex + 2] = i + resolution;

                    //second triangle drawing
                    tris[triIndex + 3] = i;
                    tris[triIndex + 4] = i + 1;
                    tris[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        //in case the resolutiuon changes, we dont want old data as it can cause the mesh to become distorted
        mesh.Clear();

        //set new verts, tris, and create new normals
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}

    

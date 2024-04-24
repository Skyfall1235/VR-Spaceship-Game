using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshCollider))]
public class FOVSensor : MonoBehaviour
{
    public UnityEvent<GameObject> FoundNewObject = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> LostObject = new UnityEvent<GameObject>();
    [SerializeField][HideInInspector] bool m_shouldDrawGizmos = true;
    [SerializeField][HideInInspector] float m_sightDistance = 20;
    [SerializeField][HideInInspector] float m_horizontalSightAngle = 20;
    [SerializeField][HideInInspector] float m_verticalSightAngle = 20;
    List<GameObject> m_objectsInSight = new List<GameObject>();

    //moved init out to awake and out of the create shape
    int[] tris;

    [SerializeField][HideInInspector] int m_xSize = 2;
    [SerializeField][HideInInspector] int m_ySize = 2;
    [SerializeField] LayerMask m_fovCollisionMask;
    Vector3[] preCollisionVertexPositions;

    #region Monobehavior methods

    private void OnTriggerEnter(Collider other)
    {
        if (!m_objectsInSight.Contains(other.gameObject))
        {
            FoundNewObject.Invoke(other.gameObject);
            m_objectsInSight.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_objectsInSight.Contains(other.gameObject))
        {
            LostObject.Invoke(other.gameObject);
            m_objectsInSight.Remove(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        CreateShape();
    }

    private void Awake()
    {
        CheckComponents();
        //NEW
        tris = new int[m_xSize * m_ySize * 6 + (6 * m_xSize) + (6 * m_ySize)];
    }

    private void OnDrawGizmos()
    {
        if (m_shouldDrawGizmos)
        {
            CheckComponents();
            //Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            //Gizmos.DrawMesh(m_fovMesh);
            Vector3 startingpoint = Vector3.zero;
            Vector3 farFOVCenter = startingpoint + Vector3.forward * m_sightDistance;
            Vector3 topRight = CalculateTopRightPoint(farFOVCenter);
            Vector3 topLeft = CalculateTopLeftPoint(farFOVCenter);
            Vector3 bottomRight = CalculateBottomRightPoint(farFOVCenter);
            Vector3 bottomLeft = CalculateBottomLeftPoint(farFOVCenter);
            Gizmos.DrawLine(startingpoint, topRight);
            Gizmos.DrawLine(startingpoint, topLeft);
            Gizmos.DrawLine(startingpoint, bottomLeft);
            Gizmos.DrawLine(startingpoint, bottomRight);
            Gizmos.DrawLine(topLeft, bottomLeft);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.color = Color.red;
            if (m_fovMesh.vertices != null)
            {
                CreateShape();
                foreach (Vector3 vertex in m_fovMesh.vertices)
                {
                    Gizmos.DrawSphere(vertex, .1f);
                }
            }
        }
    }

    #endregion

    Mesh m_fovMesh;
    MeshFilter m_filter;
    MeshCollider m_collider;

    public void CheckComponents()
    {
        if (m_filter == null)
        {
            m_filter = transform.GetComponent<MeshFilter>();
        }
        if (m_collider == null)
        {
            m_collider = transform.GetComponent<MeshCollider>();
            m_collider.convex = true;
            m_collider.isTrigger = true;
        }
        if(m_fovMesh == null)
        {
            m_fovMesh = new Mesh();
            m_filter.mesh = m_fovMesh;
            m_collider.sharedMesh = m_fovMesh;
            m_fovMesh.MarkDynamic();
        }
    }

    public void CreateShape()
    {
        Vector3[] verts;
        
        if (preCollisionVertexPositions == null || preCollisionVertexPositions.Length <= 0) 
        {
            PrecalculateVertexPositions();
        }
        verts = UpdateVerticies();
        DrawTriangles(ref tris);
        //NEW
        //m_fovMesh.Clear();
        m_fovMesh.vertices = verts;
        m_fovMesh.triangles = tris;
        m_fovMesh.RecalculateNormals();
    }

    void DrawTriangles(ref int[] tris)
    {
        int currentVert = 1;
        int currentTri = 0;
        //Create the front wall
        //NEW
        //unraveled the most complex loop for memory optimisation
        for (int x = 0; x < m_xSize; x++)
        {
            for (int y = 0; y < m_ySize; y++)
            {
                tris[currentTri + 0] = currentVert;
                tris[currentTri + 1] = currentVert + m_ySize + 1;
                tris[currentTri + 2] = currentVert + 1;
                tris[currentTri + 3] = currentVert + 1;
                tris[currentTri + 4] = currentVert + m_ySize + 1;
                tris[currentTri + 5] = currentVert + m_ySize + 2;
                currentVert++;
                currentTri += 6;
            }
            currentVert++;
        }

        //Create the top wall
        for (int x = 0, vertexNum = m_ySize + 1; x < m_xSize; x++)
        {
            tris[currentTri] = 0;
            tris[currentTri + 1] = vertexNum;
            tris[currentTri + 2] = vertexNum + m_ySize + 1;
            currentTri += 3;
            vertexNum += m_ySize + 1;
        }
        //Create the bottom wall
        for (int x = 0, vertexNum = 1; x < m_xSize; x++)
        {

            tris[currentTri + 0] = vertexNum + m_ySize + 1;
            tris[currentTri + 1] = vertexNum;
            tris[currentTri + 2] = 0;
            currentTri += 3;
            vertexNum += m_ySize + 1;
        }
        //Create the left wall
        for (int y = 0, vertexNum = 1; y < m_ySize; y++)
        {
            tris[currentTri] = 0;
            tris[currentTri + 1] = vertexNum;
            tris[currentTri + 2] = vertexNum + 1;
            currentTri += 3;
            vertexNum += 1;
        }
        //create the right wall
        for (int y = 0, vertexNum = ((m_xSize + 1) * (m_ySize + 1)) - m_ySize; y < m_ySize; y++)
        {
            tris[currentTri + 2] = 0;
            tris[currentTri + 1] = vertexNum;
            tris[currentTri + 0] = vertexNum + 1;
            currentTri += 3;
            vertexNum += 1;
        }
    }

    void PrecalculateVertexPositions()
    {
        Vector3 farFOVCenter = Vector3.forward * m_sightDistance;

        Vector3 topRight = CalculateTopRightPoint(farFOVCenter);
        Vector3 topLeft = CalculateTopLeftPoint(farFOVCenter);
        Vector3 bottomRight = CalculateBottomRightPoint(farFOVCenter);
        Vector3 bottomLeft = CalculateBottomLeftPoint(farFOVCenter);

        float distanceOnX = Vector3.Distance(topLeft, topRight) / m_xSize;
        float distanceOnY = Vector3.Distance(bottomRight, topRight) / (m_ySize);
        preCollisionVertexPositions = new Vector3[((m_xSize + 1) * (m_ySize + 1)) + 1];

        for (int x = 0, i = 1; x <= m_xSize; x++)
        {
            for (int y = 0; y <= m_ySize; y++)
            {
                preCollisionVertexPositions[i] = new Vector3(bottomLeft.x + (distanceOnX * x), bottomLeft.y + (distanceOnY * y), farFOVCenter.z);
                i++;
            }
        }
    }

    public Vector3[] UpdateVerticies()
    {
        Vector3[] currentVerticies = new Vector3[preCollisionVertexPositions.Length];
        Array.Copy(preCollisionVertexPositions, currentVerticies, preCollisionVertexPositions.Length);
        for (int i = 1; i < currentVerticies.Length; i++)
        {
            if (Physics.Linecast(transform.rotation * preCollisionVertexPositions[0] + transform.position, transform.rotation * preCollisionVertexPositions[i] + transform.position, out RaycastHit hitInfo, m_fovCollisionMask))
            {
                currentVerticies[i] = Quaternion.Inverse(transform.rotation) * (hitInfo.point - transform.position);
            }
        }
        return currentVerticies;
    }

    #region Calculating Corners

    //AAAAAAAAAAAAAAAAAH
    Vector3 CalculateTopRightPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + (CalculateDistanceOfSightCone() * Vector3.right) + (CalculateDistanceOfSightCone() * Vector3.up);
    }
    Vector3 CalculateTopLeftPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + (CalculateDistanceOfSightCone() * Vector3.left) + (CalculateDistanceOfSightCone() * Vector3.up);
    }
    Vector3 CalculateBottomRightPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + (CalculateDistanceOfSightCone() * Vector3.right) + (CalculateDistanceOfSightCone() * Vector3.down);
    }
    Vector3 CalculateBottomLeftPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + (CalculateDistanceOfSightCone() * Vector3.left) + (CalculateDistanceOfSightCone() * Vector3.down);
    }

    float CalculateDistanceOfSightCone()
    {
        return (Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance);
    }

    #endregion

}
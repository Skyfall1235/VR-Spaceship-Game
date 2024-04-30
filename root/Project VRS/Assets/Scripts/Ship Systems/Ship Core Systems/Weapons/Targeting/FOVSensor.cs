using System;
using System.Collections.Generic;
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

    [SerializeField][HideInInspector] int m_xSize = 2;
    [SerializeField][HideInInspector] int m_ySize = 2;
    [SerializeField] LayerMask m_fovCollisionMask;
    Vector3[] preCollisionVertexPositions;
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
    }
    Mesh m_fovMesh;
    public MeshFilter m_Filter { get; private set; }
    public MeshCollider m_Collider {  get; private set; }
    public void CheckComponents()
    {
        if (m_Filter == null)
        {
            m_Filter = transform.GetComponent<MeshFilter>();
        }
        if (m_Collider == null)
        {
            m_Collider = transform.GetComponent<MeshCollider>();
            m_Collider.convex = true;
            m_Collider.isTrigger = true;
        }
        if(m_fovMesh == null)
        {
            m_fovMesh = new Mesh();
            m_Filter.mesh = m_fovMesh;
            m_Collider.sharedMesh = m_fovMesh;
            m_fovMesh.MarkDynamic();
        }
    }
    public void CreateShape()
    {
        Vector3[] verts;
        int[] tris = new int[m_xSize * m_ySize * 6 + (6 * m_xSize) + (6 * m_ySize)];
        if (preCollisionVertexPositions == null || preCollisionVertexPositions.Length <= 0) 
        {
            PrecalculateVertexPositions();
        }
        verts = UpdateVerticies();
        DrawTriangles(ref tris);
        m_fovMesh.vertices = verts;
        m_fovMesh.triangles = tris;
        m_fovMesh.RecalculateNormals();
    }
    void DrawTriangles(ref int[] tris)
    {
        int currentVert = 1;
        int currentTri = 0;
        //Create the front wall
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
            tris[currentTri + 0] = 0;
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
            tris[currentTri + 0] = 0;
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
    public void PrecalculateVertexPositions()
    {
        Vector3 startingpoint = Vector3.zero;
        Vector3 farFOVCenter = startingpoint + Vector3.forward * m_sightDistance;


        Vector3 topRight = CalculateTopRightPoint(farFOVCenter);
        Vector3 topLeft = CalculateTopLeftPoint(farFOVCenter);
        Vector3 bottomRight = CalculateBottomRightPoint(farFOVCenter);
        Vector3 bottomLeft = CalculateBottomLeftPoint(farFOVCenter);

        float distanceOnX = Vector3.Distance(topLeft, topRight) / m_xSize;
        float distanceOnY = Vector3.Distance(bottomRight, topRight) / (m_ySize);
        preCollisionVertexPositions = new Vector3[((m_xSize + 1) * (m_ySize + 1)) + 1];
        preCollisionVertexPositions[0] = startingpoint;
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
    private void OnDrawGizmos()
    {
        if (m_shouldDrawGizmos)
        {
            CheckComponents();
            Gizmos.matrix = transform.localToWorldMatrix;
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
    Vector3 CalculateTopRightPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.right) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.up);
    }
    Vector3 CalculateTopLeftPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.left) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.up);
    }
    Vector3 CalculateBottomRightPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.right) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.down);
    }
    Vector3 CalculateBottomLeftPoint(Vector3 farFOVCenter)
    {
        return farFOVCenter + ((Mathf.Sin(m_horizontalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.left) + ((Mathf.Sin(m_verticalSightAngle / 2 * Mathf.Deg2Rad) * m_sightDistance) * Vector3.down);
    }
}
[CustomEditor(typeof(FOVSensor))]
public class AISensorInspector : Editor
{
    SerializedProperty m_shouldDrawGizmos;
    SerializedProperty m_sightDistance;
    SerializedProperty m_horizontalSightAngle;
    SerializedProperty m_verticalSightAngle;
    SerializedProperty m_xSize;
    SerializedProperty m_ySize;
    private void OnEnable()
    {
        m_shouldDrawGizmos = serializedObject.FindProperty(nameof(m_shouldDrawGizmos));
        m_sightDistance = serializedObject.FindProperty(nameof(m_sightDistance));
        m_horizontalSightAngle = serializedObject.FindProperty(nameof(m_horizontalSightAngle));
        m_verticalSightAngle = serializedObject.FindProperty(nameof(m_verticalSightAngle));
        m_xSize = serializedObject.FindProperty(nameof(m_xSize));
        m_ySize = serializedObject.FindProperty(nameof(m_ySize));
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        FOVSensor scriptToUpdate = target as FOVSensor;
        EditorGUI.BeginChangeCheck();
        bool newshouldDrawGizmos = EditorGUILayout.Toggle("Should Draw Gizmos?", m_shouldDrawGizmos.boolValue);
        float newSightDistance = EditorGUILayout.FloatField("Sight Distance", m_sightDistance.floatValue);
        float newHorizontalSightAngle = EditorGUILayout.Slider("Horizontal Angle", m_horizontalSightAngle.floatValue, 0.01f, 180);
        float newVerticalSightAngle = EditorGUILayout.Slider("Horizontal Angle", m_verticalSightAngle.floatValue, 0.01f, 180);
        int newXSize = EditorGUILayout.IntField("X Vertex Count", m_xSize.intValue);
        int newYSize = EditorGUILayout.IntField("Y Vertex Count", m_ySize.intValue);
        if (EditorGUI.EndChangeCheck())
        {
            m_shouldDrawGizmos.boolValue = newshouldDrawGizmos;
            m_sightDistance.floatValue = Mathf.Clamp(newSightDistance, 0.01f, float.MaxValue);
            m_horizontalSightAngle.floatValue = newHorizontalSightAngle;
            m_verticalSightAngle.floatValue = newVerticalSightAngle;
            m_xSize.intValue = Mathf.Clamp(newXSize, 1, int.MaxValue);
            m_ySize.intValue = Mathf.Clamp(newYSize, 1, int.MaxValue);
        }
        scriptToUpdate.CheckComponents();
        scriptToUpdate.PrecalculateVertexPositions();
        scriptToUpdate.CreateShape();
        serializedObject.ApplyModifiedProperties();
    }
}


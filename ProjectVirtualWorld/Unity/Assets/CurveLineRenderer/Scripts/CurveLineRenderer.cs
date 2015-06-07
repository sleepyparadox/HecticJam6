using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CurveLineRenderer : MonoBehaviour
{
    private const float epsilon = 0.0001f;

    /**
     * Type of the spline. Defines how the spline will be drawn near corners
     */
    public enum LineType
    {
        Default,    // Corners are not smoothed. Default value
        Rounded,	// Corners are smoothed along the arc of given radius
        Splitted,   // Each segment is independent from other segments
    }

    /**
     * Type of the spline
     */
    public LineType type = LineType.Default;

    /**
     * Width of the line
     */
    public float width = 1.0f;

    /**
     * A smoothing radius
     */
    public float radius = 1.0f;

    /**
     * Specifies a offset of smoothing angle. Defines count of the fragments of smoothing corners
     */
    public float roundedAngle = 15.0f;

    /**
     * Normal of the spline
     */
    public Vector3 normal = Vector3.up;

    /**
     * If enabled, the reverse side of the spline will be drawn
     */
    public bool reverseSideEnabled = true;

    /**
     * List of the spline vertices
     */
    public List<Vector3> vertices = new List<Vector3>() 
    {
	    new Vector3(0, 0, 0), new Vector3(0, 0, 1)
    };

    private Mesh mesh;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Curve Line Mesh";
            meshFilter.mesh = mesh;
        }

        Invalidate();
    }

    /**
     * Set new list of vertices. Calls Invalidate()
     */
    public void SetVertices(List<Vector3> vertexList)
    {
        vertices.AddRange(vertexList);
        Invalidate();
    }

    /**
     * Add new vertex to exists list of vertices. Calls Invalidate()
     */
    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);
        Invalidate();
    }

    /**
     * Clear the list of vertices. Calls Invalidate()
     */
    public void ClearVertices()
    {
        vertices.Clear();
        Invalidate();
    }

    /**
     * Invalidate the whole spline. Apply changes of fields and call redraw of spline
     */
    public void Invalidate()
    {
        if (mesh == null)
            return;

        mesh = Rebuild(mesh);
    }

    /**
     * Build curve line mesh with defined parameters 
     */
    public Mesh Rebuild(Mesh mesh)
    {
        if (mesh == null)
            return mesh;

        mesh.Clear();

        if (vertices == null || vertices.Count < 2 || width <= 0f || normal == Vector3.zero
                || (type == LineType.Rounded && (roundedAngle <= 0f || radius <= 0f)))
            return mesh;

        // Normalize the normal vector
        Vector3 n = normal.normalized;

        // Define the list of vertices depending on the type
        List<Vector3> vertexList = (type == LineType.Rounded) ?
            getRoundedVertices() : getDefaultVertices();

        // First vertex
        List<Vector3> meshVertices = new List<Vector3>();
        Vector3 direction = vertexList[1] - vertexList[0];
        direction.Normalize();

        Vector3 qdir = Vector3.Cross(direction, n).normalized;

        meshVertices.Add(vertexList[0] - qdir * width * 0.5f);
        meshVertices.Add(vertexList[0] + qdir * width * 0.5f);

        // Inner vertices
        Vector3 nextNormal = n;

        for (int i = 1; i < vertexList.Count - 1; ++i)
        {
            Vector3 nextDirection = vertexList[i + 1] - vertexList[i];
            nextDirection.Normalize();

            Vector3 nextQdir = Vector3.Cross(nextDirection, nextNormal).normalized;
            if (nextQdir == Vector3.zero || (nextQdir + qdir).magnitude < epsilon)
            {
                nextQdir = qdir;
                nextNormal *= -1;
            }

            if (type == LineType.Splitted)
            {
                // Add both vertices
                meshVertices.Add(vertexList[i] - qdir * width * 0.5f);
                meshVertices.Add(vertexList[i] + qdir * width * 0.5f);

                meshVertices.Add(vertexList[i] - nextQdir * width * 0.5f);
                meshVertices.Add(vertexList[i] + nextQdir * width * 0.5f);
            }
            else
            {
                Vector3 pdir = (qdir + nextQdir).normalized;
                float w = width / Mathf.Sin(Vector3.Angle(direction, pdir) * Mathf.PI / 180.0f);

                meshVertices.Add(vertexList[i] - pdir * w * 0.5f);
                meshVertices.Add(vertexList[i] + pdir * w * 0.5f);
            }
                
            direction = nextDirection;
            qdir = nextQdir;
        }

        // Last vertex
        meshVertices.Add(vertexList[vertexList.Count - 1] - qdir * width * 0.5f);
        meshVertices.Add(vertexList[vertexList.Count - 1] + qdir * width * 0.5f);

        // Build mesh
        BuildMesh(mesh, meshVertices, vertexList.Count - 1);

        return mesh;
    }

    /**
     * Build mesh by list of vertices
     */
    private void BuildMesh(Mesh mesh, List<Vector3> meshVertices, int segmentCount)
    {
        int trianglesCount = segmentCount * 2 * 3 * (reverseSideEnabled ? 2 : 1);

        List<int> triangles = new List<int>();
        for (int i = 0; i < trianglesCount; ++i)
            triangles.Add(i);

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < segmentCount; ++i)
        {
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(1, 0);
            Vector2 uv2 = new Vector2(0, 1);
            Vector2 uv3 = new Vector2(1, 1);

            // Top side of the spline
            if (type == LineType.Splitted)
            {
                vertices.Add(meshVertices[i * 4]);
                vertices.Add(meshVertices[i * 4 + 1]);
                vertices.Add(meshVertices[i * 4 + 2]);
                vertices.Add(meshVertices[i * 4 + 2]);
                vertices.Add(meshVertices[i * 4 + 1]);
                vertices.Add(meshVertices[i * 4 + 3]);
            }
            else
            {
                vertices.Add(meshVertices[i * 2]);
                vertices.Add(meshVertices[i * 2 + 1]);
                vertices.Add(meshVertices[i * 2 + 2]);
                vertices.Add(meshVertices[i * 2 + 2]);
                vertices.Add(meshVertices[i * 2 + 1]);
                vertices.Add(meshVertices[i * 2 + 3]);
            }

            uvs.Add(uv0);
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv2);
            uvs.Add(uv1);
            uvs.Add(uv3);

            // Back side of the spline
            if (reverseSideEnabled)
            {
                if (type == LineType.Splitted)
                {
                    vertices.Add(meshVertices[i * 4 + 1]);
                    vertices.Add(meshVertices[i * 4]);
                    vertices.Add(meshVertices[i * 4 + 2]);
                    vertices.Add(meshVertices[i * 4 + 1]);
                    vertices.Add(meshVertices[i * 4 + 2]);
                    vertices.Add(meshVertices[i * 4 + 3]);
                }
                else
                {
                    vertices.Add(meshVertices[i * 2 + 1]);
                    vertices.Add(meshVertices[i * 2]);
                    vertices.Add(meshVertices[i * 2 + 2]);
                    vertices.Add(meshVertices[i * 2 + 1]);
                    vertices.Add(meshVertices[i * 2 + 2]);
                    vertices.Add(meshVertices[i * 2 + 3]);
                }

                uvs.Add(uv1);
                uvs.Add(uv0);
                uvs.Add(uv2);
                uvs.Add(uv1);
                uvs.Add(uv2);
                uvs.Add(uv3);
            }
        }

        // Set parameters to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    /**
     * Get the list of vertices for Default spline type
     */
    private List<Vector3> getDefaultVertices()
    {
        return vertices;
    }

    /**
     * Get the list of vertices for Rounded spline type
     */
    private List<Vector3> getRoundedVertices()
    {
        Vector3 n = normal.normalized;
        Vector3 center;
        float Radius = (radius < width / 2) ? width / 2 : radius;

        List<Vector3> roundedVertices = new List<Vector3>();
        Vector3 dir = (vertices[1] - vertices[0]);
        dir.Normalize();

        Vector3 qdir = Vector3.Cross(dir, n).normalized;
        roundedVertices.Add(vertices[0]);
        for (int i = 1; i < vertices.Count - 1; ++i)
        {
            Vector3 nextDir = vertices[i + 1] - vertices[i];
            Vector3 nextQdir = (Mathf.Abs(Vector3.Angle(Vector3.Cross(dir, nextDir), n) - 90f) < 1f) ? 
                qdir : Vector3.Cross(nextDir, n).normalized;
            nextDir.Normalize();
           
            if (Vector3.Angle(nextDir, dir) < 1.0f)
            {
                // Next direction is equal to current direction
                roundedVertices.Add(vertices[i]);
            }
            else
            {
                float maxR = Mathf.Min((vertices[i] - vertices[i - 1]).magnitude,
                                       (vertices[i + 1] - vertices[i]).magnitude) * 0.5f;

                float r = Radius;
                Vector3 pdir = ((-dir).normalized + nextDir.normalized).normalized;
                if (pdir.magnitude < epsilon || (qdir - nextQdir).magnitude < epsilon)
                {
                    // Normal is in surface of rotation
                    Vector3 normal1 = -Vector3.Cross(dir, qdir);
                    Vector3 normal2 = -Vector3.Cross(nextDir, nextQdir);
                    pdir = (normal1 + normal2).normalized;
                }

                float angle = Vector3.Angle(dir, pdir);
                float rwidth = r / Mathf.Sin(angle * Mathf.PI / 180.0f);
                float rlength = Mathf.Sqrt(rwidth * rwidth - r * r);

                if (rlength > maxR)
                {
                    rlength = maxR;
                    rwidth = rlength / Mathf.Cos(angle * Mathf.PI / 180.0f);
                }

                Vector3 vertex1 = vertices[i] + pdir * rwidth;
                Vector3 vertex2 = vertices[i] - pdir * rwidth;

                Vector3 rightPoint = vertices[i] + nextDir * rlength;
                Vector3 leftPoint = vertices[i] + dir * (-rlength);

                center = (Mathf.Abs(Vector3.Dot(leftPoint - vertex1, dir)) < epsilon) ?
                    vertex1 : vertex2;

                Vector3 rotateAxis = Vector3.Cross(dir, nextDir);
                Vector3 rotateVector = leftPoint - center;

                angle = Vector3.Angle(leftPoint - center, rightPoint - center);
                int segmentCount = (int)(angle / roundedAngle + 0.5f);
                float angleDelta = angle / segmentCount;

                Quaternion q = Quaternion.AngleAxis(angleDelta, rotateAxis);

                roundedVertices.Add(leftPoint);
                for (int j = 0; j < segmentCount - 1; ++j)
                {
                    rotateVector = q * rotateVector;
                    roundedVertices.Add(center + rotateVector);
                }
                roundedVertices.Add(rightPoint);
            }

            dir = nextDir;
            qdir = nextQdir;
        }
        roundedVertices.Add(vertices[vertices.Count - 1]);

        return roundedVertices;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PathMeshRenderer : MonoBehaviour {
    [Header("Path display settings")]
    public Material pathMaterial;
    public float pathThickness, pathWidth;

    // Path rendering
    private GameObject pathMesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    // Use this for initialization
    void Start() {
        pathMesh = new GameObject("Path mesh");
    }

    // Methods below are from the Road Mesh Creator example.

    /// <summary>
    /// Create a mesh from current stored path. 
    /// </summary>
    /// <returns>the corresponding mesh object</returns>
    private Mesh CreateMeshFromCurve(List<PathDrawer.Coords> curve) {
        Vector3[] verts = new Vector3[(curve.Count - 1) * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (curve.Count - 1);
        int[] pathTriangles = new int[numTris * 3];
        int[] underPathTriangles = new int[numTris * 3];
        int[] sideOfPathTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;
        int[] triangleMap = { 0, 1, 8, 1, 9, 8 };
        int[] sidesTriangleMap = { 4, 14, 6, 12, 14, 4, 5, 7, 15, 13, 5, 15 };

        // Compute curve length in order to interpolate UV coordinates
        float curveLength = .0f;
        for(int i = 0; i < curve.Count - 1; i++) {
            curveLength += Mathf.Abs((curve[i + 1].pos - curve[i].pos).magnitude);
        }

        float dst = .0f;
        for (int i = 0; i < curve.Count - 1; i++) {
            Vector3 vertex = curve[i].pos;
            Vector3 nextVertex = curve[i + 1].pos;
            Vector3 normal = curve[i].rot * Vector3.up;

            Vector3 tangent = Vector3.Normalize(nextVertex - vertex);
            Vector3 localUp = normal;
            Vector3 localRight = Vector3.Cross(tangent, normal);

            Vector3 leftSide = vertex - localRight * Mathf.Abs(pathWidth);
            Vector3 rightSide = vertex + localRight * Mathf.Abs(pathWidth);

            // Top of path vertices
            verts[vertIndex + 0] = leftSide;
            verts[vertIndex + 1] = rightSide;
            // Bottom of path vertices
            verts[vertIndex + 2] = leftSide - localUp * pathThickness;
            verts[vertIndex + 3] = rightSide - localUp * pathThickness;
            // Duplicate vertices to get flat shading for sides of path
            verts[vertIndex + 4] = verts[vertIndex + 0];
            verts[vertIndex + 5] = verts[vertIndex + 1];
            verts[vertIndex + 6] = verts[vertIndex + 2];
            verts[vertIndex + 7] = verts[vertIndex + 3];

            // uv coordinates on y axis to path normalized length 
            uvs[vertIndex + 0] = new Vector2(0, dst / curveLength);
            uvs[vertIndex + 1] = new Vector2(1, dst / curveLength);

            // Top of curve normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of curve normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of curve normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Index triangles on vertices 
            // Call procedure for each vertex except the last (reason for the condition)
            if (i < curve.Count - 2) {
                for (int j = 0; j < triangleMap.Length; j++) {
                    pathTriangles[triIndex + j] = vertIndex + triangleMap[j];
                    underPathTriangles[triIndex + j] = vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++) {
                    sideOfPathTriangles[triIndex * 2 + j] = vertIndex + sidesTriangleMap[j];
                }
            }
            vertIndex += 8;
            triIndex += 6;
            // Store the current discretized distance on curve
            dst += (nextVertex - vertex).magnitude;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.subMeshCount = 3;
        mesh.SetTriangles(pathTriangles, 0);
        mesh.SetTriangles(underPathTriangles, 1);
        mesh.SetTriangles(sideOfPathTriangles, 2);
        mesh.RecalculateBounds();
        
        return mesh;
    }

    // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
    void AssignMeshComponents() {
        Transform meshHolder = pathMesh.transform;

        meshHolder.transform.position = Vector3.zero;

        meshHolder.transform.rotation = Quaternion.identity;

        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.gameObject.GetComponent<MeshFilter>()) {
            meshHolder.gameObject.AddComponent<MeshFilter>();
        }
        if (!meshHolder.GetComponent<MeshRenderer>()) {
            meshHolder.gameObject.AddComponent<MeshRenderer>();
        }

        meshRenderer = meshHolder.GetComponent<MeshRenderer>();
        meshFilter = meshHolder.GetComponent<MeshFilter>();
    }

    /// <summary>
    /// Assign a given public material to the 3D curve
    /// </summary>
    void AssignMaterials() {
        if (pathMaterial != null) {
            // Assign given material to the 3 submeshes
            meshRenderer.sharedMaterials = new Material[] { pathMaterial, pathMaterial, pathMaterial };
        }
    }

    /// <summary>
    /// Call necessary procedures in order to generate the curve mesh on the screen
    /// </summary>
    /// <param name="curve"></param>
    public void RenderCurve(List<PathDrawer.Coords> curve) {
        AssignMeshComponents();
        AssignMaterials();
        meshFilter.mesh = CreateMeshFromCurve(curve);
    }
}
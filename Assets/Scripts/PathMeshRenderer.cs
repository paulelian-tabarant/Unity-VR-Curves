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
	void Start () {
        pathMesh = new GameObject("Path mesh");
	}

    // Methods below are from the Road Mesh Creator example.
/*
    /// <summary>
    /// Create a mesh from current stored path. 
    /// </summary>
    /// <returns>the corresponding mesh object</returns>
    private Mesh CreateMeshFromBezierPath(BezierPath bPath) {
        VertexPath vPath = new VertexPath(bPath);
        Vector3[] verts = new Vector3[vPath.NumVertices * 8];
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];

        int numTris = 2 * (vPath.NumVertices - 1);
        int[] pathTriangles = new int[numTris * 3];
        int[] underPathTriangles = new int[numTris * 3];
        int[] sideOfPathTriangles = new int[numTris * 2 * 3];

        int vertIndex = 0;
        int triIndex = 0;
        int[] triangleMap = { 0, 8, 1, 1, 8, 9 };
using PathCreation;
        int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

        for (int i = 0; i < vPath.NumVertices; i++) {
            Vector3 localUp = Vector3.Cross(vPath.tangents[i], vPath.normals[i]);
            Vector3 localRight = Vector3.Cross(localUp, vPath.tangents[i]);

            Vector3 leftSide = vPath.vertices[i] - localRight * Mathf.Abs(pathWidth);
            Vector3 rightSide = vPath.vertices[i] + localRight * Mathf.Abs(pathWidth);

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

            // uv coordinates on y axis to path normalized length (=times field)
            uvs[vertIndex + 0] = new Vector2(0, vPath.times[i]);
            uvs[vertIndex + 1] = new Vector2(1, vPath.times[i]);

            // Top of road normals
            normals[vertIndex + 0] = localUp;
            normals[vertIndex + 1] = localUp;
            // Bottom of road normals
            normals[vertIndex + 2] = -localUp;
            normals[vertIndex + 3] = -localUp;
            // Sides of road normals
            normals[vertIndex + 4] = -localRight;
            normals[vertIndex + 5] = localRight;
            normals[vertIndex + 6] = -localRight;
            normals[vertIndex + 7] = localRight;

            // Index triangles
            // condition for the path extremity
            if (i < vPath.NumVertices - 1) { 
                for (int j = 0; j < triangleMap.Length; j++) {
                    pathTriangles[triIndex + j] = vertIndex + triangleMap[j];
using PathCreation;
                    underPathTriangles[triIndex + j] = vertIndex + triangleMap[triangleMap.Length - 1 - j] + 2;
                }
                for (int j = 0; j < sidesTriangleMap.Length; j++) {
                    sideOfPathTriangles[triIndex * 2 + j] = vertIndex + sidesTriangleMap[j];
                }
            }
        //pathMeshRenderer = GetComponent<PathMeshRenderer>();
            vertIndex += 8;
            triIndex += 6;
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
            // Render the line on screen

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

            // Render the line on screen
        meshRenderer = meshHolder.GetComponent<MeshRenderer>();
        meshFilter = meshHolder.GetComponent<MeshFilter>();
    }

    void AssignMaterials() {
        if (pathMaterial != null) {
            meshRenderer.sharedMaterials = new Material[] { pathMaterial };
        }
    }

    public void RenderBezierPath(BezierPath path) {
        AssignMeshComponents();
        AssignMaterials();
        meshFilter.mesh = CreateMeshFromBezierPath(path);
    }*/
}

using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeGenerator : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateCubeMesh();
    }

    Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();

        // 立方体的8个顶点
        Vector3[] vertices = {
            new Vector3(0, 0, 0), // 0
            new Vector3(1, 0, 0), // 1
            new Vector3(1, 1, 0), // 2
            new Vector3(0, 1, 0), // 3
            new Vector3(0, 1, 1), // 4
            new Vector3(1, 1, 1), // 5
            new Vector3(1, 0, 1), // 6
            new Vector3(0, 0, 1)  // 7
        };

        // 每个面的三角形顶点索引
        int[] triangles = {
            // 前
            0, 2, 1, 0, 3, 2,
            // 右
            1, 2, 5, 1, 5, 6,
            // 背
            6, 5, 4, 6, 4, 7,
            // 左
            7, 4, 3, 7, 3, 0,
            // 顶
            3, 4, 5, 3, 5, 2,
            // 底
            1, 6, 7, 1, 7, 0
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 重新计算法线，用于光照等

        return mesh;
    }
}

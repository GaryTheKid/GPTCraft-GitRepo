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

        // �������8������
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

        // ÿ����������ζ�������
        int[] triangles = {
            // ǰ
            0, 2, 1, 0, 3, 2,
            // ��
            1, 2, 5, 1, 5, 6,
            // ��
            6, 5, 4, 6, 4, 7,
            // ��
            7, 4, 3, 7, 3, 0,
            // ��
            3, 4, 5, 3, 5, 2,
            // ��
            1, 6, 7, 1, 7, 0
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // ���¼��㷨�ߣ����ڹ��յ�

        return mesh;
    }
}

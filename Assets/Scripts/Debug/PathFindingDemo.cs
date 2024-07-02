using UnityEngine;

public class PathFindingDemo : MonoBehaviour
{
    public Material m1;
    public Material m2;
    public GameObject cubePrefab; // 确保你有一个Cube的Prefab

    // Perlin Noise参数
    public float scale = 0.6f; // 缩放系数，调整以改变噪声的“频率”
    public float threshold = 0.5f; // 生成Cube的阈值

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < 17; x++) // X轴上扩展到16
        {
            for (int y = 0; y < 6; y++) // Y轴高度设置为6
            {
                for (int z = 0; z < 17; z++) // Z轴上扩展到16
                {
                    // 使用Perlin Noise决定是否创建Cube
                    float noise = Mathf.PerlinNoise((x + transform.position.x) * scale, (z + transform.position.z) * scale);

                    // 考虑到Y轴的变化对噪声值没有影响，这里不需要修改噪声计算逻辑
                    if (y + transform.position.y > noise * 10f) // 如果噪声值高于阈值，则不创建Cube
                    {
                        continue; // 跳过当前迭代，不生成Cube
                    }

                    // 在当前位置创建一个Cube
                    GameObject cube = Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
                    // 交替使用两种材质
                    Material selectedMaterial = (x + y + z) % 2 == 0 ? m1 : m2;
                    cube.GetComponent<Renderer>().material = selectedMaterial;
                }
            }
        }
    }
}

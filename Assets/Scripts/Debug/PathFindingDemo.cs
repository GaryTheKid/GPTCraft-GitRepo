using UnityEngine;

public class PathFindingDemo : MonoBehaviour
{
    public Material m1;
    public Material m2;
    public GameObject cubePrefab; // ȷ������һ��Cube��Prefab

    // Perlin Noise����
    public float scale = 0.6f; // ����ϵ���������Ըı������ġ�Ƶ�ʡ�
    public float threshold = 0.5f; // ����Cube����ֵ

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < 17; x++) // X������չ��16
        {
            for (int y = 0; y < 6; y++) // Y��߶�����Ϊ6
            {
                for (int z = 0; z < 17; z++) // Z������չ��16
                {
                    // ʹ��Perlin Noise�����Ƿ񴴽�Cube
                    float noise = Mathf.PerlinNoise((x + transform.position.x) * scale, (z + transform.position.z) * scale);

                    // ���ǵ�Y��ı仯������ֵû��Ӱ�죬���ﲻ��Ҫ�޸����������߼�
                    if (y + transform.position.y > noise * 10f) // �������ֵ������ֵ���򲻴���Cube
                    {
                        continue; // ������ǰ������������Cube
                    }

                    // �ڵ�ǰλ�ô���һ��Cube
                    GameObject cube = Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
                    // ����ʹ�����ֲ���
                    Material selectedMaterial = (x + y + z) % 2 == 0 ? m1 : m2;
                    cube.GetComponent<Renderer>().material = selectedMaterial;
                }
            }
        }
    }
}

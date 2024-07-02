using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // ʹ��TextMeshPro

public class DebugTextManager : MonoBehaviour
{
    public static DebugTextManager Instance { get; private set; }

    public GameObject textPrefab; // ָ��TextMeshPro�ı�Ԥ�Ƽ�������

    private Dictionary<Vector2Int, TextMeshPro> debugTexts = new Dictionary<Vector2Int, TextMeshPro>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetDebugText(Vector2Int coord, string text)
    {
        if (debugTexts.ContainsKey(coord))
        {
            // ����Ѿ����ı���������
            debugTexts[coord].text = text;
            StartCoroutine(FlashText(debugTexts[coord])); // ���ñ�ɫЧ��
        }
        else
        {
            // ���򣬴����µ��ı�����
            GameObject textObj = Instantiate(textPrefab, new Vector3(coord.x + 0.5f, 5f, coord.y + 0.5f), Quaternion.Euler(90f, 0f, 0f), transform);
            TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = text;
            debugTexts.Add(coord, tmp);
            StartCoroutine(FlashText(tmp)); // �����ı����ñ�ɫЧ��
        }
    }

    private IEnumerator FlashText(TextMeshPro tmp)
    {
        Color originalColor = tmp.color; // ����ԭʼ��ɫ
        tmp.color = Color.red; // ���
        yield return new WaitForSeconds(0.1f); // �ȴ�0.3��
        tmp.color = originalColor; // �ָ�ԭʼ��ɫ
    }
}


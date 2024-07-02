using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // 使用TextMeshPro

public class DebugTextManager : MonoBehaviour
{
    public static DebugTextManager Instance { get; private set; }

    public GameObject textPrefab; // 指向TextMeshPro文本预制件的引用

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
            // 如果已经有文本，更新它
            debugTexts[coord].text = text;
            StartCoroutine(FlashText(debugTexts[coord])); // 调用变色效果
        }
        else
        {
            // 否则，创建新的文本对象
            GameObject textObj = Instantiate(textPrefab, new Vector3(coord.x + 0.5f, 5f, coord.y + 0.5f), Quaternion.Euler(90f, 0f, 0f), transform);
            TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
            tmp.text = text;
            debugTexts.Add(coord, tmp);
            StartCoroutine(FlashText(tmp)); // 对新文本调用变色效果
        }
    }

    private IEnumerator FlashText(TextMeshPro tmp)
    {
        Color originalColor = tmp.color; // 保存原始颜色
        tmp.color = Color.red; // 变红
        yield return new WaitForSeconds(0.1f); // 等待0.3秒
        tmp.color = originalColor; // 恢复原始颜色
    }
}


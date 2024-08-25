using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Survival_EXPBar : MonoBehaviour
{
    public PlayerEXPController playerEXPController;

    public Image expFillImage;  // ������ʾ������ȵ�Image
    public TextMeshProUGUI levelText;  // ������ʾ�ȼ���TextMeshProUGUI

    private void OnEnable()
    {
        playerEXPController.OnExpChange += UpdateEXPUI;
    }

    private void OnDisable()
    {
        playerEXPController.OnExpChange -= UpdateEXPUI;
    }

    public void UpdateEXPUI(float currentEXP, float nextLevelEXP, short currentLevel)
    {
        // ���¾����������������
        expFillImage.fillAmount = currentEXP / nextLevelEXP;

        // ���µȼ���ʾ
        levelText.text = currentLevel.ToString();
    }
}

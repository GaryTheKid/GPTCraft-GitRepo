using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Survival_EXPBar : MonoBehaviour
{
    public PlayerEXPController playerEXPController;

    public Image expFillImage;  // 用于显示经验进度的Image
    public TextMeshProUGUI levelText;  // 用于显示等级的TextMeshProUGUI

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
        // 更新经验进度条的填充比例
        expFillImage.fillAmount = currentEXP / nextLevelEXP;

        // 更新等级显示
        levelText.text = currentLevel.ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;  // 导入UI命名空间

public class UI_Survival_LifeBar : MonoBehaviour
{
    public PlayerHealthController playerHealthController;

    public Image[] hearts;  // 心形图标的数组
    public Sprite fullHeartSprite;  // 完整心形的图片
    public Sprite halfHeartSprite;  // 半颗心的图片

    private void OnEnable()
    {
        playerHealthController.OnHealthChanged += UpdateLifeUI;
    }

    private void OnDisable()
    {
        playerHealthController.OnHealthChanged -= UpdateLifeUI;
    }

    // 更新UI显示生命值
    public void UpdateLifeUI(byte currentHealth, byte maxHealth)
    {
        int heartCount = maxHealth / 2;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < heartCount)
            {
                if (i < currentHealth / 2)
                {
                    hearts[i].sprite = fullHeartSprite;  // 完整的心
                    hearts[i].enabled = true;  // 确保图标启用
                }
                else if (i == currentHealth / 2 && currentHealth % 2 == 1)
                {
                    hearts[i].sprite = halfHeartSprite;  // 半个心
                    hearts[i].enabled = true;  // 确保图标启用
                }
                else
                {
                    hearts[i].sprite = null;  // 无图标，隐藏心形图标
                    hearts[i].enabled = false;  // 禁用图标
                }
            }
            else
            {
                hearts[i].enabled = false;  // 隐藏多余的心形图标
            }
        }
    }
}

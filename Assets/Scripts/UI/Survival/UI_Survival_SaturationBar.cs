using UnityEngine;
using UnityEngine.UI;

public class UI_Survival_SaturationBar : MonoBehaviour
{
    public PlayerHealthController playerHealthController;

    public Image[] saturationIcons;  // 饱腹感（Saturation）图标的数组
    public Sprite fullSaturationSprite;  // 完整饱腹感（Saturation）图标
    public Sprite halfSaturationSprite;  // 半饱腹感（Saturation）图标

    private void OnEnable()
    {
        playerHealthController.OnSaturationChanged += UpdateSaturationUI;
    }

    private void OnDisable()
    {
        playerHealthController.OnSaturationChanged -= UpdateSaturationUI;
    }

    public void UpdateSaturationUI(byte currentSaturation, byte maxSaturation)
    {
        int saturationCount = maxSaturation / 2;

        for (int i = saturationIcons.Length - 1; i >= 0; i--)
        {
            int indexFromLeft = saturationIcons.Length - 1 - i;

            if (indexFromLeft < saturationCount)
            {
                if (indexFromLeft < currentSaturation / 2)
                {
                    saturationIcons[i].sprite = fullSaturationSprite;  // 完整饱腹感图标
                    saturationIcons[i].enabled = true;  // 确保图标启用
                }
                else if (indexFromLeft == currentSaturation / 2 && currentSaturation % 2 == 1)
                {
                    saturationIcons[i].sprite = halfSaturationSprite;  // 半饱腹感图标
                    saturationIcons[i].enabled = true;  // 确保图标启用
                }
                else
                {
                    saturationIcons[i].sprite = null;  // 无图标，隐藏饱腹感图标
                    saturationIcons[i].enabled = false;  // 禁用图标
                }
            }
            else
            {
                saturationIcons[i].enabled = false;  // 隐藏多余的饱腹感图标
            }
        }
    }
}
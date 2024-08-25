using UnityEngine;
using UnityEngine.UI;

public class UI_Survival_SaturationBar : MonoBehaviour
{
    public PlayerHealthController playerHealthController;

    public Image[] saturationIcons;  // �����У�Saturation��ͼ�������
    public Sprite fullSaturationSprite;  // ���������У�Saturation��ͼ��
    public Sprite halfSaturationSprite;  // �뱥���У�Saturation��ͼ��

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
                    saturationIcons[i].sprite = fullSaturationSprite;  // ����������ͼ��
                    saturationIcons[i].enabled = true;  // ȷ��ͼ������
                }
                else if (indexFromLeft == currentSaturation / 2 && currentSaturation % 2 == 1)
                {
                    saturationIcons[i].sprite = halfSaturationSprite;  // �뱥����ͼ��
                    saturationIcons[i].enabled = true;  // ȷ��ͼ������
                }
                else
                {
                    saturationIcons[i].sprite = null;  // ��ͼ�꣬���ر�����ͼ��
                    saturationIcons[i].enabled = false;  // ����ͼ��
                }
            }
            else
            {
                saturationIcons[i].enabled = false;  // ���ض���ı�����ͼ��
            }
        }
    }
}
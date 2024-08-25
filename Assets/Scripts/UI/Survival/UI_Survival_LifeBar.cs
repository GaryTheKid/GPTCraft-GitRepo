using UnityEngine;
using UnityEngine.UI;  // ����UI�����ռ�

public class UI_Survival_LifeBar : MonoBehaviour
{
    public PlayerHealthController playerHealthController;

    public Image[] hearts;  // ����ͼ�������
    public Sprite fullHeartSprite;  // �������ε�ͼƬ
    public Sprite halfHeartSprite;  // ����ĵ�ͼƬ

    private void OnEnable()
    {
        playerHealthController.OnHealthChanged += UpdateLifeUI;
    }

    private void OnDisable()
    {
        playerHealthController.OnHealthChanged -= UpdateLifeUI;
    }

    // ����UI��ʾ����ֵ
    public void UpdateLifeUI(byte currentHealth, byte maxHealth)
    {
        int heartCount = maxHealth / 2;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < heartCount)
            {
                if (i < currentHealth / 2)
                {
                    hearts[i].sprite = fullHeartSprite;  // ��������
                    hearts[i].enabled = true;  // ȷ��ͼ������
                }
                else if (i == currentHealth / 2 && currentHealth % 2 == 1)
                {
                    hearts[i].sprite = halfHeartSprite;  // �����
                    hearts[i].enabled = true;  // ȷ��ͼ������
                }
                else
                {
                    hearts[i].sprite = null;  // ��ͼ�꣬��������ͼ��
                    hearts[i].enabled = false;  // ����ͼ��
                }
            }
            else
            {
                hearts[i].enabled = false;  // ���ض��������ͼ��
            }
        }
    }
}

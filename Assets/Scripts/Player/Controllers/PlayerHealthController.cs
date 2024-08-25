using System;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    private PlayerStats stats;

    public event Action<byte, byte> OnHealthChanged;
    public event Action<byte, byte> OnSaturationChanged;

    private float saturationTimer;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        HandleSaturationNaturalDrop();
    }

    public void TakeDamage(byte damage)
    {
        // ȷ���˺�ֵ������
        if (damage < 0)
        {
            Debug.LogWarning("Negative damage value received. Ignoring.");
            return;
        }

        Debug.Log("Take Damage: " + damage);

        // ȷ������ֵ�����Ϊ��ֵ
        stats.SURVIVAL_HP = (byte)Mathf.Max(0, stats.SURVIVAL_HP - damage);

        // �����¼������ݵ�ǰ����ֵ���������ֵ
        OnHealthChanged?.Invoke(stats.SURVIVAL_HP, stats.SURVIVAL_maxHP);

        // �������ֵΪ�㣬ִ�������߼�
        if (stats.SURVIVAL_HP == 0)
        {
            Die();
        }
    }

    public void Heal(byte healAmount)
    {
        // ȷ���ָ���������
        if (healAmount < 0)
        {
            Debug.LogWarning("Negative heal amount received. Ignoring.");
            return;
        }

        Debug.Log("Heal: " + healAmount);

        // ȷ������ֵ���ᳬ�����ֵ
        stats.SURVIVAL_HP = (byte)Mathf.Min(stats.SURVIVAL_maxHP, stats.SURVIVAL_HP + healAmount);

        // �����¼������ݵ�ǰ����ֵ���������ֵ
        OnHealthChanged?.Invoke(stats.SURVIVAL_HP, stats.SURVIVAL_maxHP);
    }

    public void DecreaseSaturation(byte amount)
    {
        // ȷ��������������
        if (amount < 0)
        {
            Debug.LogWarning("Negative saturation decrease value received. Ignoring.");
            return;
        }

        Debug.Log("Decrease Saturation: " + amount);

        // ȷ�������в����Ϊ��ֵ
        stats.SURVIVAL_saturation = (byte)Mathf.Max(0, stats.SURVIVAL_saturation - amount);

        // �����¼������ݵ�ǰ�����к���󱥸���
        OnSaturationChanged?.Invoke(stats.SURVIVAL_saturation, stats.SURVIVAL_maxSaturation);
    }

    public void IncreaseSaturation(byte amount)
    {
        // ȷ��������������
        if (amount < 0)
        {
            Debug.LogWarning("Negative saturation increase value received. Ignoring.");
            return;
        }

        Debug.Log("Increase Saturation: " + amount);

        // ȷ�������в��ᳬ�����ֵ
        stats.SURVIVAL_saturation = (byte)Mathf.Min(stats.SURVIVAL_maxSaturation, stats.SURVIVAL_saturation + amount);

        // �����¼������ݵ�ǰ�����к���󱥸���
        OnSaturationChanged?.Invoke(stats.SURVIVAL_saturation, stats.SURVIVAL_maxSaturation);
    }

    private void HandleSaturationNaturalDrop()
    {
        saturationTimer -= Time.deltaTime;
        if (saturationTimer <= 0)
        {
            // ��������и���10���ظ�1������
            if (stats.SURVIVAL_saturation > 10)
            {
                Heal(1);
            }
            // ���������Ϊ0���۳�1������
            else if (stats.SURVIVAL_saturation == 0)
            {
                TakeDamage(1);
            }

            // ����1�㱥����
            DecreaseSaturation(1);

            // ���ü�ʱ��
            saturationTimer = stats.SURVIVAL_saturationDecreaseInterval;
        }
    }

    void Die()
    {
        // ������������߼�
        Debug.Log("Player died!");
        // ���磬���Խ�����ҿ��ƣ���������������
    }
}

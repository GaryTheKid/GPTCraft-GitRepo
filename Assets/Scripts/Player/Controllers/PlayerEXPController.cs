using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEXPController : MonoBehaviour
{
    private PlayerStats stats;

    public event Action<float, float, short> OnExpChange; // ��ǰ����ֵ��������������ֵ

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        ResetExperience();
    }

    public void GainExperience(float amount)
    {
        // ȷ������ֵ������Ϊ����
        if (amount < 0)
        {
            Debug.LogWarning("Negative experience amount received. Ignoring.");
            return;
        }

        Debug.Log("Gained Experience: " + amount);

        // ���Ӿ���ֵ
        stats.SURVIVAL_EXP += amount;

        // ����Ƿ��������
        while (stats.SURVIVAL_EXP >= stats.SURVIVAL_NextLVEXP)
        {
            LevelUp();
        }

        // �����¼������ݵ�ǰ����ֵ��������������ֵ
        OnExpChange?.Invoke(stats.SURVIVAL_EXP, stats.SURVIVAL_NextLVEXP, stats.SURVIVAL_LV);
    }

    private void LevelUp()
    {
        // ���ӵȼ�
        stats.SURVIVAL_LV++;
        Debug.Log("Level Up! New Level: " + stats.SURVIVAL_LV);

        // ��ȥ��������ľ���ֵ
        stats.SURVIVAL_EXP -= stats.SURVIVAL_NextLVEXP;

        // ������һ���ȼ�����ľ���ֵ
        stats.SURVIVAL_NextLVEXP = CalculateNextLevelEXP((short)(stats.SURVIVAL_LV + 1));

        // ��������ֵ�仯�¼�
        OnExpChange?.Invoke(stats.SURVIVAL_EXP, stats.SURVIVAL_NextLVEXP, stats.SURVIVAL_LV);
    }

    private float CalculateNextLevelEXP(short level)
    {
        // �����趨
        float A = 1000f;
        float B = 44000f;
        float C = 0.95f;

        // ������һ�ȼ�����ľ���ֵ
        return A * Mathf.Pow((level - 1) / B, 1f / 3f) + C * (level - 1);
    }


    public void ResetExperience()
    {
        // ���þ���ֵ�͵ȼ�
        stats.SURVIVAL_EXP = 0f;
        stats.SURVIVAL_LV = 1;
        stats.SURVIVAL_NextLVEXP = 10f; // ��ʼ���������辭��ֵΪ10

        Debug.Log("Experience and Level Reset.");

        // �����¼������ݵ�ǰ����ֵ��������������ֵ
        OnExpChange?.Invoke(stats.SURVIVAL_EXP, stats.SURVIVAL_NextLVEXP, stats.SURVIVAL_LV);
    }
}

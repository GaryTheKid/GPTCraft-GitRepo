using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEXPController : MonoBehaviour
{
    private PlayerStats stats;

    public event Action<float, float, short> OnExpChange; // 当前经验值和所需升级经验值

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
        // 确保经验值增加量为正数
        if (amount < 0)
        {
            Debug.LogWarning("Negative experience amount received. Ignoring.");
            return;
        }

        Debug.Log("Gained Experience: " + amount);

        // 增加经验值
        stats.SURVIVAL_EXP += amount;

        // 检查是否可以升级
        while (stats.SURVIVAL_EXP >= stats.SURVIVAL_NextLVEXP)
        {
            LevelUp();
        }

        // 触发事件，传递当前经验值和所需升级经验值
        OnExpChange?.Invoke(stats.SURVIVAL_EXP, stats.SURVIVAL_NextLVEXP, stats.SURVIVAL_LV);
    }

    private void LevelUp()
    {
        // 增加等级
        stats.SURVIVAL_LV++;
        Debug.Log("Level Up! New Level: " + stats.SURVIVAL_LV);

        // 减去升级所需的经验值
        stats.SURVIVAL_EXP -= stats.SURVIVAL_NextLVEXP;

        // 计算下一个等级所需的经验值
        stats.SURVIVAL_NextLVEXP = CalculateNextLevelEXP((short)(stats.SURVIVAL_LV + 1));

        // 触发经验值变化事件
        OnExpChange?.Invoke(stats.SURVIVAL_EXP, stats.SURVIVAL_NextLVEXP, stats.SURVIVAL_LV);
    }

    private float CalculateNextLevelEXP(short level)
    {
        // 参数设定
        float A = 1000f;
        float B = 44000f;
        float C = 0.95f;

        // 计算下一等级所需的经验值
        return A * Mathf.Pow((level - 1) / B, 1f / 3f) + C * (level - 1);
    }


    public void ResetExperience()
    {
        // 重置经验值和等级
        stats.SURVIVAL_EXP = 0f;
        stats.SURVIVAL_LV = 1;
        stats.SURVIVAL_NextLVEXP = 10f; // 初始的升级所需经验值为10

        Debug.Log("Experience and Level Reset.");

        // 触发事件，传递当前经验值和所需升级经验值
        OnExpChange?.Invoke(stats.SURVIVAL_EXP, stats.SURVIVAL_NextLVEXP, stats.SURVIVAL_LV);
    }
}

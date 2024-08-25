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
        // 确保伤害值是正数
        if (damage < 0)
        {
            Debug.LogWarning("Negative damage value received. Ignoring.");
            return;
        }

        Debug.Log("Take Damage: " + damage);

        // 确保生命值不会变为负值
        stats.SURVIVAL_HP = (byte)Mathf.Max(0, stats.SURVIVAL_HP - damage);

        // 触发事件，传递当前生命值和最大生命值
        OnHealthChanged?.Invoke(stats.SURVIVAL_HP, stats.SURVIVAL_maxHP);

        // 如果生命值为零，执行死亡逻辑
        if (stats.SURVIVAL_HP == 0)
        {
            Die();
        }
    }

    public void Heal(byte healAmount)
    {
        // 确保恢复量是正数
        if (healAmount < 0)
        {
            Debug.LogWarning("Negative heal amount received. Ignoring.");
            return;
        }

        Debug.Log("Heal: " + healAmount);

        // 确保生命值不会超过最大值
        stats.SURVIVAL_HP = (byte)Mathf.Min(stats.SURVIVAL_maxHP, stats.SURVIVAL_HP + healAmount);

        // 触发事件，传递当前生命值和最大生命值
        OnHealthChanged?.Invoke(stats.SURVIVAL_HP, stats.SURVIVAL_maxHP);
    }

    public void DecreaseSaturation(byte amount)
    {
        // 确保减少量是正数
        if (amount < 0)
        {
            Debug.LogWarning("Negative saturation decrease value received. Ignoring.");
            return;
        }

        Debug.Log("Decrease Saturation: " + amount);

        // 确保饱腹感不会变为负值
        stats.SURVIVAL_saturation = (byte)Mathf.Max(0, stats.SURVIVAL_saturation - amount);

        // 触发事件，传递当前饱腹感和最大饱腹感
        OnSaturationChanged?.Invoke(stats.SURVIVAL_saturation, stats.SURVIVAL_maxSaturation);
    }

    public void IncreaseSaturation(byte amount)
    {
        // 确保增加量是正数
        if (amount < 0)
        {
            Debug.LogWarning("Negative saturation increase value received. Ignoring.");
            return;
        }

        Debug.Log("Increase Saturation: " + amount);

        // 确保饱腹感不会超过最大值
        stats.SURVIVAL_saturation = (byte)Mathf.Min(stats.SURVIVAL_maxSaturation, stats.SURVIVAL_saturation + amount);

        // 触发事件，传递当前饱腹感和最大饱腹感
        OnSaturationChanged?.Invoke(stats.SURVIVAL_saturation, stats.SURVIVAL_maxSaturation);
    }

    private void HandleSaturationNaturalDrop()
    {
        saturationTimer -= Time.deltaTime;
        if (saturationTimer <= 0)
        {
            // 如果饱腹感高于10，回复1点生命
            if (stats.SURVIVAL_saturation > 10)
            {
                Heal(1);
            }
            // 如果饱腹感为0，扣除1点生命
            else if (stats.SURVIVAL_saturation == 0)
            {
                TakeDamage(1);
            }

            // 减少1点饱腹感
            DecreaseSaturation(1);

            // 重置计时器
            saturationTimer = stats.SURVIVAL_saturationDecreaseInterval;
        }
    }

    void Die()
    {
        // 处理玩家死亡逻辑
        Debug.Log("Player died!");
        // 例如，可以禁用玩家控制，播放死亡动画等
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageController : MonoBehaviour
{
    private PlayerStats stats;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void TakeDamage(byte damage)
    {
        // Ensure damage value is positive
        if (damage < 0)
        {
            Debug.LogWarning("Negative damage value received. Ignoring.");
            return;
        }

        Debug.Log("Take Damage: " + damage);

        // Ensure survival HP is always non-negative
        stats.SURVIVAL_HP = (byte)Mathf.Max(0, stats.SURVIVAL_HP - damage);

        // Check if survival HP is zero, meaning player is dead
        if (stats.SURVIVAL_HP == 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle player death here
        Debug.Log("Player died!");
        // For example, you might want to disable player control, play death animation, etc.
    }
}

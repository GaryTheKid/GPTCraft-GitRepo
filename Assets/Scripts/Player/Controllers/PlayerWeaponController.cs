using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public delegate void OnMeleeAttackEventHandler(ItemData itemData, IDurable item, short durabilityChangeAmount);
    public event OnMeleeAttackEventHandler OnMeleeAttackEvent;

    public List<MobWorldController> enemies;

    private PlayerStats stats;

    private void Awake()
    {
        enemies = new List<MobWorldController>();
        stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // 在Update中检测鼠标左键点击来触发攻击
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    public void AddEnemy(MobWorldController enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(MobWorldController enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    public void Attack()
    {
        if (stats.STATE_invPageState != InventoryPageState.None) return;

        foreach (MobWorldController enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(transform, stats.INTERACTION_attackDamage);

                if (stats.EQUIPMENT_equippedItem is IDurable)
                {
                    // 暂定掉除的耐久为1
                    OnMeleeAttackEvent(stats.EQUIPMENT_equippedItemData, stats.EQUIPMENT_equippedItem as IDurable, -1);
                }
            }
        }
    }
}


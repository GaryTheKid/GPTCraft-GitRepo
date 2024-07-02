using UnityEngine;

public abstract class Item_Weapon : Item, IEquipable, IDurable, IWeapon
{
    [Space(25)]

    [Header("====== Weapon Attributes ======")]
    public short maxDurability;
    public short attackDamage;
    public float attackSpeed;

    public short GetAttackDamage()
    {
        return attackDamage;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    public short GetMaxDurability()
    {
        return maxDurability;
    }

    public bool UpdateDurability(ItemData itemData, short changeAmount)
    {
        // 更新耐久值
        itemData.durability += changeAmount;

        // 确保耐久值不超过最大值
        if (itemData.durability > maxDurability)
        {
            itemData.durability = maxDurability;
        }

        // 判断是否已经损坏
        if (itemData.durability <= 0)
        {
            // 耐久值小于等于0，代表道具已经损坏
            itemData.durability = 0; // 确保耐久值不为负数
            return true;
        }
        else
        {
            // 耐久值大于0，道具未损坏
            return false;
        }
    }

    public override ItemData CreateItemData()
    {
        return new ItemData(id, maxDurability);
    }
}
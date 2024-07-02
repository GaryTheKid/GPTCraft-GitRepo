
using UnityEngine;

public abstract class Item_Tool : Item, IEquipable, IDurable, ITool
{
    [Space(25)]

    [Header("====== Tool Attributes ======")]
    public short maxDurability;
    public float destroyEfficiency;
    public float destroyEfficiencyBonus_Wood;
    public float destroyEfficiencyBonus_Stone;
    public float destroyEfficiencyBonus_SandMud;
    public float destroyEfficiencyBonus_Plant;

    public float GetDestroyEfficiency()
    {
        return destroyEfficiency;
    }

    public float GetDestroyEffiencyBonus_Wood()
    {
        return destroyEfficiencyBonus_Wood;
    }

    public float GetDestroyEffiencyBonus_Stone()
    {
        return destroyEfficiencyBonus_Stone;
    }

    public float GetDestroyEffiencyBonus_SandMud()
    {
        return destroyEfficiencyBonus_SandMud;
    }

    public float GetDestroyEffiencyBonus_Plant()
    {
        return destroyEfficiencyBonus_Plant;
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

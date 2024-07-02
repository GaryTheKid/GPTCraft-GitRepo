
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
        // �����;�ֵ
        itemData.durability += changeAmount;

        // ȷ���;�ֵ���������ֵ
        if (itemData.durability > maxDurability)
        {
            itemData.durability = maxDurability;
        }

        // �ж��Ƿ��Ѿ���
        if (itemData.durability <= 0)
        {
            // �;�ֵС�ڵ���0����������Ѿ���
            itemData.durability = 0; // ȷ���;�ֵ��Ϊ����
            return true;
        }
        else
        {
            // �;�ֵ����0������δ��
            return false;
        }
    }

    public override ItemData CreateItemData()
    {
        return new ItemData(id, maxDurability);
    }
}

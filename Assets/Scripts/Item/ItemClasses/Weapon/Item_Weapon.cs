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
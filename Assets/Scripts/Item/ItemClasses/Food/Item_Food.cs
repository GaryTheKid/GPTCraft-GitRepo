using UnityEngine;

public class Item_Food : Item, IEquipable, IFood
{
    [Header("====== Food ======")]
    public byte nutrition;

    public override ItemData CreateItemData()
    {
        return new ItemData(id);
    }

    public virtual byte GetFoodNutritionVal()
    {
        return nutrition;
    }
}
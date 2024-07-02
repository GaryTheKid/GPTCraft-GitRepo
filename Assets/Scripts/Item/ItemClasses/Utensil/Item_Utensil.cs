public abstract class Item_Utensil : Item, IEquipable
{
    public override ItemData CreateItemData()
    {
        return new ItemData(id);
    }
}
public abstract class Item_Block : Item, IEquipable, IConstructable
{
    public abstract byte GetItemBlockID();

    public override ItemData CreateItemData()
    {
        return new ItemData(id);
    }
}
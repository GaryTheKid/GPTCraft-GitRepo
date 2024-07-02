public class Item_Food : Item, IEquipable
{
    public override ItemData CreateItemData()
    {
        return new ItemData(id);
    }
}
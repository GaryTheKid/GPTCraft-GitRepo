[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int stackCount;

    public InventorySlot(ItemData itemData)
    {
        this.itemData = itemData;
        stackCount = 1; // 默认堆叠数量为1
    }

    public InventorySlot(ItemData itemData, int stackCount)
    {
        this.itemData = itemData;
        this.stackCount = stackCount; // 默认堆叠数量为1
    }

    public bool CompareItemDataType(ItemData itemData)
    {
        return this.itemData.id == itemData.id;
    }

    public bool CompareItemDataType(Item item)
    {
        return itemData.id == item.id;
    }
}
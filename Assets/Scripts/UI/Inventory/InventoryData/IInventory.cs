public interface IInventory
{
    bool AddItem(ItemData itemData);
    int AddItem(ItemData itemData, int amount);
    bool RemoveItem(ItemData itemData);
    bool RemoveItem(int index, int amount, out ItemData slotItemData);
    bool RemoveAllSlotItems(int index, out ItemData slotItemData, out int slotItemAmount);
    bool IsInventoryFull();

    InventorySlot GetItemSlot(int index);
}
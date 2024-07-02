public interface ICraftResult
{
    void ClearAllCraftResults();
    void SetBindedCraftingAreaInventory(Inventory inv);
    ICraftArea GetBindedCraftAreaInventory();
    bool IsCraftResultSlotEmpty();
    InventorySlot GetCraftResultSlot();
    abstract bool IsInventoryStateMatched(InventoryPageState state);
    abstract void UpdateCraftResult();
}
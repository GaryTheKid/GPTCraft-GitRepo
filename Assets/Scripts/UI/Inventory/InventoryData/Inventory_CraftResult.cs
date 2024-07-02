public abstract class Inventory_CraftResult : Inventory, ICraftResult
{
    protected ICraftArea bindedCraftArea;

    public Inventory_CraftResult(int size, int idHead)
    {
        this.size = size;
        this.idHead = idHead;
        this.inventorySlots = new InventorySlot[size];
    }

    public void ClearAllCraftResults()
    {
        for (int i = 0; i < size; i++)
        {
            inventorySlots[i] = null;
        }
    }

    public void SetBindedCraftingAreaInventory(Inventory inv)
    {
        bindedCraftArea = inv as ICraftArea;
    }

    public ICraftArea GetBindedCraftAreaInventory()
    {
        return bindedCraftArea;
    }

    public bool IsCraftResultSlotEmpty()
    {
        InventorySlot craftResultSlot = inventorySlots[0];
        return craftResultSlot == null || craftResultSlot.stackCount == 0 || craftResultSlot.itemData == null;
    }

    public InventorySlot GetCraftResultSlot()
    {
        return inventorySlots[0];
    }

    public abstract bool IsInventoryStateMatched(InventoryPageState state);

    public abstract void UpdateCraftResult();
}

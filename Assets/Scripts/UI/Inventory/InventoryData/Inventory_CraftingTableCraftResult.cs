public class Inventory_CraftingTableCraftResult : Inventory_CraftResult
{
    public const int INVENTORY_INDEX = 5;
    public const int SIZE = 1;
    public const int ID_HEAD = 50;

    public Inventory_CraftingTableCraftResult() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }

    public Inventory_CraftingTableCraftResult(Inventory bindedCraftAreaInv) : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];

        SetBindedCraftingAreaInventory(bindedCraftAreaInv);
    }

    public override void UpdateCraftResult()
    {
        // get hashstring
        string hashString = bindedCraftArea.GetCraftHashString();

        // get hashcode and recipe
        int hashCode = hashString.GetHashCode();

        if (ResourceAssets.singleton.craftingRecipes.ContainsKey(hashCode))
        {
            byte recipeID = ResourceAssets.singleton.craftingRecipes[hashCode];
            Item craftItem = ResourceAssets.singleton.items[recipeID];
            int CraftAmount = craftItem.craftingResultItemCount;

            if (IsCraftingAuthorized(craftItem))
            {
                inventorySlots[0] = new InventorySlot(craftItem.CreateItemData(), CraftAmount);
            }
        }
    }

    private bool IsCraftingAuthorized(Item craftItem)
    {
        return craftItem.craftAuth == CraftAuth.CraftingTable || craftItem.craftAuth == CraftAuth.Hand;
    }

    public override bool IsInventoryStateMatched(InventoryPageState state)
    {
        return state == InventoryPageState.CraftingTable;
    }
}

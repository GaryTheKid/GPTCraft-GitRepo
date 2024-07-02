using UnityEngine;

public class UI_Inventory_CraftingTableResult : UI_InventoryBase
{
    // backpackCraft Result (Single Slot)
    public Transform craftingTableResultFrame;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_CraftingTableCraftResult.SIZE;
        int slotsIDHead = Inventory_CraftingTableCraftResult.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(craftingTableResultFrame, slotsSize, slotsIDHead);

        // nothing special for initializing backpackCraftResult
    }
}
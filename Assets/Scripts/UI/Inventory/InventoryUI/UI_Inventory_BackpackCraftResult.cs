using UnityEngine;

public class UI_Inventory_BackpackCraftResult : UI_InventoryBase
{
    // backpackCraft Result (Single Slot)
    public Transform backpackCraftResultFrame;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_BackpackCraftResult.SIZE;
        int slotsIDHead = Inventory_BackpackCraftResult.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(backpackCraftResultFrame, slotsSize, slotsIDHead);

        // nothing special for initializing backpackCraftResult
    }
}
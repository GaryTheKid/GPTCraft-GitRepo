using UnityEngine;

public class UI_Inventory_BackpackCraft : UI_InventoryBase
{
    public Transform backpackCraftFrameParent;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_BackpackCraftArea.SIZE;
        int slotsIDHead = Inventory_BackpackCraftArea.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(backpackCraftFrameParent, slotsSize, slotsIDHead);

        // nothing special for initializing backpackCraft
    }
}

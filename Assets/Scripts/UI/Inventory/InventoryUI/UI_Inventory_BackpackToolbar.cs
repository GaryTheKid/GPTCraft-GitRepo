using UnityEngine;

public class UI_Inventory_BackpackToolbar : UI_InventoryBase
{
    public Transform backpackToolbarFrameParent;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_Toolbar.SIZE;
        int slotsIDHead = Inventory_Toolbar.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(backpackToolbarFrameParent, slotsSize, slotsIDHead);

        // nothing special for initializing backpackToolbar
    }
}

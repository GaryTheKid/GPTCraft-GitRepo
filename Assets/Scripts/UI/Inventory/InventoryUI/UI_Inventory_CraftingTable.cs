using UnityEngine;

public class UI_Inventory_CraftingTable : UI_InventoryBase, IUIPage
{
    public GameObject backpackCanvas;
    public GameObject CraftingTableCanvas;
    public GameObject CraftingTableCanvas_Background;
    public Transform craftingTableFrameParent;

    private bool isPageActive;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_CraftingTableCraftArea.SIZE;
        int slotsIDHead = Inventory_CraftingTableCraftArea.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(craftingTableFrameParent, slotsSize, slotsIDHead);

        // nothing special for initializing backpackCraft
    }

    public bool IsPageActive()
    {
        return isPageActive;
    }

    public void PageOn()
    {
        isPageActive = true;
        backpackCanvas.SetActive(true);
        CraftingTableCanvas.SetActive(true);
        CraftingTableCanvas_Background.SetActive(true);
    }

    public void PageOff()
    {
        isPageActive = false;
        backpackCanvas.SetActive(false);
        CraftingTableCanvas.SetActive(false);
        CraftingTableCanvas_Background.SetActive(false);
    }

    public InventoryPageState GetPageState()
    {
        return InventoryPageState.CraftingTable;
    }
}

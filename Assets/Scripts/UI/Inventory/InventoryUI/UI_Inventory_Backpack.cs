using UnityEngine;

public class UI_Inventory_Backpack : UI_InventoryBase, IUIPage
{
    public GameObject backpackCanvas;
    public GameObject backpackCraftCanvas;
    public GameObject backpackCanvas_background;
    public Transform backpackFrameParent;

    private bool isPageActive;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_Backpack.SIZE;
        int slotsIDHead = Inventory_Backpack.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(backpackFrameParent, slotsSize, slotsIDHead);

        // nothing special for initializing backpack
    }

    public bool IsPageActive()
    {
        return isPageActive;
    }

    public void PageOn()
    {
        isPageActive = true;
        backpackCanvas.SetActive(true);
        backpackCraftCanvas.SetActive(true);
        backpackCanvas_background.SetActive(true);
    }

    public void PageOff()
    {
        isPageActive = false;
        backpackCanvas.SetActive(false);
        backpackCraftCanvas.SetActive(false);
        backpackCanvas_background.SetActive(false);
    }

    public InventoryPageState GetPageState()
    {
        return InventoryPageState.Backpack;
    }
}

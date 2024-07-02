using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_FurnanceFuel : UI_InventoryBase
{
    // (Single Slot)
    public Transform furnanceFuelFrame;
    public Image combustProgress;

    public override void SetInventory(Inventory inv)
    {
        this.inv = inv;
        slots = inv.GetInventorySlots;

        IFurnanceFuel furnanceFuel = inv as IFurnanceFuel;
        furnanceFuel.SetCombustionProgressUpdate(UpdateCombustProgress);
    }

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_FurnanceFuel.SIZE;
        int slotsIDHead = Inventory_FurnanceFuel.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(furnanceFuelFrame, slotsSize, slotsIDHead);
    }

    public void UpdateCombustProgress(float fillAmount)
    {
        combustProgress.fillAmount = fillAmount;
    }
}

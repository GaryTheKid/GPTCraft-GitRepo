using UnityEngine.UI;
using UnityEngine;

public class UI_Inventory_FurnanceProduct : UI_InventoryBase
{
    // (Single Slot)
    public Transform furnanceProductFrame;
    public Image productionProgress;

    public override void SetInventory(Inventory inv)
    {
        this.inv = inv;
        slots = inv.GetInventorySlots;

        IFurnanceProduct furnanceProduct = inv as IFurnanceProduct;
        furnanceProduct.SetProductionProgressUpdate(UpdateProductionProgress);
    }

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_FurnanceProduct.SIZE;
        int slotsIDHead = Inventory_FurnanceProduct.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(furnanceProductFrame, slotsSize, slotsIDHead);
    }

    public void UpdateProductionProgress(float fillAmount)
    {
        productionProgress.fillAmount = fillAmount;
    }
}
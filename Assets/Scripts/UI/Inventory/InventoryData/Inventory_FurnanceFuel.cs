using System;
using UnityEngine;

public class Inventory_FurnanceFuel : Inventory, IFurnanceFuel
{
    public const int INVENTORY_INDEX = 8;
    public const int SIZE = 1;
    public const int ID_HEAD = 53;

    private Action<float> OnCombustionProgressUpdate;

    private IFurnanceProduct bindedFurnanceProductInv;
    private FurnanceProductionData productionData;

    #region Constructors
    public Inventory_FurnanceFuel() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }
    public Inventory_FurnanceFuel(Inventory inv) : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];

        SetBindedFurnanceProductInventory(inv);
    }
    #endregion


    #region Initialization
    private void SetBindedFurnanceProductInventory(Inventory inv)
    {
        bindedFurnanceProductInv = inv as IFurnanceProduct;
        productionData = bindedFurnanceProductInv.GetProductionData();
    }
    public void SetCombustionProgressUpdate(Action<float> OnCombustionProgressUpdate)
    {
        this.OnCombustionProgressUpdate = OnCombustionProgressUpdate;
    }
    #endregion


    #region Combustion
    public void LoadFuel()
    {
        if (CheckFuel(out IFuel fuelInfo)) productionData.LoadFuel(fuelInfo);
        else productionData.UnloadFuel();
    }
    public void InitializeCombustion()
    { 
        if (!productionData.IsCombustable()) return;

        if (bindedFurnanceProductInv.StartCombustionProcess(InitializeCombustion, LoadFuel, OnCombustionProgressUpdate, productionData))
        {
            ConsumeOneFuel();
            productionData.ActivateFurnance();
        }
    }
    private void ConsumeOneFuel()
    {
        if (IsSlotEmpty(inventorySlots[0])) return;

        inventorySlots[0].stackCount--;

        if (inventorySlots[0].stackCount <= 0)
        {
            inventorySlots[0] = null;
        }
    }
    public bool CheckFuel(out IFuel fuelInfo)
    {
        fuelInfo = null;

        InventorySlot slot = inventorySlots[0];
        if (IsSlotEmpty(slot)) return false;

        fuelInfo = GetFuel();
        if (fuelInfo == null) return false;
        return true;
    }
    private IFuel GetFuel()
    {
        byte fuelItemId = inventorySlots[0].itemData.id;
        if (ResourceAssets.singleton.items.ContainsKey(fuelItemId))
        {
            Item fuelItem = ResourceAssets.singleton.items[fuelItemId];

            if (IsFuel(fuelItem))
            {
                return fuelItem as IFuel;
            }
            else
            {
                return null;
            }
        }

        return null;
    }
    private bool IsFuel(Item prodItem)
    {
        return prodItem is IFuel;
    }
    #endregion


    #region Furnance UI Page
    public bool IsInventoryStateMatched(InventoryPageState state)
    {
        return state == InventoryPageState.Furnance;
    }
    #endregion
}

using System;
using UnityEngine;

public class Inventory_FurnanceProduct : Inventory, IFurnanceProduct
{
    public const int INVENTORY_INDEX = 7;
    public const int SIZE = 1;
    public const int ID_HEAD = 52;

    private Action<float> OnProductionProgressUpdate;
    private Action OnCompletionUIUpdate;

    public FurnanceProductionData productionData;

    #region Constructors
    public Inventory_FurnanceProduct() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];

        SetProductionData(new FurnanceProductionData());
    }
    #endregion


    #region Initialization
    public void SetProductionData(FurnanceProductionData productionData)
    {
        this.productionData = productionData;
    }
    public FurnanceProductionData GetProductionData()
    {
        return productionData;
    }
    public void SetProductionProgressUpdate(Action<float> OnProductionProgressUpdate)
    {
        this.OnProductionProgressUpdate = OnProductionProgressUpdate;
    }
    public void SetOnCompletionUIUpdate(Action OnCompletionUIUpdate)
    {
        this.OnCompletionUIUpdate = OnCompletionUIUpdate;
    }
    #endregion


    #region Simulate Production
    public bool StartProductionProcess(Action OnAttemptReInitializeProduction, Action OnReloadSource, Action ConsumeOneSourceItem, FurnanceProductionData productionData)
    {
        if (IsProductSlotStackFull()) return false;

        return WorldObjectSimulationManager.singleton.Simulate_StartFurnanceProduction(productionData, OnAttemptReInitializeProduction, OnReloadSource, ConsumeOneSourceItem, OnProductionProgressUpdate , CompleteProductionProcess, OnCompletionUIUpdate);
    }
    public void StopProductionProcess()
    {
        WorldObjectSimulationManager.singleton.Simulate_StopFurnanceProduction(productionData, OnProductionProgressUpdate, OnCompletionUIUpdate);
    }
    private void CompleteProductionProcess(Action OnAttemptReInitializeProduction, Action OnReloadSource, Action ConsumeOneSourceItem, IForgeable prodForgeInfo, Item prodItem)
    {
        if (prodForgeInfo != null && prodItem != null)
        {
            GenerateProduct(prodForgeInfo.GetProductCount(), prodItem.CreateItemData());
            ConsumeOneSourceItem();
        }
        
        OnReloadSource();
        OnAttemptReInitializeProduction();
    }
    private void GenerateProduct(int prodCount, ItemData prodItemData)
    {
        InventorySlot prodSlot = inventorySlots[0];
        if (IsSlotStackFull(prodSlot)) return;

        if (IsSlotEmpty(prodSlot))
        {
            inventorySlots[0] = new InventorySlot(prodItemData, prodCount);
        }
        else
        {
            if (prodSlot.CompareItemDataType(prodItemData))
            {
                prodSlot.stackCount += prodCount;
            }
            else
            {
                inventorySlots[0] = new InventorySlot(prodItemData, prodCount);
            }
        }

        if (inventorySlots[0].stackCount > SIZE_MAXSTACKSIZE) inventorySlots[0].stackCount = SIZE_MAXSTACKSIZE;
    }
    public bool IsProductSlotReadyForProduction(Item prodItem)
    {
        InventorySlot prodSlot = inventorySlots[0];
        if (IsSlotStackFull(prodSlot)) return false;

        if (IsSlotEmpty(prodSlot))
        {
            return true;
        }
        else
        {
            if (prodSlot.CompareItemDataType(prodItem))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public bool IsProductSlotStackFull()
    {
        if (IsSlotEmpty(inventorySlots[0])) return false;
        else return inventorySlots[0].stackCount >= SIZE_MAXSTACKSIZE;
    }
    #endregion


    #region Simulate Combustion
    public bool StartCombustionProcess(Action OnAttemptReInitializeCombustion, Action OnReloadFuel, Action<float> OnCombustionProgressUpdate, FurnanceProductionData productionData)
    {
        if (IsProductSlotStackFull()) return false;

        return WorldObjectSimulationManager.singleton.Simulate_StartFurnanceCombustion(productionData, OnAttemptReInitializeCombustion, OnReloadFuel, OnCombustionProgressUpdate, CompleteCombustionProcess, OnCompletionUIUpdate);
    }
    private void CompleteCombustionProcess(Action OnAttemptReInitializeCombustion, Action OnReloadFuel)
    {
        OnReloadFuel();
        productionData.CheckFurnanceActiveState();
        OnAttemptReInitializeCombustion();
    }
    #endregion
}




// For later use
public class FurnanceProductionData
{
    // production requirements
    public Item prodItem;
    public IForgeable prodForgeInfo;
    public IFuel fuelInfo;

    // production info
    public bool isSourceReady;
    public bool isFuelReady;
    public bool isActive;
    public float productionProgress;
    public float combustionProgress;

    public FurnanceProductionData()
    {

    }

    public void LoadSource(Item prodItem, IForgeable prodForgeInfo)
    {
        this.prodItem = prodItem;
        this.prodForgeInfo = prodForgeInfo;
        isSourceReady = true;
    }

    public void UnloadSource()
    {
        prodItem = null;
        prodForgeInfo = null;
        isSourceReady = false;
    }

    public void LoadFuel(IFuel fuelInfo)
    {
        this.fuelInfo = fuelInfo;
        isFuelReady = true;
    }

    public void UnloadFuel()
    {
        fuelInfo = null;
        isFuelReady = false;
    }

    public void ActivateFurnance()
    {
        isActive = true;
    }

    public void CheckFurnanceActiveState()
    {
        if (!isFuelReady || !isFuelReady) isActive = false;
    }

    public bool IsProductable()
    {
        if (isActive && isSourceReady) return true;
        else return false;
    }

    public bool IsCombustable()
    {
        if (isFuelReady && isSourceReady) return true;
        else return false;
    }
}
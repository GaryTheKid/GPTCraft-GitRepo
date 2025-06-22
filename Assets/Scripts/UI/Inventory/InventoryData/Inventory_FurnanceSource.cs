using UnityEngine;
public class Inventory_FurnanceSource : Inventory, IFurnanceSource, I1x1CraftArea
{
    public const int INVENTORY_INDEX = 6;
    public const int SIZE = 1;
    public const int ID_HEAD = 51;

    private IFurnanceProduct bindedFurnanceProductInv;
    private FurnanceProductionData productionData;

    #region Constructors
    public Inventory_FurnanceSource() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }
    public Inventory_FurnanceSource(Inventory inv) : base(SIZE, ID_HEAD)
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
    #endregion


    #region Production
    public void LoadSource()
    {
        if (CheckSourceItemForProduction(out Item prodItem, out IForgeable prodForgeInfo)) productionData.LoadSource(prodItem, prodForgeInfo);
        else productionData.UnloadSource();
    }
    public void InitializeProduction()
    {
        if (!productionData.IsProductable()) return;

        bindedFurnanceProductInv.StartProductionProcess(InitializeProduction, LoadSource, ConsumeOneSourceItem, productionData);
    }
    public void ResetProduction()
    {
        bindedFurnanceProductInv.StopProductionProcess();
    }
    public void ConsumeOneSourceItem()
    {
        if (IsSlotEmpty(inventorySlots[0])) return;

        inventorySlots[0].stackCount--;

        if (inventorySlots[0].stackCount <= 0)
        {
            inventorySlots[0] = null;
        }
    }
    public bool CheckSourceItemForProduction(out Item prodItem, out IForgeable prodForgeInfo)
    {
        prodForgeInfo = null;
        prodItem = null;

        InventorySlot slot = inventorySlots[0];
        if (IsSlotEmpty(slot)) return false;

        prodItem = GetProductItem();
        if (prodItem == null) return false;
        if (!bindedFurnanceProductInv.IsProductSlotReadyForProduction(prodItem)) return false;

        prodForgeInfo = prodItem as IForgeable;
        return true;
    }
    private Item GetProductItem()
    {
        // get hashstring
        string hashString = GetProductHashString();

        // get hashcode and recipe
        int hashCode = hashString.GetHashCode();

        if (ResourceAssets.singleton.craftingRecipes.ContainsKey(hashCode))
        {
            byte recipeID = ResourceAssets.singleton.craftingRecipes[hashCode];
            Item prodItem = ResourceAssets.singleton.items[recipeID];

            if (IsProductionAuthorized(prodItem) && IsProductForgeable(prodItem))
            {
                return prodItem;
            }
            else
            {
                return null;
            }
        }

        return null;
    }
    private bool IsProductionAuthorized(Item prodItem)
    {
        return prodItem.craftAuth == CraftAuth.Furnance;
    }
    private bool IsProductForgeable(Item prodItem)
    {
        return prodItem is IForgeable;
    }
    public string GetProductHashString()
    {
        return Convert1x1CraftRecipeToHashString();
    }
    public string Convert1x1CraftRecipeToHashString()
    {
        InventorySlot slot = inventorySlots[0];
        Item item = ResourceAssets.singleton.items[slot.itemData.id];
        string result = "";
        if (IsSlotEmpty(slot))
        {
            result += "None";
        }
        else
        {
            result += item.itemName;
        }

        // fill 8 empty strings
        for (int i = 0; i < 8; i++)
        {
            result += "None";
        }

        result += CraftAuth.Furnance.ToString();

        return result;
    }
    #endregion


    #region Furnance UI Page
    public bool IsInventoryStateMatched(InventoryPageState state) 
    {
        return state == InventoryPageState.Furnance;
    }
    #endregion
}
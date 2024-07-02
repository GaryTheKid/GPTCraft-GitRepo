using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_Inventory : MonoBehaviour
{
    public PlayerInventoryController inventoryController;
    public PlayerEquipmentController equipmentController;
    public PlayerInteractionController interactionController;
    public PlayerWeaponController weaponController;
    public RectTransform equipHighlight;

    private UI_Inventory_Toolbar inventory_toolbar;
    private UI_Inventory_Backpack inventory_backpack;
    private UI_Inventory_BackpackToolbar inventory_backpackToolbar;
    private UI_Inventory_BackpackCraft inventory_backpackCraft;
    private UI_Inventory_BackpackCraftResult inventory_backpackCraftResult;
    private UI_Inventory_CraftingTable inventory_craftingTable;
    private UI_Inventory_CraftingTableResult inventory_craftingTableResult;
    private UI_Inventory_FurnanceSource inventory_furnanceSource;
    private UI_Inventory_FurnanceProduct inventory_furnanceProduct;
    private UI_Inventory_FurnanceFuel inventory_furnanceFuel;
    private List<UI_InventoryBase> allInventoryUIs;
    private Dictionary<Inventory, List<UI_InventoryBase>> inventoryUIMap;

    #region Initialization
    private void Awake()
    {
        // get inventory ui classes (only modify here)
        inventory_toolbar = GetComponent<UI_Inventory_Toolbar>();
        inventory_backpack = GetComponent<UI_Inventory_Backpack>();
        inventory_backpackToolbar = GetComponent<UI_Inventory_BackpackToolbar>();
        inventory_backpackCraft = GetComponent<UI_Inventory_BackpackCraft>();
        inventory_backpackCraftResult = GetComponent<UI_Inventory_BackpackCraftResult>();
        inventory_craftingTable = GetComponent<UI_Inventory_CraftingTable>();
        inventory_craftingTableResult = GetComponent<UI_Inventory_CraftingTableResult>();
        inventory_furnanceSource = GetComponent<UI_Inventory_FurnanceSource>();
        inventory_furnanceProduct = GetComponent<UI_Inventory_FurnanceProduct>();
        inventory_furnanceFuel = GetComponent<UI_Inventory_FurnanceFuel>();

        // add all inventory ui classes in the list
        allInventoryUIs = new List<UI_InventoryBase>();
        foreach (UI_InventoryBase ui_inventoryClass in GetComponents<UI_InventoryBase>())
        {
            allInventoryUIs.Add(ui_inventoryClass);

            // initialization
            ui_inventoryClass.InitializeSlots();
        }
    }
    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 初始化 (modify here)
        inventory_toolbar.SetInventory(inventoryController.Toolbar);
        inventory_backpack.SetInventory(inventoryController.Backpack);
        inventory_backpackToolbar.SetInventory(inventoryController.Toolbar);
        inventory_backpackCraft.SetInventory(inventoryController.BackpackCraft);
        inventory_backpackCraftResult.SetInventory(inventoryController.BackpackCraftResult);
        inventory_craftingTable.SetInventory(inventoryController.CraftingTableCraft);
        inventory_craftingTableResult.SetInventory(inventoryController.CraftingTableCraftResult);
        inventory_furnanceSource.SetInventory(inventoryController.FurnanceSource);
        inventory_furnanceProduct.SetInventory(inventoryController.FurnanceProduct);
        inventory_furnanceFuel.SetInventory(inventoryController.FurnanceFuel);

        // 绑定Inventory的UI更新事件
        inventoryController.OnInventoryUIUpdate.AddListener(UpdateInventoryUI);
        inventoryController.OnEquipmentUpdate.AddListener(UpdateEquipment);
        inventoryController.OnInventoryUIPageSwitchOnOff.AddListener(SwitchInventoryUIPageOnOff);
        inventoryController.OnUniversialUIPageSwitchOnOff.AddListener(SwitchUniversialUIPageOnOff);

        // 生成每个InventoryData Class对应的InventoryUI Class映射图
        inventoryUIMap = new Dictionary<Inventory, List<UI_InventoryBase>>();
        foreach (UI_InventoryBase ui_inventoryClass in allInventoryUIs)
        {
            Inventory inv = ui_inventoryClass.Inventory;
            if (inventoryUIMap.ContainsKey(inv))
            {
                inventoryUIMap[inv].Add(ui_inventoryClass);
            }
            else
            {
                List<UI_InventoryBase> newUIList = new List<UI_InventoryBase>();
                newUIList.Add(ui_inventoryClass);
                inventoryUIMap[inv] = newUIList;
            }
        }

        // 初始更新
        UpdateInventoryUI();
    }
    #endregion

    #region Events Binding
    private void OnEnable()
    {
        BindAllEvents();
    }
    private void OnDisable()
    {
        UnBindAllEvents();
    }
    private void BindAllEvents()
    {
        foreach (UI_InventoryBase ui_inventoryClass in GetComponents<UI_InventoryBase>())
        {
            ui_inventoryClass.BindEventsForSlots(
                TryMoveItems, 
                TryMoveOneItem, 
                RemoveAllAtIndex, 
                RemoveOneCraftingAreaRecipe,
                InterruptFurnanceProduction,
                HandleDragFailEvent
            );
        }
        equipmentController.SwitchEquipmentEvent += TryEquipItem;
        interactionController.OnConstructEvent += RemoveOneItemAtIndex;
        interactionController.OnBlockDestroyEvent += UpdateItemDurability;
        weaponController.OnMeleeAttackEvent += UpdateItemDurability;
    }
    private void UnBindAllEvents()
    {
        foreach (UI_InventoryBase ui_inventoryClass in GetComponents<UI_InventoryBase>())
        {
            ui_inventoryClass.UnBindEventsForSlots(
                TryMoveItems, 
                TryMoveOneItem, 
                RemoveAllAtIndex, 
                RemoveOneCraftingAreaRecipe,
                InterruptFurnanceProduction,
                HandleDragFailEvent
            );
        }
        equipmentController.SwitchEquipmentEvent -= TryEquipItem;
        interactionController.OnConstructEvent -= RemoveOneItemAtIndex;
        interactionController.OnBlockDestroyEvent -= UpdateItemDurability;
        weaponController.OnMeleeAttackEvent -= UpdateItemDurability;
    }
    #endregion

    #region UIs Update
    private void UpdateInventoryUI()
    {
        foreach (UI_InventoryBase ui_inventoryClass in GetComponents<UI_InventoryBase>())
        {
            ui_inventoryClass.UpdateSlotsUI(RemoveOneItemAtIndex);
        }
    }
    private void UpdateEquipment(int equipmentSlotIndex)
    {
        equipmentController.UpdateEquipment((byte)equipmentSlotIndex, TryEquipItem(equipmentSlotIndex));
    }
    private void UpdateEquipmentHighlightUI(int toolbarIndex)
    {
        equipHighlight.SetParent(inventory_toolbar.ToolbarEquipmentAnchors[toolbarIndex]);
        equipHighlight.anchoredPosition = Vector2.zero;
    }
    private void ResetAllUIInteractorHighlights()
    {
        foreach (UI_InventoryBase ui_inventoryClass in GetComponents<UI_InventoryBase>())
        {
            ui_inventoryClass.ResetAllInteractorHighlights();
        }
    }
    #endregion

    #region Functions
    private void SwitchInventoryUIPageOnOff(PlayerStats stats, Inventory inv)
    {
        foreach (UI_InventoryBase inventoryUI in inventoryUIMap[inv])
        {
            if (!(inventoryUI is IUIPage)) continue;

            IUIPage uiPage = inventoryUI as IUIPage;
            if (uiPage.IsPageActive())
            {
                // switch off
                uiPage.PageOff();
                CursorOff();
                stats.STATE_invPageState = InventoryPageState.None;
                inventory_toolbar.BindEventsForSlots(
                    TryMoveItems, 
                    TryMoveOneItem, 
                    RemoveAllAtIndex, 
                    RemoveOneCraftingAreaRecipe,
                    InterruptFurnanceProduction,
                    HandleDragFailEvent
                );

                ResetAllUIInteractorHighlights();
            }
            else
            {
                // switch on
                if (stats.STATE_invPageState != InventoryPageState.None) return;

                uiPage.PageOn();
                CursorOn();
                stats.STATE_invPageState = uiPage.GetPageState();
                inventory_toolbar.UnBindEventsForSlots(
                    TryMoveItems, 
                    TryMoveOneItem, 
                    RemoveAllAtIndex, 
                    RemoveOneCraftingAreaRecipe,
                    InterruptFurnanceProduction,
                    HandleDragFailEvent
                );
            }
        }
    }
    private void SwitchUniversialUIPageOnOff(PlayerStats stats)
    {
        // 检测是否有界面打开，若有，关闭所有已打开界面
        if (stats.STATE_invPageState != InventoryPageState.None)
        {
            foreach (UI_InventoryBase ui_inventoryClass in allInventoryUIs)
            {
                if (!(ui_inventoryClass is IUIPage)) continue;

                IUIPage uiPage = ui_inventoryClass as IUIPage;
                if (uiPage.IsPageActive())
                {
                    // switch off
                    uiPage.PageOff();
                    CursorOff();
                    inventory_toolbar.BindEventsForSlots(
                        TryMoveItems,
                        TryMoveOneItem,
                        RemoveAllAtIndex,
                        RemoveOneCraftingAreaRecipe,
                        InterruptFurnanceProduction,
                        HandleDragFailEvent
                    );

                    ResetAllUIInteractorHighlights();
                }
            }

            stats.STATE_invPageState = InventoryPageState.None;
        }
        // 若无界面打开，打开背包
        else
        {
            foreach (UI_InventoryBase inventoryUI in inventoryUIMap[inventoryController.Backpack])
            {
                if (!(inventoryUI is IUIPage)) continue;

                IUIPage uiPage = inventoryUI as IUIPage;
                if (!uiPage.IsPageActive())
                { 
                    uiPage.PageOn();
                    CursorOn();
                    inventory_toolbar.UnBindEventsForSlots(
                        TryMoveItems,
                        TryMoveOneItem,
                        RemoveAllAtIndex,
                        RemoveOneCraftingAreaRecipe,
                        InterruptFurnanceProduction,
                        HandleDragFailEvent
                    );
                }
            }

            stats.STATE_invPageState = InventoryPageState.Backpack;
        }
    }
    private void CursorOn()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    private void CursorOff()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private ItemData TryEquipItem(int toolbarIndex)
    {
        InventorySlot[] toolbar = inventory_toolbar.Slots;

        if (toolbarIndex < 0 || toolbarIndex > toolbar.Length)
        {
            return null;
        }        

        if (toolbar[toolbarIndex] == null ||
            toolbar[toolbarIndex].itemData == null ||
            toolbar[toolbarIndex].stackCount == 0)
        {
            UpdateEquipmentHighlightUI(toolbarIndex);
            return DefaultStats.EQUIPMENT_equippedItem;
        }

        Item item = ResourceAssets.singleton.items[toolbar[toolbarIndex].itemData.id];
        if (!(item is IEquipable))
        {
            return null;
        }

        UpdateEquipmentHighlightUI(toolbarIndex);
        return toolbar[toolbarIndex].itemData;
    }
    private void TryMoveItems(int sourceIndex, int destinationIndex)
    {
        inventoryController.MoveItemInInventory(sourceIndex, destinationIndex);
    }
    private int TryMoveOneItem(int sourceIndex, int destinationIndex)
    {
        return inventoryController.MoveItemInInventory(sourceIndex, destinationIndex, 1);
    }
    private void HandleDragFailEvent(int index)
    {
        inventoryController.HandleDragFail(index);
    }
    private void RemoveAllAtIndex(int index)
    {
        inventoryController.RemoveAllItemsByIndex(index, true);
    }
    private void RemoveOneItemAtIndex(int index)
    {
        inventoryController.RemoveItemByIndex(index, 1, false);
    }
    private void RemoveOneCraftingAreaRecipe(int index) 
    {
        inventoryController.RemoveOneCraftingAreaRecipe(index);
    }
    private void InterruptFurnanceProduction(int index)
    {
        inventoryController.InterruptFurnanceProduction(index);
    }
    private void UpdateItemDurability(ItemData itemData, IDurable item, short changeAmount)
    {
        item.UpdateDurability(itemData, changeAmount);
        UpdateInventoryUI();
    }
    #endregion
}
using UnityEngine;
using UnityEngine.Events;

public enum InventoryPageState
{
    None,
    Backpack,
    CraftingTable,
    Furnance
}

public class PlayerInventoryController : MonoBehaviour
{
    public readonly static Inventory_Toolbar toolbar = new Inventory_Toolbar();
    public readonly static Inventory_Backpack backpack = new Inventory_Backpack();
    public readonly static Inventory_BackpackCraftArea backpackCraftArea = new Inventory_BackpackCraftArea();
    public readonly static Inventory_BackpackCraftResult backpackCraftResult = new Inventory_BackpackCraftResult(backpackCraftArea);
    public readonly static Inventory_CraftingTableCraftArea craftingTableCraftArea = new Inventory_CraftingTableCraftArea();
    public readonly static Inventory_CraftingTableCraftResult craftingTableCraftResult = new Inventory_CraftingTableCraftResult(craftingTableCraftArea);
    public readonly static Inventory_FurnanceProduct furnanceProduct = new Inventory_FurnanceProduct();
    public readonly static Inventory_FurnanceSource furnanceSource = new Inventory_FurnanceSource(furnanceProduct);
    public readonly static Inventory_FurnanceFuel furnanceFuel = new Inventory_FurnanceFuel(furnanceProduct);

    public Inventory_Toolbar Toolbar { get { return toolbar; } }
    public Inventory_Backpack Backpack { get { return backpack; } }
    public Inventory_BackpackCraftArea BackpackCraft { get { return backpackCraftArea; } }
    public Inventory_BackpackCraftResult BackpackCraftResult { get { return backpackCraftResult; } }
    public Inventory_CraftingTableCraftArea CraftingTableCraft { get { return craftingTableCraftArea; } }
    public Inventory_CraftingTableCraftResult CraftingTableCraftResult { get { return craftingTableCraftResult; } }
    public Inventory_FurnanceSource FurnanceSource { get { return furnanceSource; } }
    public Inventory_FurnanceProduct FurnanceProduct { get { return furnanceProduct; } }
    public Inventory_FurnanceFuel FurnanceFuel { get { return furnanceFuel; } }


    private readonly static Inventory[] inventorys = new Inventory[]
    {
        toolbar,
        backpack,
        backpackCraftArea,
        backpackCraftResult,
        craftingTableCraftArea,
        craftingTableCraftResult,
        furnanceSource,
        furnanceProduct,
        furnanceFuel
    };

    public UnityEvent<int> OnEquipmentUpdate = new UnityEvent<int>();
    public UnityEvent OnInventoryUIUpdate = new UnityEvent();
    public UnityEvent<PlayerStats, Inventory> OnInventoryUIPageSwitchOnOff = new UnityEvent<PlayerStats, Inventory>();
    public UnityEvent<PlayerStats> OnUniversialUIPageSwitchOnOff = new UnityEvent<PlayerStats>();

    private PlayerStats stats;

    #region Initialization
    private void Awake()
    {
        stats = GetComponent<PlayerStats>();

        foreach (Inventory inv in inventorys)
        {
            if (inv is IFurnanceProduct)
            {
                IFurnanceProduct furnanceProduct = inv as IFurnanceProduct;
                furnanceProduct.SetOnCompletionUIUpdate(UpdateInventory);
            }
        }
    }

    private void OnEnable()
    {
        BindInventoryEvents();
    }

    private void OnDisable()
    {
        UnbindInventoryEvents();
    }
    #endregion

    private void Update()
    {
        // ���ر���
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchUniversialUIPageOnOff();
        }
    }

    #region Inventory Data Manipulations
    public bool AddItemToInventory(ItemData itemData)
    {
        bool isAddingItemToToolbarSuccessful = toolbar.AddItem(itemData);
        if (isAddingItemToToolbarSuccessful)
        {
            UpdateInventory();
            return true;
        }

        bool isAddingItemToBackpackSuccessful = backpack.AddItem(itemData);
        if (isAddingItemToBackpackSuccessful)
        {
            UpdateInventory();
            return true;
        }

        // ����������ͱ��������ˣ�����false��ʾ���ʧ��
        if (!isAddingItemToToolbarSuccessful && !isAddingItemToBackpackSuccessful) PopItemObjToWorld(itemData, 1);

        return false;
    }
    public bool AddItemToInventory(ItemData itemData, int amount)
    {
        int amountLeft = amount;
        amountLeft = toolbar.AddItem(itemData, amountLeft);
        if (amountLeft == 0)
        {
            UpdateInventory();
            return true;
        }

        amountLeft = backpack.AddItem(itemData, amountLeft);
        if (amountLeft == 0)
        {
            UpdateInventory();
            return true;
        }

        // ����������ͱ��������ˣ��һ���δ����ӵ���Ʒʣ�࣬����false��ʾ���ʧ��
        if (amountLeft > 0) PopItemObjToWorld(itemData, amountLeft);

        return false;
    }
    public void RemoveItemFromInventory(ItemData itemData, bool isPopingItem)
    {
        bool isRemovingItemFromToolbarSuccessful = toolbar.RemoveItem(itemData);
        if (isRemovingItemFromToolbarSuccessful)
        {
            UpdateInventory();

            // ���ɾ���ɹ�������ƷΪ����ɾ��
            if (isPopingItem) PopItemObjToWorld(itemData, 1);

            return;
        }

        bool isRemovingItemFromBackpackSuccessful = backpack.RemoveItem(itemData);
        if (isRemovingItemFromBackpackSuccessful)
        {
            UpdateInventory();

            // ���ɾ���ɹ�������ƷΪ����ɾ��
            if (isPopingItem) PopItemObjToWorld(itemData, 1);

            return;
        }
    }
    public void RemoveItemByIndex(int index, int amount, bool isPopingItem)
    {
        Inventory inv = GetInventoryFromIndex(index);
        int localIndex = GetInventoryLocalIndex(index);

        bool isRemovingItemSuccessful = inv.RemoveItem(localIndex, amount, out ItemData slotItemData);
        if (isRemovingItemSuccessful)
        {
            UpdateInventory();

            // ���ɾ���ɹ�������ƷΪ����ɾ��
            if (isPopingItem) PopItemObjToWorld(slotItemData, amount);

            return;
        }
    }
    public void RemoveAllItemsByIndex(int index, bool isPopingItem)
    {
        Inventory inv = GetInventoryFromIndex(index);

        if (inv is ICraftResult) return;

        int localIndex = GetInventoryLocalIndex(index);

        bool isRemovingItemSuccessful = inv.RemoveAllSlotItems(localIndex, out ItemData slotItemData, out int slotItemAmount);
        if (isRemovingItemSuccessful)
        {
            UpdateInventory();

            // ���ɾ���ɹ�������ƷΪ����ɾ��
            if (isPopingItem) PopItemObjToWorld(slotItemData, slotItemAmount);

            return;
        }
    }
    public void MoveItemInInventory(int srcIndex, int desIndex)
    {
        Inventory srcInv = GetInventoryFromIndex(srcIndex);
        Inventory desInv = GetInventoryFromIndex(desIndex);

        if (desInv is ICraftResult) return;
        if (desInv is IFurnanceProduct) return;

        // ���Դ��Ŀ���Ƿ���ͬ
        if (srcIndex == desIndex) return;

        InventorySlot[] srcInvSlots = srcInv.GetInventorySlots;
        InventorySlot[] desInvSlots = desInv.GetInventorySlots;

        if (srcInvSlots == null || desInvSlots == null) return;

        int localSrcIndex = GetInventoryLocalIndex(srcIndex);
        int localDesIndex = GetInventoryLocalIndex(desIndex);
        InventorySlot srcSlot = srcInvSlots[localSrcIndex];
        InventorySlot desSlot = desInvSlots[localDesIndex];

        // ��Դ��Ŀ�궼��Ϊ�յ�����²��ƶ���Ʒ
        if (srcSlot != null && desSlot != null)
        {
            Item srcItem = ResourceAssets.singleton.items[srcSlot.itemData.id];
            Item desItem = ResourceAssets.singleton.items[desSlot.itemData.id];

            if (srcInv is IFurnanceProduct && (srcSlot == null || (desSlot != null && srcItem.id != desItem.id))) return;

            // ���������Ʒ���Ͳ�ͬ�����߲��ɶѵ�����ֱ�ӽ�����Ʒ��
            if (srcItem.id != desItem.id || !srcItem.isStackable || !desItem.isStackable)
            {
                desInvSlots[localDesIndex] = srcSlot;
                srcInvSlots[localSrcIndex] = desSlot;
            }
            else if (srcItem.id == desItem.id && srcItem.isStackable && desItem.isStackable)
            {
                // ���������Ʒ������ͬ�ҿɶѵ�����ϲ��ѵ�����
                int totalStack = srcSlot.stackCount + desSlot.stackCount;
                if (totalStack <= Inventory.SIZE_MAXSTACKSIZE) // ���� maxStackSize �����ѵ�����
                {
                    desSlot.stackCount = totalStack;
                    srcInvSlots[localSrcIndex] = null;
                }
                else
                {
                    // ����ʣ��������Դ��Ʒ��
                    desSlot.stackCount = Inventory.SIZE_MAXSTACKSIZE;
                    srcSlot.stackCount = totalStack - Inventory.SIZE_MAXSTACKSIZE;
                }
            }
        }
        else
        {
            // ֱ�ӽ�����Ʒ��
            desInvSlots[localDesIndex] = srcSlot;
            srcInvSlots[localSrcIndex] = desSlot;
        }

        // ������Ʒ�ϳ���
        UpdateCraftingArea();
        UpdateFurnanceProduction();

        UpdateInventory();
    }
    public int MoveItemInInventory(int srcIndex, int desIndex, int amountToMove)
    {
        Inventory srcInv = GetInventoryFromIndex(srcIndex);
        Inventory desInv = GetInventoryFromIndex(desIndex);

        if (srcInv is ICraftResult) return -1;
        if (desInv is ICraftResult) return 0;
        if (srcInv is IFurnanceProduct) return -1;
        if (desInv is IFurnanceProduct) return 0;

        // ���Դ��Ŀ���Ƿ���ͬ
        if (srcIndex == desIndex) return 0;

        InventorySlot[] srcInvSlots = srcInv.GetInventorySlots;
        InventorySlot[] desInvSlots = desInv.GetInventorySlots;

        if (srcInvSlots == null || desInvSlots == null) return 0;

        int localSrcIndex = GetInventoryLocalIndex(srcIndex);
        int localDesIndex = GetInventoryLocalIndex(desIndex);
        InventorySlot srcSlot = srcInvSlots[localSrcIndex];
        InventorySlot desSlot = desInvSlots[localDesIndex];

        // ��Դ��Ϊ�յ�����²��ƶ���Ʒ
        if (srcSlot == null) return 0;

        int srcAmount = srcSlot.stackCount;
        Item srcItem = ResourceAssets.singleton.items[srcSlot.itemData.id];

        // ���Դ��Ʒ���಻�ɶѵ���ֱ�ӽ����ò���
        if (!srcItem.isStackable) return 0;

        // Ŀ�����Ϊ�գ�ֱ�ӽ����ò���
        if (desSlot == null)
        {
            desInvSlots[localDesIndex] = new InventorySlot(srcSlot.itemData);
            desSlot = desInvSlots[localDesIndex];
            desSlot.stackCount = 0;
        }

        int desAmount = desSlot.stackCount;
        Item desItem = ResourceAssets.singleton.items[desSlot.itemData.id];

        // �����¼��ж�
        if (srcInv is IFurnanceProduct && (srcSlot == null || (desSlot != null && srcItem.id != desItem.id))) return 0;

        // ���Դ��Ŀ�����Ʒ���಻ͬ�򲻿ɶѵ���ֱ�ӽ����ò���
        if (srcItem.id != desItem.id || !desItem.isStackable) return 0;

        int transferAmount = 0;
        while (srcAmount > 0 && desAmount < Inventory.SIZE_MAXSTACKSIZE && transferAmount < amountToMove)
        {
            srcAmount--;
            desAmount++;
            transferAmount++;
        }

        srcSlot.stackCount = srcAmount;
        desSlot.stackCount = desAmount;

        // ����Ƿ���0�����У���Ϊ�ո�
        if (srcSlot.stackCount <= 0) srcInvSlots[localSrcIndex] = null;
        if (desSlot.stackCount <= 0) desInvSlots[localDesIndex] = null;

        // ������Ʒ�ϳ���
        UpdateCraftingArea();
        UpdateFurnanceProduction();

        UpdateInventory();

        return srcSlot.stackCount;
    }
    public void RemoveOneCraftingAreaRecipe(int index)
    {
        Inventory inv = GetInventoryFromIndex(index);
        if (!(inv is ICraftResult)) return;

        ICraftResult craftResult = inv as ICraftResult;
        if (craftResult.IsCraftResultSlotEmpty()) return;

        ICraftArea craftArea = craftResult.GetBindedCraftAreaInventory();
        craftArea.RemoveOneElementForEachSlot();

        UpdateInventory();
    }
    #endregion


    #region Global Update for Inventories
    private void UpdateInventory()
    {
        // ����UI�Ĵ���...
        OnEquipmentUpdate.Invoke(stats.EQUIPMENT_toolbarIndex);

        // ����UI�����¼�
        OnInventoryUIUpdate.Invoke();

        // ����Inventory�Ƿ�����״̬
        UpdateInventoryFullState();
    }
    private void UpdateInventoryFullState()
    {
        if (toolbar.IsInventoryFull() && backpack.IsInventoryFull()) stats.FLAG_isInventoryFull = true;
        else stats.FLAG_isInventoryFull = false;
    }
    private void UpdateCraftingArea()
    {
        ICraftResult craftResult = null;
        foreach (Inventory inv in inventorys)
        {
            if (inv is ICraftResult)
            {
                craftResult = inv as ICraftResult;
                if (craftResult.IsInventoryStateMatched(stats.STATE_invPageState)) break;
                else craftResult = null;
            }
        }

        if (craftResult == null) return;

        craftResult.ClearAllCraftResults();
        craftResult.UpdateCraftResult();
    }
    private void UpdateFurnanceProduction()
    {
        IFurnanceSource furnanceSource = null;
        foreach (Inventory inv in inventorys)
        {
            if (inv is IFurnanceSource)
            {
                furnanceSource = inv as IFurnanceSource;
                if (furnanceSource.IsInventoryStateMatched(stats.STATE_invPageState)) break;
                else furnanceSource = null;
            }
        }

        IFurnanceFuel furnanceFuel = null;
        foreach (Inventory inv in inventorys)
        {
            if (inv is IFurnanceFuel)
            {
                furnanceFuel = inv as IFurnanceFuel;
                if (furnanceFuel.IsInventoryStateMatched(stats.STATE_invPageState)) break;
                else furnanceFuel = null;
            }
        }

        if (furnanceSource == null || furnanceFuel == null) return;

        furnanceFuel.LoadFuel();
        furnanceSource.LoadSource();

        furnanceFuel.InitializeCombustion();
        furnanceSource.InitializeProduction();
    }
    #endregion


    #region External Interaction
    private void BindInventoryEvents()
    {
        Block_Utensil_Furnace.OnBlockInteractEvent += SwitchFurnanceUIOnOff;
        Block_Utensil_CraftingTable.OnBlockInteractEvent += SwitchCraftingTableUIOnOff;
    }
    private void UnbindInventoryEvents()
    {
        Block_Utensil_Furnace.OnBlockInteractEvent -= SwitchFurnanceUIOnOff;
        Block_Utensil_CraftingTable.OnBlockInteractEvent -= SwitchCraftingTableUIOnOff;
    }
    private void PopItemObjToWorld(ItemData itemData, int amount)
    {
        if (amount <= 0 || itemData == null)
        {
            return;
        }

        WorldObjectSpawner.singleton.PopItemToDirection(transform.position, transform.forward, itemData, amount);
    }
    private void SwitchUniversialUIPageOnOff()
    {
        OnUniversialUIPageSwitchOnOff.Invoke(stats);
        ResetCraftingAreaOnInventoryUISwitchOff();
    }
    private void SwitchInventoryUIPageOnOff(Inventory inv)
    {
        OnInventoryUIPageSwitchOnOff.Invoke(stats, inv);
        ResetCraftingAreaOnInventoryUISwitchOff();
    }
    public void SwitchFurnanceUIOnOff()
    {
        OnInventoryUIPageSwitchOnOff.Invoke(stats, furnanceSource);
        ResetCraftingAreaOnInventoryUISwitchOff();
    }
    public void SwitchCraftingTableUIOnOff()
    {
        OnInventoryUIPageSwitchOnOff.Invoke(stats, craftingTableCraftArea);
        ResetCraftingAreaOnInventoryUISwitchOff();
    }
    public void ResetCraftingAreaOnInventoryUISwitchOff()
    {
        foreach (Inventory inv in inventorys)
        {
            if (inv is ICraftResult)
            {
                ICraftResult craftResult = inv as ICraftResult;
                craftResult.ClearAllCraftResults();
            }

            if (inv is ICraftArea)
            {
                ICraftArea craftArea = inv as ICraftArea;
                craftArea.WithdrawAllCraftItems(AddItemToInventory);
            }
        }

        UpdateInventory();
    }
    public void HandleDragFail(int index)
    {
        Inventory inv = GetInventoryFromIndex(index);

        if (inv is IFurnanceSource)
        {
            (inv as IFurnanceSource).InitializeProduction();
            return;
        }

        if (!(inv is ICraftResult)) return;

        ICraftResult craftResult = inv as ICraftResult;
        if (craftResult.IsCraftResultSlotEmpty()) return;

        InventorySlot slot = craftResult.GetCraftResultSlot();
        AddItemToInventory(slot.itemData, slot.stackCount);
        craftResult.ClearAllCraftResults();

        UpdateCraftingArea();
        UpdateInventory();
    }
    public void InterruptFurnanceProduction(int index)
    {
        Inventory inv = GetInventoryFromIndex(index);
        if (!(inv is IFurnanceSource)) return;

        IFurnanceSource furnanceSource = inv as IFurnanceSource;
        furnanceSource.ResetProduction();
        UpdateInventory();
    }
    #endregion


    #region Utilities
    private Inventory GetInventoryFromIndex(int index)
    {
        foreach (var inv in inventorys)
        {
            if (index >= inv.idHead && index < inv.idHead + inv.size)
            {
                return inv;
            }
        }
        return null;
    }
    private InventorySlot GetSlotFromIndex(int index)
    {
        int localInventoryIndex = GetInventoryLocalIndex(index);

        foreach (var inv in inventorys)
        {
            if (index >= inv.idHead && index < inv.idHead + inv.size)
            {
                return inv.GetInventorySlots[localInventoryIndex];
            }
        }
        return null;
    }
    private void ClearSlotFromIndex(int index)
    {
        int localInventoryIndex = GetInventoryLocalIndex(index);

        foreach (var inv in inventorys)
        {
            if (index >= inv.idHead && index < inv.idHead + inv.size)
            {
                inv.GetInventorySlots[localInventoryIndex] = null;
                break;
            }
        }
    }
    private int GetInventoryLocalIndex(int index)
    {
        foreach (var inv in inventorys)
        {
            if (index >= inv.idHead && index < inv.idHead + inv.size)
            {
                return index - inv.idHead;
            }
        }
        return 0;
    }
    #endregion
}
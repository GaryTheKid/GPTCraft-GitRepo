using UnityEngine;

public abstract class Inventory : IInventory
{
    public const int SIZE_MAXSTACKSIZE = 64;

    public int size;
    public int idHead;
    protected InventorySlot[] inventorySlots;
    public InventorySlot[] GetInventorySlots { get { return inventorySlots; } }

    public Inventory(){}
    public Inventory(int size, int idHead)
    {
        this.size = size;
        this.idHead = idHead;
        this.inventorySlots = new InventorySlot[size];
    }

    public virtual bool AddItem(ItemData itemData)
    {
        Item item = ResourceAssets.singleton.items[itemData.id];

        // 先尝试添加到已有相同物品的物品槽
        for (int i = 0; i < size; i++)
        {
            if (inventorySlots[i] != null && inventorySlots[i].itemData.id == itemData.id && item.isStackable)
            {
                int totalStack = inventorySlots[i].stackCount + 1;
                if (totalStack <= SIZE_MAXSTACKSIZE)
                {
                    inventorySlots[i].stackCount = totalStack;
                    return true;
                }
            }
        }

        // 如果不可堆叠或没有已有相同物品的物品槽，则尝试添加到空的物品槽
        ItemData itemDataCopy = itemData.GetCopy();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = new InventorySlot(itemDataCopy);
                return true;
            }
        }

        // 如果工具栏和背包都满了，返回false表示添加失败
        return false;
    }
    public virtual int AddItem(ItemData itemData, int amount)
    {
        Item item = ResourceAssets.singleton.items[itemData.id];

        // 先尝试添加到已有相同物品的物品槽
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] != null && inventorySlots[i].itemData.id == itemData.id && item.isStackable)
            {
                int totalStack = inventorySlots[i].stackCount + amount;
                if (totalStack <= SIZE_MAXSTACKSIZE)
                {
                    inventorySlots[i].stackCount = totalStack;
                    return 0;
                }
                else
                {
                    int remaining = SIZE_MAXSTACKSIZE - inventorySlots[i].stackCount;
                    inventorySlots[i].stackCount = SIZE_MAXSTACKSIZE;
                    amount -= remaining;
                }
            }
        }

        // 添加剩余数量到空的物品槽
        ItemData itemDataCopy = itemData.GetCopy();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null && amount > 0)
            {
                int stackToAdd = Mathf.Min(amount, SIZE_MAXSTACKSIZE);
                inventorySlots[i] = new InventorySlot(itemDataCopy, stackToAdd);
                amount -= stackToAdd;
            }
        }

        return amount;
    }
    public virtual bool RemoveItem(ItemData itemData)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] != null && inventorySlots[i].itemData.id == itemData.id)
            {
                if (inventorySlots[i].stackCount > 1)
                {
                    // 如果堆叠数量大于1，减少堆叠数量
                    inventorySlots[i].stackCount--;
                }
                else
                {
                    // 否则直接移除物品槽
                    inventorySlots[i] = null;
                }

                // 移除成功
                return true;
            }
        }

        // 移除失败
        return false;
    }
    public virtual bool RemoveItem(int index, int amount, out ItemData slotItemData)
    {
        if (inventorySlots[index] != null)
        {
            slotItemData = inventorySlots[index].itemData;

            if (amount >= inventorySlots[index].stackCount)
            {
                // 如果要删除的数量大于等于物品槽的数量，直接删除物品槽
                inventorySlots[index] = null;
                return true;
            }
            else
            {
                // 否则减少物品槽的堆叠数量
                inventorySlots[index].stackCount -= amount;
                return true;
            }
        }

        slotItemData = null;
        return false;
    }
    public virtual bool RemoveAllSlotItems(int index, out ItemData slotItemData, out int slotItemAmount)
    {
        if (inventorySlots[index] == null)
        {
            slotItemData = null;
            slotItemAmount = 0;
            return false;
        }
        else
        {
            slotItemData = inventorySlots[index].itemData;
            slotItemAmount = inventorySlots[index].stackCount;
            inventorySlots[index] = null;
            return true;
        }
    }
    public bool IsInventoryFull()
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot == null || slot.stackCount == 0 || slot.itemData == null)
            {
                return false;
            }
        }

        return true;
    }
    public InventorySlot GetItemSlot(int index)
    {
        // 获取物品的逻辑
        return inventorySlots[index];
    }

    protected bool IsSlotEmpty(InventorySlot slot)
    {
        return slot == null || slot.itemData == null || slot.stackCount == 0;
    }
    protected bool IsSlotStackFull(InventorySlot slot)
    {
        if (IsSlotEmpty(slot)) return false;

        return slot.stackCount >= SIZE_MAXSTACKSIZE;
    }
}
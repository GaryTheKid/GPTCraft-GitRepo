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

        // �ȳ�����ӵ�������ͬ��Ʒ����Ʒ��
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

        // ������ɶѵ���û��������ͬ��Ʒ����Ʒ�ۣ�������ӵ��յ���Ʒ��
        ItemData itemDataCopy = itemData.GetCopy();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = new InventorySlot(itemDataCopy);
                return true;
            }
        }

        // ����������ͱ��������ˣ�����false��ʾ���ʧ��
        return false;
    }
    public virtual int AddItem(ItemData itemData, int amount)
    {
        Item item = ResourceAssets.singleton.items[itemData.id];

        // �ȳ�����ӵ�������ͬ��Ʒ����Ʒ��
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

        // ���ʣ���������յ���Ʒ��
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
                    // ����ѵ���������1�����ٶѵ�����
                    inventorySlots[i].stackCount--;
                }
                else
                {
                    // ����ֱ���Ƴ���Ʒ��
                    inventorySlots[i] = null;
                }

                // �Ƴ��ɹ�
                return true;
            }
        }

        // �Ƴ�ʧ��
        return false;
    }
    public virtual bool RemoveItem(int index, int amount, out ItemData slotItemData)
    {
        if (inventorySlots[index] != null)
        {
            slotItemData = inventorySlots[index].itemData;

            if (amount >= inventorySlots[index].stackCount)
            {
                // ���Ҫɾ�����������ڵ�����Ʒ�۵�������ֱ��ɾ����Ʒ��
                inventorySlots[index] = null;
                return true;
            }
            else
            {
                // ���������Ʒ�۵Ķѵ�����
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
        // ��ȡ��Ʒ���߼�
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
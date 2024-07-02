using System;

public abstract class Inventory_CraftArea : Inventory, ICraftArea
{
    public Inventory_CraftArea(int size, int idHead)
    {
        this.size = size;
        this.idHead = idHead;
        this.inventorySlots = new InventorySlot[size];
    }

    public void WithdrawAllCraftItems(Func<ItemData, int, bool> WithdrawItemFunc)
    {
        // move all in the (crafting area) to (toolbar or backpack)
        for (int i = 0; i < size; i++)
        {
            if (inventorySlots[i] == null) continue;

            WithdrawItemFunc(inventorySlots[i].itemData, inventorySlots[i].stackCount);
            inventorySlots[i] = null;
        }
    }

    public void RemoveOneElementForEachSlot()
    {
        // �������� backpackCraft slot
        for (int i = 0; i < size; i++)
        {
            InventorySlot slot = inventorySlots[i];

            // ��� slot ��Ϊ�������������㣬���������
            if (slot != null && slot.stackCount > 0)
            {
                // ��������
                slot.stackCount--;

                // �������Ϊ�㣬����ո� slot
                if (slot.stackCount <= 0)
                {
                    inventorySlots[i] = null;
                }
            }
        }
    }

    public abstract string GetCraftHashString();

    protected void FindStartRowAndColumn(InventorySlot[,] inventoryMatrix, out int startRow, out int startCol)
    {
        startRow = 0;
        startCol = 0;
        for (int row = 0; row < 3; row++)
        {
            bool rowHasItem = false;
            for (int col = 0; col < 3; col++)
            {
                if (inventoryMatrix[row, col] != null)
                {
                    rowHasItem = true;
                    break;
                }
            }
            if (rowHasItem)
            {
                startRow = row;
                break;
            }
        }

        for (int col = 0; col < 3; col++)
        {
            bool colHasItem = false;
            for (int row = 0; row < 3; row++)
            {
                if (inventoryMatrix[row, col] != null)
                {
                    colHasItem = true;
                    break;
                }
            }
            if (colHasItem)
            {
                startCol = col;
                break;
            }
        }
    }

    protected InventorySlot[,] TrimMatrix(InventorySlot[,] inventoryMatrix)
    {
        int startRow, startCol;
        FindStartRowAndColumn(inventoryMatrix, out startRow, out startCol);

        // ���Ʒǿյľ��β��ֵ��¾�����
        InventorySlot[,] trimmedMatrix = new InventorySlot[3, 3];
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int oldRow = row + startRow;
                int oldCol = col + startCol;

                if (oldRow < 3 && oldCol < 3)
                {
                    trimmedMatrix[row, col] = inventoryMatrix[oldRow, oldCol];
                }
                else
                {
                    trimmedMatrix[row, col] = null;
                }
            }
        }

        return trimmedMatrix;
    }
}

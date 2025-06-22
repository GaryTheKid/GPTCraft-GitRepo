using UnityEngine;

public class Inventory_BackpackCraftArea : Inventory_CraftArea, I2x2CraftArea
{
    public const int INVENTORY_INDEX = 2;
    public const int SIZE = 4;
    public const int ID_HEAD = 36;

    public Inventory_BackpackCraftArea() : base(SIZE, ID_HEAD)
    {
        // ����������� inventory ���г�ʼ��
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }

    public override string GetCraftHashString()
    {
        return Convert2x2CraftRecipeToHashString();
    }

    public string Convert2x2CraftRecipeToHashString() 
    {
        // ����һ��3x3�ľ���
        InventorySlot[,] inventoryMatrix = new InventorySlot[3, 3];

        // ������� slots ��ǰ�ĸ�Ԫ����䵽�����ǰ������
        for (int i = 0; i < Mathf.Min(size, 4); i++)
        {
            int row = i / 2;
            int col = i % 2;
            inventoryMatrix[row, col] = inventorySlots[i];
        }

        inventoryMatrix = TrimMatrix(inventoryMatrix);

        // �����ַ�����ʾ
        string result = "";
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                InventorySlot slot = inventoryMatrix[row, col];

                if (slot == null || slot.itemData == null || slot.stackCount == 0)
                {
                    result += "None";
                }
                else
                {
                    result += ResourceAssets.singleton.items[slot.itemData.id].itemName;
                }
            }
        }

        result += CraftAuth.Hand.ToString();
        return result;
    }
}

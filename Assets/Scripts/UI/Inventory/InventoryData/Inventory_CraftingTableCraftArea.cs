using UnityEngine;

public class Inventory_CraftingTableCraftArea : Inventory_CraftArea, I3x3CraftArea
{
    public const int INVENTORY_INDEX = 4;
    public const int SIZE = 9;
    public const int ID_HEAD = 41;

    public Inventory_CraftingTableCraftArea() : base(SIZE, ID_HEAD)
    {
        // 可以在这里对 inventory 进行初始化
        size = SIZE;
        idHead = ID_HEAD;
        inventorySlots = new InventorySlot[size];
    }

    public override string GetCraftHashString()
    {
        return Convert3x3CraftRecipeToHashString();
    }

    public string Convert3x3CraftRecipeToHashString()
    {
        // 创建一个3x3的矩阵
        InventorySlot[,] inventoryMatrix = new InventorySlot[3, 3];

        // 将输入的 slots 填充到矩阵中
        for (int i = 0; i < Mathf.Min(size, 9); i++)
        {
            int row = i / 3;
            int col = i % 3;
            inventoryMatrix[row, col] = inventorySlots[i];
        }

        inventoryMatrix = TrimMatrix(inventoryMatrix);

        // 构建字符串表示
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

        result += CraftAuth.CraftingTable.ToString();
        return result;
    }
}

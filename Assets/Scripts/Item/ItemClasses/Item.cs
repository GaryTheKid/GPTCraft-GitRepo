using UnityEngine;

public enum CraftAuth
{
    Hand,
    CraftingTable,
    Furnance
}

public abstract class Item : ScriptableObject
{
    [Header("====== Identity ======")]
    public byte id;
    public string itemName;
    public Sprite itemIcon;
    public GameObject itemPrefab;

    [Space(25)]

    [Header("====== Attributes ======")]
    public bool isStackable;
    public byte itemSelfDestroyTime;

    [Space(25)]

    [Header("====== Craft ======")]
    public CraftAuth craftAuth; 
    public int craftingResultItemCount;
    public Item[] craftingRecipe_row1;
    public Item[] craftingRecipe_row2;
    public Item[] craftingRecipe_row3;
    
    // functions
    public abstract ItemData CreateItemData();

    public int GetCraftingRecipeHashCode()
    {
        // 创建一个3x3的矩阵
        Item[,] craftingMatrix = new Item[3, 3];
        ConvertRowsToMatrixRecipe(craftingMatrix);
        craftingMatrix = TrimMatrix(craftingMatrix);

        string hashString = BuildHashString(craftingMatrix);
        return hashString.GetHashCode();
    }

    private void ConvertRowsToMatrixRecipe(Item[,] craftingMatrix)
    {
        // 将 craftingRecipe_row1、craftingRecipe_row2 和 craftingRecipe_row3 合并成一个矩阵
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;
            Item item = null;

            if (i < craftingRecipe_row1.Length)
            {
                item = craftingRecipe_row1[i];
            }
            else if (i < craftingRecipe_row1.Length + craftingRecipe_row2.Length)
            {
                item = craftingRecipe_row2[i - craftingRecipe_row1.Length];
            }
            else if (i < craftingRecipe_row1.Length + craftingRecipe_row2.Length + craftingRecipe_row3.Length)
            {
                item = craftingRecipe_row3[i - craftingRecipe_row1.Length - craftingRecipe_row2.Length];
            }

            craftingMatrix[row, col] = item;
        }
    }

    private void FindStartRowAndColumn(Item[,] craftingMatrix, out int startRow, out int startCol)
    {
        // 寻找旧矩阵的起始行和列
        startRow = 0;
        startCol = 0;
        for (int row = 0; row < 3; row++)
        {
            bool rowHasItem = false;
            for (int col = 0; col < 3; col++)
            {
                if (craftingMatrix[row, col] != null)
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
                if (craftingMatrix[row, col] != null)
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

    private Item[,] TrimMatrix(Item[,] craftingMatrix)
    {
        int startRow, startCol;
        FindStartRowAndColumn(craftingMatrix, out startRow, out startCol);

        // 复制非空的矩形部分到新矩阵中
        Item[,] trimmedMatrix = new Item[3, 3];
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int oldRow = row + startRow;
                int oldCol = col + startCol;

                if (oldRow < 3 && oldCol < 3)
                {
                    trimmedMatrix[row, col] = craftingMatrix[oldRow, oldCol];
                }
                else
                {
                    trimmedMatrix[row, col] = null;
                }
            }
        }

        return trimmedMatrix;
    }

    private string BuildHashString(Item[,] craftingMatrix)
    {
        string hashString = "";
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Item recipeItem = craftingMatrix[row, col];

                if (recipeItem == null)
                {
                    hashString += "None";
                }
                else
                {
                    hashString += recipeItem.itemName;
                }
            }
        }

        return hashString;
    }
}

public class ItemData
{
    public byte id;
    public short durability;

    public ItemData(){}

    public ItemData(byte id) 
    {
        this.id = id;
    }

    public ItemData (byte id, short durability)
    {
        this.id = id;
        this.durability = durability;
    }

    public ItemData GetCopy()
    {
        ItemData newItemData = new ItemData();
        newItemData.id = id;
        newItemData.durability = durability;

        return newItemData;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_CraftingTable", menuName = "ScriptableObject/Item/Block/CraftingTable")]
public class Item_Block_CraftingTable : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Utensil_CraftingTable.refID;
    }
}
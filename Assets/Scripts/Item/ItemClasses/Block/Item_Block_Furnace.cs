using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_Furnace", menuName = "ScriptableObject/Item/Block/Furnace")]
public class Item_Block_Furnace : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Utensil_Furnace.refID;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_Dirt", menuName = "ScriptableObject/Item/Block/Dirt")]
public class Item_Block_Dirt : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Nature_Dirt.refID;
    }
}
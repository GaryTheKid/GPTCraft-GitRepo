using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_Stone", menuName = "ScriptableObject/Item/Block/Stone")]
public class Item_Block_Stone : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Nature_Stone.refID;
    }
}
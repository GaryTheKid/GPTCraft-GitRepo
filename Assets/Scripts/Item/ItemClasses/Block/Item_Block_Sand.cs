using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_Sand", menuName = "ScriptableObject/Item/Block/Sand")]
public class Item_Block_Sand : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Nature_Sand.refID;
    }
}
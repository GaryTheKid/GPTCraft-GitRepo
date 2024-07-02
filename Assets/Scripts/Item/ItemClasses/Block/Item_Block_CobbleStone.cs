using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_CobbleStone", menuName = "ScriptableObject/Item/Block/CobbleStone")]
public class Item_Block_CobbleStone : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Construction_CobbleStone.refID;
    }
}
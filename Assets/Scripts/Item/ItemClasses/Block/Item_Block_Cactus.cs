using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_Cactus", menuName = "ScriptableObject/Item/Block/Cactus")]
public class Item_Block_Cactus : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Nature_Cactus.refID;
    }
}

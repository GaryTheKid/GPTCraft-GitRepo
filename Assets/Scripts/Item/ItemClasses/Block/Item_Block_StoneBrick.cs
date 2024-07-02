using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_StoneBrick", menuName = "ScriptableObject/Item/Block/StoneBrick")]
public class Item_Block_StoneBrick : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Construction_StoneBrick.refID;
    }
}
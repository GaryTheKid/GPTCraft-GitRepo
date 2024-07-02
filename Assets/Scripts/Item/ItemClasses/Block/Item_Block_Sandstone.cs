using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_Sandstone", menuName = "ScriptableObject/Item/Block/Sandstone")]
public class Item_Block_Sandstone : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Construction_Sandstone.refID;
    }
}
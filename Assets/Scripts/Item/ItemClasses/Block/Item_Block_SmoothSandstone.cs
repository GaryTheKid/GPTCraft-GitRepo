using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_SmoothSandstone", menuName = "ScriptableObject/Item/Block/SmoothSandstone")]
public class Item_Block_SmoothSandstone : Item_Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Construction_SmoothSandstone.refID;
    }
}
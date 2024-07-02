using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_WoodenPlank", menuName = "ScriptableObject/Item/Block/WoodenPlank")]
public class Item_Block_WoodenPlank : Item_Block, IFuel 
{
    [Space(25)]

    [Header("====== Fuel ======")]
    public float combustValue;

    public float GetCombustValue()
    {
        return combustValue;
    }

    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_Construction_WoodenPlank.refID;
    }
}
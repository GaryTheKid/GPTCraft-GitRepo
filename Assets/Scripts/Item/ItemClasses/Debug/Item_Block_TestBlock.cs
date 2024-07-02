using UnityEngine;

[CreateAssetMenu(fileName = "Item_Block_TestBlock", menuName = "ScriptableObject/Item/Block/TestBlock")]
public class Item_Block_TestBlock : Item_Block, IForgeable, IFuel
{
    [Space(25)]

    [Header("====== Forge ======")]
    public float forgeProductionTime;

    [Space(25)]

    [Header("====== Fuel ======")]
    public float combustValue;

    public float GetCombustValue()
    {
        return combustValue;
    }

    public int GetProductCount()
    {
        return craftingResultItemCount;
    }

    public float GetProductionTime()
    {
        return forgeProductionTime;
    }


    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetItemBlockID()
    {
        return Block_TestBlock.refID;
    }
}

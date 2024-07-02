using UnityEngine;

[CreateAssetMenu(fileName = "Item_Food_CookedPorkchop", menuName = "ScriptableObject/Item/Food/CookedPorkchop")]
public class Item_Food_CookedPorkchop : Item_Food, IForgeable
{
    [Space(25)]

    [Header("====== Forge ======")]
    public float forgeProductionTime;

    public static byte refID;

    public int GetProductCount()
    {
        return craftingResultItemCount;
    }

    public float GetProductionTime()
    {
        return forgeProductionTime;
    }

    private void OnEnable()
    {
        refID = id;
    }
}
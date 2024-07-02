using UnityEngine;

[CreateAssetMenu(fileName = "Item_Utensil_Charcoal", menuName = "ScriptableObject/Item/Utensil/Charcoal")]
public class Item_Utensil_Charcoal : Item_Utensil, IForgeable, IFuel
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
}
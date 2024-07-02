using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_WoodenShovel", menuName = "ScriptableObject/Item/Tool/WoodenShovel")]
public class Item_Tool_WoodenShovel : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
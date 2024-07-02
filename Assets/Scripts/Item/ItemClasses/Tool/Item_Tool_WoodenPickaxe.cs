using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_WoodenPickaxe", menuName = "ScriptableObject/Item/Tool/WoodenPickaxe")]
public class Item_Tool_WoodenPickaxe : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
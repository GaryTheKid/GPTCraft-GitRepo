using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_WoodenHoe", menuName = "ScriptableObject/Item/Tool/WoodenHoe")]
public class Item_Tool_WoodenHoe : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
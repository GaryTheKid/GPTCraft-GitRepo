using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_StoneHoe", menuName = "ScriptableObject/Item/Tool/StoneHoe")]
public class Item_Tool_StoneHoe : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
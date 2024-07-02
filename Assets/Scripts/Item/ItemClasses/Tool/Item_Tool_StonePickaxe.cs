using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_StonePickaxe", menuName = "ScriptableObject/Item/Tool/StonePickaxe")]
public class Item_Tool_StonePickaxe : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
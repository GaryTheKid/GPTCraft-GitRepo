using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_StoneShovel", menuName = "ScriptableObject/Item/Tool/StoneShovel")]
public class Item_Tool_StoneShovel : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_TestTool", menuName = "ScriptableObject/Item/Tool/TestTool")]
public class Item_Tool_TestTool : Item_Tool
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

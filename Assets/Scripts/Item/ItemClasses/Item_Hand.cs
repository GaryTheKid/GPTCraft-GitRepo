using UnityEngine;

[CreateAssetMenu(fileName = "Item_Hand", menuName = "ScriptableObject/Item/Hand")]
public class Item_Hand : Item, IEquipable
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override ItemData CreateItemData()
    {
        return new ItemData(refID);
    }
}

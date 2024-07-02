using UnityEngine;

[CreateAssetMenu(fileName = "Item_Food_RawPorkChop", menuName = "ScriptableObject/Item/Food/RawPorkChop")]
public class Item_Food_RawPorkChop : Item_Food
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

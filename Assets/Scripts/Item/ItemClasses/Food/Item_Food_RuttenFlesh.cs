using UnityEngine;

[CreateAssetMenu(fileName = "Item_Food_RuttenFlesh", menuName = "ScriptableObject/Item/Food/RuttenFlesh")]
public class Item_Food_RuttenFlesh : Item_Food
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

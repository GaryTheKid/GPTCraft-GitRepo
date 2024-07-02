using UnityEngine;

[CreateAssetMenu(fileName = "Item_Utensil_String", menuName = "ScriptableObject/Item/Utensil/String")]
public class Item_Utensil_String : Item_Utensil
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

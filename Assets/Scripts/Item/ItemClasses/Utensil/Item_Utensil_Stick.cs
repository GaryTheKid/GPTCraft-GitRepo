using UnityEngine;

[CreateAssetMenu(fileName = "Item_Utensil_Stick", menuName = "ScriptableObject/Item/Utensil/Stick")]
public class Item_Utensil_Stick : Item_Utensil
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_WoodenSword", menuName = "ScriptableObject/Item/Weapon/WoodenSword")]
public class Item_Weapon_WoodenSword : Item_Weapon
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
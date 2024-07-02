using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_TestWeapon", menuName = "ScriptableObject/Item/Weapon/TestWeapon")]
public class Item_Weapon_TestWeapon : Item_Weapon
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

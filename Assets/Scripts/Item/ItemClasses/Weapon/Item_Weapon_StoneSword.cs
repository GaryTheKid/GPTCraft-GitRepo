using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_StoneSword", menuName = "ScriptableObject/Item/Weapon/StoneSword")]
public class Item_Weapon_StoneSword : Item_Weapon
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
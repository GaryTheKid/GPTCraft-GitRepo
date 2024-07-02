using UnityEngine;

[CreateAssetMenu(fileName = "Item_Tool_StoneAxe", menuName = "ScriptableObject/Item/Tool/StoneAxe")]
public class Item_Tool_StoneAxe : Item_Tool, IWeapon
{
    [Space(25)]

    [Header("====== Weapon Attributes ======")]
    public short attackDamage;
    public float attackSpeed;

    public short GetAttackDamage()
    {
        return attackDamage;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }



    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
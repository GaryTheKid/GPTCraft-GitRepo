using UnityEngine;

[CreateAssetMenu(fileName = "Item_Food_RuttenFlesh", menuName = "ScriptableObject/Item/Food/RuttenFlesh")]
public class Item_Food_RuttenFlesh : Item_Food
{
    public byte nutritionVariation_min;

    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }

    public override byte GetFoodNutritionVal()
    {
        return (byte)Random.Range(nutritionVariation_min, nutrition + 1);
    }
}

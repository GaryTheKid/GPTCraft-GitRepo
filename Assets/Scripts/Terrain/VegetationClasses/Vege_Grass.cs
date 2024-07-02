using UnityEngine;

[CreateAssetMenu(fileName = "Vege_Grass", menuName = "ScriptableObject/Vege/Grass")]
public class Vege_Grass : Vegetation
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
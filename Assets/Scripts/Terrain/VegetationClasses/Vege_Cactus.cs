using UnityEngine;

[CreateAssetMenu(fileName = "Vege_Cactus", menuName = "ScriptableObject/Vege/Cactus")]
public class Vege_Cactus : Vegetation
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
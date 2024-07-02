using UnityEngine;

[CreateAssetMenu(fileName = "Vege_OakTree", menuName = "ScriptableObject/Vege/OakTree")]
public class Vege_OakTree : Vegetation
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}
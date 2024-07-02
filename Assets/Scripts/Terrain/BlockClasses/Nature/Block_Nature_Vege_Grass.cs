using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_Vege_Grass", menuName = "ScriptableObject/Block/Nature/Vege_Grass")]
public class Block_Nature_Vege_Grass : Block_Nature, IBlock_Plant
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_Grass", menuName = "ScriptableObject/Block/Nature/Grass")]
public class Block_Nature_Grass : Block_Nature, IBlock_SandMud
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

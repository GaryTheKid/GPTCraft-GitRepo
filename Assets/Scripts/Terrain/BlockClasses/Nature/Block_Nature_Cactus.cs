using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_Cactus", menuName = "ScriptableObject/Block/Nature/Cactus")]
public class Block_Nature_Cactus : Block_Nature, IBlock_Plant
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

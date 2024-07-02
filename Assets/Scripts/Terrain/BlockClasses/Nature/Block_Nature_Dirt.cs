using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_Dirt", menuName = "ScriptableObject/Block/Nature/Dirt")]
public class Block_Nature_Dirt : Block_Nature, IBlock_SandMud
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

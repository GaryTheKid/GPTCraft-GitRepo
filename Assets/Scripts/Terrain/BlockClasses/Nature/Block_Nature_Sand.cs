using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_Sand", menuName = "ScriptableObject/Block/Nature/Sand")]
public class Block_Nature_Sand : Block_Nature, IBlock_SandMud
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

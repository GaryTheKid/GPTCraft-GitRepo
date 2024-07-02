using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_OakLeaf", menuName = "ScriptableObject/Block/Nature/OakLeaf")]
public class Block_Nature_OakLeaf : Block_Nature, IBlock_Plant
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

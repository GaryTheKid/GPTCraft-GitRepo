using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_Stone", menuName = "ScriptableObject/Block/Nature/Stone")]
public class Block_Nature_Stone : Block_Nature, IBlock_Stone
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

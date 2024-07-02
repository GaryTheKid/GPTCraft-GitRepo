using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_CobbleStone", menuName = "ScriptableObject/Block/Construction/CobbleStone")]
public class Block_Construction_CobbleStone : Block_Construction, IBlock_Stone
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

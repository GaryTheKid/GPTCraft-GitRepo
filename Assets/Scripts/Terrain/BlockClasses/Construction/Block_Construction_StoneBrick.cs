using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_StoneBrick", menuName = "ScriptableObject/Block/Construction/StoneBrick")]
public class Block_Construction_StoneBrick : Block_Construction, IBlock_Stone
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

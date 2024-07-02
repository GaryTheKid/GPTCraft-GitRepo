using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_Sandstone", menuName = "ScriptableObject/Block/Construction/Sandstone")]
public class Block_Construction_Sandstone : Block_Construction, IBlock_Stone
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

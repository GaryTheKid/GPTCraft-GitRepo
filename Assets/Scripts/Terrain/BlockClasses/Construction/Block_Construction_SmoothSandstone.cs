using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_SmoothSandstone", menuName = "ScriptableObject/Block/Construction/SmoothSandstone")]
public class Block_Construction_SmoothSandstone : Block_Construction, IBlock_Stone
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

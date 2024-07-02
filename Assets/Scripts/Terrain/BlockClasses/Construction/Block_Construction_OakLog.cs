using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_OakLog", menuName = "ScriptableObject/Block/Construction/OakLog")]
public class Block_Construction_OakLog : Block_Construction, IBlock_Wood
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

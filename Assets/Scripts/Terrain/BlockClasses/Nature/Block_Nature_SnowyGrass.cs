using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_SnowyGrass", menuName = "ScriptableObject/Block/Nature/SnowyGrass")]
public class Block_Nature_SnowyGrass : Block_Nature, IBlock_SandMud
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

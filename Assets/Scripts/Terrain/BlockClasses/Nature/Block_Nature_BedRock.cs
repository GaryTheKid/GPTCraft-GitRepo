using UnityEngine;

[CreateAssetMenu(fileName = "Block_Nature_BedRock", menuName = "ScriptableObject/Block/Nature/BedRock")]
public class Block_Nature_BedRock : Block_Nature, IBlock_Invincible
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

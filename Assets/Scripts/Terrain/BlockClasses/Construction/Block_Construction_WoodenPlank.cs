using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_WoodenPlank", menuName = "ScriptableObject/Block/Construction/WoodenPlank")]
public class Block_Construction_WoodenPlank : Block_Construction, IBlock_Wood
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}


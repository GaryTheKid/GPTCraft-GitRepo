using UnityEngine;

[CreateAssetMenu(fileName = "Block_Construction_WoodenSlab", menuName = "ScriptableObject/Block/Construction/WoodenSlab")]
public class Block_Construction_WoodenSlab : Block_Construction, IBlock_Wood
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}
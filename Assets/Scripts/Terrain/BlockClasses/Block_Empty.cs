using UnityEngine;

[CreateAssetMenu(fileName = "Block_Empty", menuName = "ScriptableObject/Block/Empty")]
public class Block_Empty : Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

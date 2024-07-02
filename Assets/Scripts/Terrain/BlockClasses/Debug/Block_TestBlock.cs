using UnityEngine;

[CreateAssetMenu(fileName = "Block_TestBlock", menuName = "ScriptableObject/Block/TestBlock")]
public class Block_TestBlock : Block
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}
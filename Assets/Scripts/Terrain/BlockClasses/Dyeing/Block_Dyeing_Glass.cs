using UnityEngine;

[CreateAssetMenu(fileName = "Block_Dyeing_Glass", menuName = "ScriptableObject/Block/Dyeing/Glass")]
public class Block_Dyeing_Glass : Block_Dyeing
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
        InitFaces();
    }
}

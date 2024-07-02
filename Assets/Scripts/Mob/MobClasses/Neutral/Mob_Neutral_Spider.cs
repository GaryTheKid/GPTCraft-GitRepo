using UnityEngine;

[CreateAssetMenu(fileName = "Mob_Neutral_Spider", menuName = "ScriptableObject/Mob/Neutral/Spider")]
public class Mob_Neutral_Spider : Mob_Neutral
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Mob_Passive_Pig", menuName = "ScriptableObject/Mob/Passive/Pig")]
public class Mob_Passive_Pig : Mob_Passive
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

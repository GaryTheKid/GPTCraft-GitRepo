using UnityEngine;

[CreateAssetMenu(fileName = "Mob_Hostile_Zombie", menuName = "ScriptableObject/Mob/Hostile/Zombie")]
public class Mob_Hostile_Zombie : Mob_Hostile
{
    public static byte refID;

    private void OnEnable()
    {
        refID = id;
    }
}

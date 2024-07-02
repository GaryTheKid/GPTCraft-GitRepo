using UnityEngine;

public abstract class Mob_Hostile : Mob, IAggressiveMob
{
    [Space(25)]

    [Header("====== Weapon Stats ======")]
    public byte attackDamage;
    public float attackSpeed;
    public float attackRange;

    public byte GetAttackDamage()
    {
        return attackDamage;
    }

    public float GetAttackRange()
    {
        return attackSpeed;
    }

    public float GetAttackSpeed()
    {
        return attackRange;
    }

    public override MobData CreateMobData()
    {
        return new MobData(id, maxHP, moveSpeed, turnSpeed, attackDamage, attackSpeed, attackRange);
    }
}

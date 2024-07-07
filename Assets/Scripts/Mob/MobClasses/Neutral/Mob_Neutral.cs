using UnityEngine;

public abstract class Mob_Neutral : Mob, IAggressiveMob
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
        MobData newMobData = new MobData(id, maxHP, moveSpeed, turnSpeed, attackDamage, attackSpeed, attackRange);
        newMobData.terrifiedMoveSpeed = moveSpeed * terrifiedSpeedBoostPercentage;
        newMobData.terrifiedturnSpeed = turnSpeed * terrifiedSpeedBoostPercentage;
        return newMobData;
    }
}

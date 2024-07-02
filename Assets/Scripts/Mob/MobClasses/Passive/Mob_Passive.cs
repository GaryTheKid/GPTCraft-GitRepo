public abstract class Mob_Passive : Mob
{
    public override MobData CreateMobData()
    {
        return new MobData(id, maxHP, moveSpeed, turnSpeed);
    }
}

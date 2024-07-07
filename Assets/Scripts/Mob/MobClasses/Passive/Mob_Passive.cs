public abstract class Mob_Passive : Mob
{
    public override MobData CreateMobData()
    {
        MobData newMobData = new MobData(id, maxHP, moveSpeed, turnSpeed);
        newMobData.terrifiedMoveSpeed = moveSpeed * terrifiedSpeedBoostPercentage;
        newMobData.terrifiedturnSpeed = turnSpeed * terrifiedSpeedBoostPercentage;
        return newMobData;

    }
}

using System.Collections.Generic;
using UnityEngine;

public enum MobSpawnTime
{
    Day,
    Night,
    FullTime
}

public abstract class Mob : ScriptableObject
{
    [Header("====== Identity ======")]
    public byte id;
    public string mobName;
    public GameObject mobPrefab;

    [Space(25)]

    [Header("====== Attributes ======")]
    public MobSpawnTime mobSpawnTime;
    public List<BiomeType> mobSpawnBiomes;
    public bool isSunlightSensitive;
    public Item mobDeathDropItem;

    [Space(25)]

    [Header("====== Base Stats -- Collision ======")]
    public int mobHeight;

    [Header("====== Base Stats -- Movement ======")]
    public float moveSpeed;
    public float turnSpeed;
    public float jumpForce;
    public int explorationRange;
    public float terrifiedSpeedBoostPercentage;

    [Header("====== Base Stats -- Survival ======")]
    public short maxHP;

    [Header("====== Base Stats -- State ======")]
    public float mobActivity; // 决定了Mob在闲置时游走/发呆的概率，越大越倾向游走
    public float idleStateLength;
    public float roamPerformTimer;
    public float roamStateLength;
    public float terrifiedPerformTimer;
    public float terrifiedStateLength;
    public float chasePerformTimer;
    

    // functions
    public abstract MobData CreateMobData();

    public string GetMobInfo()
    {
        return mobName;
    }
}

public class MobData
{
    public byte id;
    public short hp;
    public float moveSpeed;
    public float turnSpeed;
    public byte attackDamage;
    public float attackSpeed;
    public float attackRange;
    public bool isTerrified;
    public float terrifiedMoveSpeed;
    public float terrifiedturnSpeed;

    public MobData() { }

    public MobData(byte id)
    {
        this.id = id;
    }

    public MobData(byte id, short hp)
    {
        this.id = id;
        this.hp = hp;
    }

    public MobData(byte id, short hp, float moveSpeed, float turnSpeed)
    {
        this.id = id;
        this.hp = hp;
        this.moveSpeed = moveSpeed;
        this.turnSpeed = turnSpeed;
    }

    public MobData(byte id, short hp, float moveSpeed, float turnSpeed, byte attackDamage, float attackSpeed, float attackRange)
    {
        this.id = id;
        this.hp = hp;
        this.moveSpeed = moveSpeed;
        this.turnSpeed = turnSpeed;
        this.attackDamage = attackDamage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
    }

    public MobData GetCopy()
    {
        MobData newMobData = new MobData();
        newMobData.id = id;
        newMobData.hp = hp;
        newMobData.moveSpeed = moveSpeed;
        newMobData.turnSpeed = turnSpeed;
        newMobData.attackDamage = attackDamage;
        newMobData.attackSpeed = attackSpeed;
        newMobData.attackRange = attackRange;
        newMobData.isTerrified = false;

        return newMobData;
    }
}
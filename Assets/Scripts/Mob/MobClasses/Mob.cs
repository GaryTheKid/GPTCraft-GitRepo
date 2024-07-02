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
    public GameObject mobDeathDropItemPrefab;

    [Space(25)]

    [Header("====== Base Stats ======")]
    public float moveSpeed;
    public float turnSpeed;
    public float jumpForce;
    public int explorationRange;
    public int mobHeight;
    public short maxHP;

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

        return newMobData;
    }
}
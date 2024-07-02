using System.Collections.Generic;
using UnityEngine;

public enum BiomeType
{
    Plain,
    SnowyPlain,
    Desert,
    Forest
}

public abstract class Biome : ScriptableObject
{
    [Header("====== Identity ======")]
    public BiomeType biomeType;


    [Space(25)]


    [Header("====== Attributes ======")]
    public byte temperatureIndex;
    public byte humidityIndex;


    [Space(25)]


    [Header("====== Biome Vegetation ======")]
    public float vegeThreshold_small;
    public float vegeThreshold_big;
    public List<Vegetation> biomeVege_small;
    public List<Vegetation> biomeVege_big;

    // functions
    public abstract byte GetBlockIdByDepth(int depthY);

    public Vegetation GetRandomVege_Small()
    {
        if (biomeVege_small == null || biomeVege_small.Count == 0)
            return null;

        return biomeVege_small[Random.Range(0, biomeVege_small.Count)];
    }

    public Vegetation GetRandomVege_Big()
    {
        if (biomeVege_big == null || biomeVege_big.Count == 0)
            return null;

        return biomeVege_big[Random.Range(0, biomeVege_big.Count)];
    }

    public Vegetation GetRandomVege()
    {
        float randVal = Random.Range(0f, 1f);
        if (randVal > 0.5f)
        {
            return GetRandomVege_Small();
        }
        else
        {
            return GetRandomVege_Big();
        }
    }
}
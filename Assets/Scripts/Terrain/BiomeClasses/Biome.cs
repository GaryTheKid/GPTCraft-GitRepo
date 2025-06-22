using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public enum BiomeType
{
    Plain,
    SnowyPlain,
    Desert,
    Forest
}

public static class BiomeExtensions
{
    public static BiomeData ToBiomeData(this Biome biome)
    {
        return new BiomeData
        {
            biomeType = biome.biomeType,
            temperatureIndex = biome.temperatureIndex,
            humidityIndex = biome.humidityIndex,
            vegeThreshold_small = biome.vegeThreshold_small,
            vegeThreshold_big = biome.vegeThreshold_big,
            blockLayer0 = 1, // TEMP
            blockLayer1 = 2, // TEMP
            blockLayer2 = 3  // TEMP
        };
    }
}

public struct BiomeData
{
    public BiomeType biomeType;
    public byte temperatureIndex;
    public byte humidityIndex;
    public float vegeThreshold_small;
    public float vegeThreshold_big;

    public byte blockLayer0;
    public byte blockLayer1;
    public byte blockLayer2;

    public byte GetBlockIdByDepth(int depthY)
    {
        if (depthY == 0) return blockLayer0;
        if (depthY == 1) return blockLayer1;
        return blockLayer2;
    }
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

    // Abstract method to define block type by depth in derived classes
    public abstract byte GetBlockIdByDepth(int depthY);

    public Vegetation GetRandomVege_Small()
    {
        if (biomeVege_small == null || biomeVege_small.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, biomeVege_small.Count);
        return biomeVege_small[index];
    }

    public Vegetation GetRandomVege_Big()
    {
        if (biomeVege_big == null || biomeVege_big.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, biomeVege_big.Count);
        return biomeVege_big[index];
    }

    public Vegetation GetRandomVege()
    {
        float randVal = UnityEngine.Random.Range(0f, 1f);
        return randVal > 0.5f ? GetRandomVege_Small() : GetRandomVege_Big();
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ResourceAssets : MonoBehaviour
{
    [Header("====== Game Resources ======")]
    public List<Block> blockScriptableObjects;
    public List<Vegetation> vegetationScriptableObjects;
    public List<Biome> biomeScriptableObjects;
    public List<Item> itemScriptableObjects;
    public List<Mob> mobScriptableObjects;

    public Dictionary<byte, Block> blocks;
    public Dictionary<byte, Vegetation> vegetations;
    public Dictionary<BiomeType, Biome> biomes_byType;
    public Dictionary<(byte, byte), Biome> biomes_byIndices;
    public Dictionary<byte, Item> items;
    public Dictionary<int, byte> craftingRecipes;
    public Dictionary<byte, Mob> mobs;

    [Space(15)]
    [Header("====== UI Resources ======")]
    public Sprite ui_emptySlot;

    public static ResourceAssets singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }

        blocks = new Dictionary<byte, Block>();
        vegetations = new Dictionary<byte, Vegetation>();
        biomes_byType = new Dictionary<BiomeType, Biome>();
        biomes_byIndices = new Dictionary<(byte, byte), Biome>();
        items = new Dictionary<byte, Item>();
        craftingRecipes = new Dictionary<int, byte>();
        mobs = new Dictionary<byte, Mob>();

        foreach (Block blockSO in blockScriptableObjects)
        {
            blocks.Add(blockSO.id, blockSO);
        }

        foreach (Vegetation vegeSO in vegetationScriptableObjects)
        {
            vegetations.Add(vegeSO.id, vegeSO);
        }

        foreach (Biome biomeSO in biomeScriptableObjects)
        {
            biomes_byType.Add(biomeSO.biomeType, biomeSO);
            biomes_byIndices.Add((biomeSO.temperatureIndex, biomeSO.humidityIndex), biomeSO);
        }

        foreach (Item itemSO in itemScriptableObjects)
        {
            items.Add(itemSO.id, itemSO);
            if (itemSO.craftingResultItemCount > 0)
            {
                craftingRecipes.Add(itemSO.GetCraftingRecipeHashCode(), itemSO.id);
            } 
        }

        foreach (Mob mobSO in mobScriptableObjects)
        {
            mobs.Add(mobSO.id, mobSO);
        } 
    }

    public byte GetMob(MobSpawnTime mobSpawnTime, BiomeType mobBiomeType, byte mobMaxHeight, out bool isQualifiedMobExist)
    {
        print(mobSpawnTime.ToString() + " " + mobBiomeType.ToString() + " " + mobMaxHeight);

        List<Mob> selectedMobs = mobs.Where(mob => (mob.Value.mobSpawnTime == MobSpawnTime.FullTime || mob.Value.mobSpawnTime == mobSpawnTime)
                                                && mob.Value.mobSpawnBiomes.Contains(mobBiomeType)
                                                && mob.Value.mobHeight <= mobMaxHeight)
            .Select(mob => mob.Value)
            .ToList();

        print(selectedMobs.Count);

        if (selectedMobs == null || selectedMobs.Count == 0)
        {
            isQualifiedMobExist = false;
            return 0;
        }

        isQualifiedMobExist = true;
        return selectedMobs[Random.Range(0, selectedMobs.Count)].id;
    }
}
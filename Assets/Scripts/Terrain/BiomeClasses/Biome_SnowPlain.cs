using UnityEngine;

[CreateAssetMenu(fileName = "Biome_SnowPlain", menuName = "ScriptableObject/Biome/SnowPlain")]
public class Biome_SnowPlain : Biome
{
    private void OnEnable()
    {
        biomeType = BiomeType.SnowyPlain;
    }

    public override byte GetBlockIdByDepth(int depthY)
    {
        int randDirtDepth = Random.Range(3, 6);

        if (depthY == 0)
        {
            return Block_Nature_SnowyGrass.refID;
        }
        else if (depthY > 0 && depthY <= randDirtDepth) 
        {
            return Block_Nature_Dirt.refID;
        }

        return Block_Nature_Stone.refID;
    }
}
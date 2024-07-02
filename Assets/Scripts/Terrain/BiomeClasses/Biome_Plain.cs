using UnityEngine;

[CreateAssetMenu(fileName = "Biome_Plain", menuName = "ScriptableObject/Biome/Plain")]
public class Biome_Plain : Biome
{
    private void OnEnable()
    {
        biomeType = BiomeType.Plain;
    }

    public override byte GetBlockIdByDepth(int depthY)
    {
        int randDirtDepth = Random.Range(3, 6);

        if (depthY == 0)
        {
            return Block_Nature_Grass.refID;
        }
        else if (depthY > 0 && depthY <= randDirtDepth)
        {
            return Block_Nature_Dirt.refID;
        }

        return Block_Nature_Stone.refID;
    }
}
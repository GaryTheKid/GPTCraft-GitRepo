using UnityEngine;

[CreateAssetMenu(fileName = "Biome_Desert", menuName = "ScriptableObject/Biome/Desert")]
public class Biome_Desert : Biome
{
    private void OnEnable()
    {
        biomeType = BiomeType.Desert;
    }

    public override byte GetBlockIdByDepth(int depthY)
    {
        int randSandDepth = Random.Range(2, 4);
        int randDirtDepth = Random.Range(randSandDepth + 1, 6);

        if (depthY <= randSandDepth)
        {
            return Block_Nature_Sand.refID;
        }
        else if (depthY > randSandDepth && depthY <= randDirtDepth)
        {
            return Block_Nature_Dirt.refID;
        }

        return Block_Nature_Stone.refID;
    }
}
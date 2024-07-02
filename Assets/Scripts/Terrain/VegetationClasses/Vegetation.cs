using UnityEngine;

public enum VegetationSize
{
    Small,
    Big
}

public abstract class Vegetation : ScriptableObject
{
    [Header("====== Identity ======")]
    public byte id;
    public string vegeName;
    public VegetationSize vegeSize;


    [Space(25)]


    [Header("====== Vegetation Blocks ======")]
    public VegeBlock[] vegeBlocks;
}

[System.Serializable]
public class VegeBlock
{
    public Vector3Int pos;
    public byte blockID;

    public VegeBlock(Vector3Int pos, byte blockID)
    {
        this.pos = pos;
        this.blockID = blockID;
    }
}
public enum BlockDir 
{
    TopFront,
    TopLeft,
    TopRight,
    TopBack,
    BottomFront,
    BottomLeft,
    BottomRight,
    BottomBack,
}

public struct TerrainBlockData
{
    public Byte3 blockCoord;
    public byte blockType;
    public BlockDir blockDir;
    public byte blockState;

    // constructor func
    public void ConstructBlock(byte x, byte y, byte z, byte blockType)
    {
        blockCoord = new Byte3(x, y, z);
        this.blockType = blockType;
    }

    public void ConstructBlock(byte x, byte y, byte z, byte blockType, BlockDir blockDir)
    {
        blockCoord = new Byte3(x, y, z);
        this.blockType = blockType;
        this.blockDir = blockDir;
    }

    public void ConstructBlock(byte x, byte y, byte z, byte blockType, byte blockState)
    {
        blockCoord = new Byte3(x, y, z);
        this.blockType = blockType;
        this.blockState = blockState;
    }

    public void ConstructBlock(byte x, byte y, byte z, byte blockType, BlockDir blockDir, byte blockState)
    {
        blockCoord = new Byte3(x, y, z);
        this.blockType = blockType;
        this.blockDir = blockDir;
        this.blockState = blockState;
    }

    public bool IsBlockEmpty()
    {
        return blockType == Block_Empty.refID;
    }

    public void DestroyBlock()
    {
        blockType = Block_Empty.refID;
    }
}
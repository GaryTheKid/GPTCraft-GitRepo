using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class TerrainManager : MonoBehaviour
{
    // singleton
    public static TerrainManager singleton;

    // terrain generation
    [Header("====== World Config ======")]
    [Header("World Loading")]
    public GameObject player;
    public int loadDistanceX;
    public int loadDistanceY;
    public int loadDistanceZ;
    public int chunksPerFrame;
    private Vector3Int lastPlayerChunkPosition;
    private int chunkLoadCounter;
    public GameObject terrainChunkPrefab;

    [Header("World Size")]
    public int worldChunkSizeX; // 游戏世界的尺寸 X (Chunk)
    public int worldChunkSizeY; // 游戏世界的尺寸 Y (个Chunk)
    public int worldChunkSizeZ; // 游戏世界的尺寸 Z (个Chunk)
    public byte chunkSizeX; // 每个TerrainChunk的尺寸
    public byte chunkSizeY; // 每个TerrainChunk的尺寸
    public byte chunkSizeZ; // 每个TerrainChunk的尺寸


    [Space(25)]


    // terrain generation
    [Header("====== Terrain Generation ======")]
    [Header("Surface Generation")]
    public float scale_C;
    public float scale_E;
    public float scale_PV;
    public AnimationCurve ContinentalnessCurve;
    public AnimationCurve ErosionCurve;
    public AnimationCurve PeaksAndValleysCurve;
    public byte noiseDisplacementOffset;
    public byte baseHeight;

    [Header("Basic Density")]
    public AnimationCurve DensitySquashCurve;
    public AnimationCurve DensityScaleCurve;

    [Header("Biome")]
    public float scale_T;
    public float scale_H;
    public AnimationCurve TemperatureCurve;
    public AnimationCurve HumidityCurve;

    [Header("Vegetation")]
    public float vegeGenerationScale_Small;
    public float vegeGenerationScale_Large;

    // data
    public Dictionary<Vector3Int, TerrainChunk> allTerrainChunks;
    public Dictionary<Vector2Int, int[,]> allTerrainChunkSurfaceHeights;
    public Dictionary<Vector2Int, Biome[,]> allTerrainChunkBiomes;
    public Dictionary<Vector3Int, List<VegeBlock>> crossChunkVegeBuffer;
    public HashSet<Vector3Int> activeChunkPositions;

    private ResourceAssets resourceAssets;
    private HashSet<Vector3Int> chunksToLoad = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> chunksToUnload = new HashSet<Vector3Int>();
    private IEnumerator currentChunkUpdateCoroutine = null;

    private Dictionary<byte, Block> blockDictRef;
    private Dictionary<(byte, byte), Biome> biomeDictRef_byIndices;

    public enum Dir
    {
        Front,
        Right,
        Back,
        Left,
        Top,
        Bottom
    }

    public readonly Dictionary<Dir, Vector3Int> DirVectors = new Dictionary<Dir, Vector3Int>
    {
        { Dir.Front, new Vector3Int(1, 0, 0) },     // 前 (x 轴正方向)
        { Dir.Right, new Vector3Int(0, 0, 1) },    // 右 (z 轴正方向)
        { Dir.Back, new Vector3Int(-1, 0, 0) },     // 背 (x 轴负方向)
        { Dir.Left, new Vector3Int(0, 0, -1) },      // 左 (z 轴负方向)
        { Dir.Top, new Vector3Int(0, 1, 0) },       // 顶 (y 轴正方向)
        { Dir.Bottom, new Vector3Int(0, -1, 0) },   // 底 (y 轴负方向)
    };

    public readonly Dictionary<Vector3, BlockDir> BlockDirs = new Dictionary<Vector3, BlockDir>
    {
        { new Vector3(1, 0, 0), BlockDir.TopFront },
        { new Vector3(1, 1, 0), BlockDir.TopFront },
        { new Vector3(0, 0, -1), BlockDir.TopLeft },
        { new Vector3(0, 1, -1), BlockDir.TopLeft },
        { new Vector3(0, 0, 1), BlockDir.TopRight },
        { new Vector3(0, 1, 1), BlockDir.TopRight },
        { new Vector3(-1, 0, 0), BlockDir.TopBack },
        { new Vector3(-1, 1, 0), BlockDir.TopBack },
        { new Vector3(1, -1, 0), BlockDir.BottomFront },
        { new Vector3(0, -1, -1), BlockDir.BottomLeft },
        { new Vector3(0, -1, 1), BlockDir.BottomRight },
        { new Vector3(-1, -1, 0), BlockDir.BottomBack },
    };

    public readonly Dictionary<BlockDir, Vector3> BlockDirVectors = new Dictionary<BlockDir, Vector3>
    {
        { BlockDir.TopFront, new Vector3(1, 1, 0) },
        { BlockDir.TopLeft, new Vector3(0, 1, -1) },
        { BlockDir.TopRight, new Vector3(0, 1, 1) },
        { BlockDir.TopBack, new Vector3(-1, 1, 0) },
        { BlockDir.BottomFront, new Vector3(1, -1, 0) },
        { BlockDir.BottomLeft, new Vector3(0, -1, -1) },
        { BlockDir.BottomRight, new Vector3(0, -1, 1) },
        { BlockDir.BottomBack, new Vector3(-1, -1, 0) },
    };

    #region Unity Functions
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }
    void Start()
    {
        resourceAssets = ResourceAssets.singleton;
        blockDictRef = ResourceAssets.singleton.blocks;
        biomeDictRef_byIndices = ResourceAssets.singleton.biomes_byIndices;
        StartCoroutine(GenerateInitialTerrainCoroutine());
    }
    void Update()
    {
        if (!player.activeInHierarchy)
        {
            return;
        }

        Vector3Int playerChunkPosition = CalculatePlayerChunkPosition();
        HandleChunkDynamicLoading(playerChunkPosition);
    }
    #endregion


    #region Dynamic Chunk Loading
    public void HandleChunkDynamicLoading(Vector3Int playerChunkPosition)
    {
        if (playerChunkPosition != lastPlayerChunkPosition)
        {
            UpdateChunksToLoadAndUnload(playerChunkPosition);
            if (currentChunkUpdateCoroutine != null)
            {
                StopCoroutine(currentChunkUpdateCoroutine);
            }
            currentChunkUpdateCoroutine = UpdateChunkLoadingCoroutine();
            StartCoroutine(currentChunkUpdateCoroutine);

            lastPlayerChunkPosition = playerChunkPosition;
        }
    }
    private void UpdateChunksToLoadAndUnload(Vector3Int playerChunkPosition)
    {
        chunksToLoad.Clear();
        chunksToUnload.Clear();

        // 确定需要加载的区块
        for (int x = -loadDistanceX; x <= loadDistanceX; x++)
        {
            for (int y = -loadDistanceY; y <= loadDistanceY; y++)
            {
                for (int z = -loadDistanceZ; z <= loadDistanceZ; z++)
                {
                    Vector3Int chunkCoord = playerChunkPosition + new Vector3Int(x, y, z);
                    chunksToLoad.Add(chunkCoord);
                }
            }
        }

        // 现在，chunksToUnload仅包含那些确实需要卸载的区块
        foreach (var activeChunk in activeChunkPositions)
        {
            if (!chunksToLoad.Contains(activeChunk))
            {
                chunksToUnload.Add(activeChunk);
            }
        }
    }
    public IEnumerator UpdateChunkLoadingCoroutine()
    {
        chunkLoadCounter = 0;

        // 处理加载
        foreach (var chunkCoord in chunksToLoad)
        {
            if (!IsChunkExistAndActive(chunkCoord))
            {
                LoadChunk(chunkCoord);

                chunkLoadCounter++;
                if (chunkLoadCounter % chunksPerFrame == 0)
                {
                    yield return null; // 可以根据需要调整yield的使用，以平衡性能
                }
            }
        }

        // 处理卸载
        foreach (var chunkCoord in chunksToUnload)
        {
            if (IsChunkExistAndActive(chunkCoord))
            {
                UnloadChunk(chunkCoord); 
                
                chunkLoadCounter++;
                if (chunkLoadCounter % chunksPerFrame == 0)
                {
                    yield return null; // 可以根据需要调整yield的使用，以平衡性能
                }
            }
        }
    }
    public void LoadChunk(Vector3Int chunkCood)
    {
        if (allTerrainChunks.ContainsKey(chunkCood))
        {
            var chunkObj = allTerrainChunks[chunkCood].gameObject;
            if (!chunkObj.activeInHierarchy)
            {
                chunkObj.SetActive(true);
            }
        }
        else
        {
            // instantiate new chunk
            TerrainChunk newChunk = CreateChunkAt(chunkCood.x, chunkCood.y, chunkCood.z);

            // add it 
            allTerrainChunks[chunkCood] = newChunk;
        }

        activeChunkPositions.Add(chunkCood);

        // iterate through all neighbors' faces (opposite)
        CheckRedrawNeighbors(chunkCood);

        // draw
        DrawChunkMesh(chunkCood);
    }
    public void UnloadChunk(Vector3Int chunkCood)
    {
        if (allTerrainChunks.ContainsKey(chunkCood))
        {
            var chunkObj = allTerrainChunks[chunkCood].gameObject;
            if (chunkObj.activeInHierarchy)
            {
                chunkObj.SetActive(false);
            }

            // iterate through all neighbors' faces (opposite)
            CheckRedrawNeighbors(chunkCood);
        }
    }
    #endregion


    #region Chunk Generation
    public IEnumerator GenerateInitialTerrainCoroutine()
    {
        allTerrainChunks = new Dictionary<Vector3Int, TerrainChunk>();
        allTerrainChunkSurfaceHeights = new Dictionary<Vector2Int, int[,]>();
        allTerrainChunkBiomes = new Dictionary<Vector2Int, Biome[,]>();
        crossChunkVegeBuffer = new Dictionary<Vector3Int, List<VegeBlock>>();
        activeChunkPositions = new HashSet<Vector3Int>();

        for (int x = 0; x < worldChunkSizeX; x++)
        {
            for (int y = 0; y < worldChunkSizeY; y++)
            {
                for (int z = 0; z < worldChunkSizeZ; z++)
                {
                    Vector3Int newCoord = new Vector3Int(x, y, z);
                    allTerrainChunks[newCoord] = CreateChunkAt(x, y, z);
                    activeChunkPositions.Add(newCoord);

                    // 每生成一定数量的Chunks后，暂停直到下一帧
                    if ((x * worldChunkSizeY * worldChunkSizeZ + y * worldChunkSizeZ + z) % chunksPerFrame == 0)
                    {
                        yield return null; // 暂停直到下一帧
                    }
                }
            }
        }

        DrawAllChunkMeshes();

        // player dynamic load debug
        player.SetActive(true);
    }
    public void DrawAllChunkMeshes()
    {
        for (int x = 0; x < worldChunkSizeX; x++)
        {
            for (int y = 0; y < worldChunkSizeY; y++)
            {
                for (int z = 0; z < worldChunkSizeZ; z++)
                {
                    DrawChunkMesh(new Vector3Int(x, y, z));
                }
            }
        }
    }
    public void DrawChunkMesh(Vector3Int ChunkCood)
    {
        allTerrainChunks[ChunkCood].DrawTerrainMesh();
    }
    private TerrainChunk CreateChunkAt(int x, int y, int z)
    {
        Vector3 position = new Vector3(x * chunkSizeX, y * chunkSizeY, z * chunkSizeZ);
        TerrainChunk chunk = Instantiate(terrainChunkPrefab, position, Quaternion.identity).GetComponent<TerrainChunk>();

        int posX = (int)position.x;
        int posY = (int)position.z;

        chunk.GenerateChunkData(
            CalculateChunkSurfaceHeights(posX, posY),
            CalculateBiomes(posX, posY));
        return chunk;
    }
    private int[,] CalculateChunkSurfaceHeights(int chunkX, int chunkZ)
    {
        // check if surface height already exists
        Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
        if (allTerrainChunkSurfaceHeights.ContainsKey(chunkPos))
        {
            return allTerrainChunkSurfaceHeights[chunkPos];
        }

        // if not, create a new one
        int[,] yMax = new int[chunkSizeX, chunkSizeZ];

        for (byte x = 0; x < chunkSizeX; x++)
        {
            for (byte z = 0; z < chunkSizeZ; z++)
            {
                int X = x + chunkX;
                int Z = z + chunkZ;

                float continentalnessValue = Mathf.PerlinNoise(X * scale_C, Z * scale_C);
                float erosionValue = Mathf.PerlinNoise((X + noiseDisplacementOffset) * scale_E, (Z + noiseDisplacementOffset) * scale_E);
                float peaksAndValleysValue = Mathf.PerlinNoise((X - noiseDisplacementOffset) * scale_PV, (Z - noiseDisplacementOffset) * scale_PV);
                float c = ContinentalnessCurve.Evaluate(continentalnessValue);
                float e = ErosionCurve.Evaluate(erosionValue);
                float pv = PeaksAndValleysCurve.Evaluate(peaksAndValleysValue);

                yMax[x, z] = Mathf.FloorToInt((c + pv) * e) + baseHeight;
            }
        }

        allTerrainChunkSurfaceHeights[chunkPos] = yMax;

        return yMax;
    }
    private Biome[,] CalculateBiomes(int chunkX, int chunkZ)
    {
        // check if biomes already exists
        Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
        if (allTerrainChunkBiomes.ContainsKey(chunkPos))
        {
            return allTerrainChunkBiomes[chunkPos];
        }

        // if not, create a new one
        Biome[,] biomes = new Biome[chunkSizeX, chunkSizeZ];

        for (byte x = 0; x < chunkSizeX; x++)
        {
            for (byte z = 0; z < chunkSizeZ; z++)
            {
                int X = x + chunkX;
                int Z = z + chunkZ;

                float temperatureValue = Mathf.PerlinNoise(X * scale_T, Z * scale_T);
                float humiditysValue = Mathf.PerlinNoise(X * scale_H, Z * scale_H);
                float t = TemperatureCurve.Evaluate(temperatureValue);
                float h = HumidityCurve.Evaluate(humiditysValue);

                biomes[x, z] = biomeDictRef_byIndices[((byte)Mathf.RoundToInt(t), (byte)Mathf.RoundToInt(h))];
            }
        }

        allTerrainChunkBiomes[chunkPos] = biomes;

        return biomes;
    }
    private bool IsChunkExistAndActive(Vector3Int chunkCood)
    {
        return allTerrainChunks.ContainsKey(chunkCood) && allTerrainChunks[chunkCood].gameObject.activeInHierarchy;
    }
    #endregion


    #region Block Info
    public bool TryGetBlockInfoAt(Vector3Int worldCood, out TerrainBlockData terrainBlockData)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCood, out Vector3Int chunkCood, out Byte3 localCood);

        // if chunk exist
        if (IsChunkExistAndActive(chunkCood))
        {
            terrainBlockData = allTerrainChunks[chunkCood].allTerrainBlockData[localCood.x, localCood.y, localCood.z];
            return true;
        }

        terrainBlockData = new TerrainBlockData();
        return false;
    }
    public bool TryGetActiveBlockWorldCood(Vector3 chunkPos, Byte3 blockLocalCood, out Vector3Int worldCood)
    {
        worldCood = Vector3Int.zero;

        // if chunk exist
        Vector3Int chunkCood = ConvertChunkPosToCood(chunkPos);

        if (IsChunkExistAndActive(chunkCood))
        {
            worldCood = GetBlockWorldCood(chunkPos, blockLocalCood);
            return true;
        }

        return false;
    }
    public Vector3Int GetBlockWorldCood(Vector3 chunkPos, Byte3 blockLocalCood)
    {
        return new Vector3Int(blockLocalCood.x + (int)chunkPos.x, blockLocalCood.y + (int)chunkPos.y, blockLocalCood.z + (int)chunkPos.z);
    }
    public bool IsBlockExist(Vector3Int chunkCoord, Byte3 blockLocalCood)
    {
        // if chunk exist
        if (IsChunkExistAndActive(chunkCoord))
        {
            return !allTerrainChunks[chunkCoord].allTerrainBlockData[blockLocalCood.x, blockLocalCood.y, blockLocalCood.z].IsBlockEmpty();
        }

        return false;
    }
    public bool IsBlockExist(Vector3Int worldCoord)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCoord, out Vector3Int chunkCoord, out Byte3 localCood);

        // if chunk exist
        if (IsChunkExistAndActive(chunkCoord))
        {
            return !allTerrainChunks[chunkCoord].allTerrainBlockData[localCood.x, localCood.y, localCood.z].IsBlockEmpty();
        }

        return false;
    }
    public bool IsBlockWalkable(Vector3Int worldCoord)
    {
        if (!IsBlockExist(worldCoord))
        {
            return true;
        }

        if (!TryGetBlockInfoAt(worldCoord, out TerrainBlockData terrainBlockData))
        {
            return false;
        }

        if (blockDictRef[terrainBlockData.blockType].isGeneratingCollision)
        {
            return false;
        }

        return true;
    }
    public bool IsBlockTransparent(Vector3Int worldCoord)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCoord, out Vector3Int chunkCoord, out Byte3 localCood);

        // if chunk exist
        if (IsChunkExistAndActive(chunkCoord))
        {
            return blockDictRef[allTerrainChunks[chunkCoord].allTerrainBlockData[localCood.x, localCood.y, localCood.z].blockType].isTransparent;
        }

        return false;
    }
    public bool IsBlockDirectional(byte blockID)
    {
        return resourceAssets.blocks[blockID].directionalState != BlockDirectionalState.N;
    }
    public bool IsBlockInteractble(byte blockID)
    {
        return resourceAssets.blocks[blockID].isInteractable;
    }
    public bool IsBlockInBound(Vector3Int worldCood)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCood, out Vector3Int chunkCood, out Byte3 localCood);

        return IsChunkExistAndActive(chunkCood);
    }
    public IBlock_InteractableBlock GetBlockInteraction(byte blockID)
    {
        return resourceAssets.blocks[blockID] as IBlock_InteractableBlock;
    }
    #endregion


    #region Block Interaction
    public void DestroyBlockAt(Vector3Int worldCood)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCood, out Vector3Int chunkCood, out Byte3 localCoord);

        // if chunk exist
        if (IsChunkExistAndActive(chunkCood))
        {
            // Spawn block item world object
            byte blockID = allTerrainChunks[chunkCood].allTerrainBlockData[localCoord.x, localCoord.y, localCoord.z].blockType;
            Item dropItem = ResourceAssets.singleton.blocks[blockID].dropItem;
            if (dropItem != null)
            {
                WorldObjectSpawner.singleton.SpawnItem(worldCood, dropItem.id, 1);
            }

            // Destroy the block
            allTerrainChunks[chunkCood].allTerrainBlockData[localCoord.x, localCoord.y, localCoord.z].DestroyBlock();

            // redraw the mesh
            DrawChunkMesh(chunkCood);

            // check neighbors
            CheckRedrawNeighbors(localCoord, chunkCood);
        }
    }
    public void ConstructBlockAt(Vector3Int worldCoord, byte blockID)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCoord, out Vector3Int chunkCoord, out Byte3 localCoord);

        if (IsBlockExist(chunkCoord, localCoord))
        {
            return;
        }

        // 检查块所在的分块是否存在且处于活动状态
        if (IsChunkExistAndActive(chunkCoord))
        {
            // 设置块类型
            allTerrainChunks[chunkCoord].allTerrainBlockData[localCoord.x, localCoord.y, localCoord.z].ConstructBlock(localCoord.x, localCoord.y, localCoord.z, blockID);

            // 重新生成网格
            DrawChunkMesh(chunkCoord);

            // 检查相邻块是否需要重新绘制
            CheckRedrawNeighbors(localCoord, chunkCoord);
        }
    }
    public void ConstructBlockAt(Vector3Int worldCoord, byte blockID, Vector3 blockDir)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCoord, out Vector3Int chunkCoord, out Byte3 localCoord);

        if (IsBlockExist(chunkCoord, localCoord))
        {
            return;
        }

        // 检查块所在的分块是否存在且处于活动状态
        if (IsChunkExistAndActive(chunkCoord))
        {
            // 设置块类型
            allTerrainChunks[chunkCoord].allTerrainBlockData[localCoord.x, localCoord.y, localCoord.z].ConstructBlock(localCoord.x, localCoord.y, localCoord.z, blockID, BlockDirs[blockDir]);

            // 重新生成网格
            DrawChunkMesh(chunkCoord);

            // 检查相邻块是否需要重新绘制
            CheckRedrawNeighbors(localCoord, chunkCoord);
        }
    }
    #endregion


    #region Utilities
    private void CheckRedrawNeighbors(Vector3Int chunkCood)
    {
        // iterate through all neighbors' faces (opposite)
        var frontNeighbor = chunkCood + DirVectors[Dir.Front];
        var rightNeighbor = chunkCood + DirVectors[Dir.Right];
        var backNeighbor = chunkCood + DirVectors[Dir.Back];
        var letfNeighbor = chunkCood + DirVectors[Dir.Left];
        var topNeighbor = chunkCood + DirVectors[Dir.Top];
        var bottomNeighbor = chunkCood + DirVectors[Dir.Bottom];

        if (IsChunkExistAndActive(frontNeighbor)) DrawChunkMesh(frontNeighbor);
        if (IsChunkExistAndActive(rightNeighbor)) DrawChunkMesh(rightNeighbor);
        if (IsChunkExistAndActive(backNeighbor)) DrawChunkMesh(backNeighbor);
        if (IsChunkExistAndActive(letfNeighbor)) DrawChunkMesh(letfNeighbor);
        if (IsChunkExistAndActive(topNeighbor)) DrawChunkMesh(topNeighbor);
        if (IsChunkExistAndActive(bottomNeighbor)) DrawChunkMesh(bottomNeighbor);
    }
    private void CheckRedrawNeighbors(Byte3 localCood, Vector3Int chunkCood)
    {
        // iterate through all neighbors' faces (opposite)
        var frontNeighbor = chunkCood + DirVectors[Dir.Front];
        var rightNeighbor = chunkCood + DirVectors[Dir.Right];
        var backNeighbor = chunkCood + DirVectors[Dir.Back];
        var letfNeighbor = chunkCood + DirVectors[Dir.Left];
        var topNeighbor = chunkCood + DirVectors[Dir.Top];
        var bottomNeighbor = chunkCood + DirVectors[Dir.Bottom];

        if (localCood.x == chunkSizeX - 1 && IsChunkExistAndActive(frontNeighbor)) DrawChunkMesh(frontNeighbor);
        if (localCood.z == chunkSizeZ - 1 && IsChunkExistAndActive(rightNeighbor)) DrawChunkMesh(rightNeighbor);
        if (localCood.x == 0 && IsChunkExistAndActive(backNeighbor)) DrawChunkMesh(backNeighbor);
        if (localCood.z == 0 && IsChunkExistAndActive(letfNeighbor)) DrawChunkMesh(letfNeighbor);
        if (localCood.y == chunkSizeY - 1 && IsChunkExistAndActive(topNeighbor)) DrawChunkMesh(topNeighbor);
        if (localCood.y == 0 && IsChunkExistAndActive(bottomNeighbor)) DrawChunkMesh(bottomNeighbor);
    }
    public bool ShouldDrawFace(Vector3Int worldCoord)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCoord, out Vector3Int chunkCoord, out Byte3 localCood);

        // if chunk exist
        if (IsChunkExistAndActive(chunkCoord))
        {
            // if neighbor block transparent
            if (blockDictRef[allTerrainChunks[chunkCoord].allTerrainBlockData[localCood.x, localCood.y, localCood.z].blockType].isTransparent)
            {
                return true;
            }

            // if neighbor block exist
            return allTerrainChunks[chunkCoord].allTerrainBlockData[localCood.x, localCood.y, localCood.z].IsBlockEmpty();
        }

        return true;
    }
    public int GetNeighberBlockLocalHeight(Vector2Int neighbourCoord, int height, int searchHeight)
    {
        for (int i = 1; i >= -1; i--)
        {
            if (!IsBlockExist(new Vector3Int(neighbourCoord.x, height + i, neighbourCoord.y)))
            {
                continue;
            }

            for (int j = 1; j <= searchHeight; j++)
            {
                if (IsBlockExist(new Vector3Int(neighbourCoord.x, height + i + j, neighbourCoord.y)))
                {
                    return height + i + j;
                }
            }

            return height + i;
        }
        
        return 999;
    }
    public Vector3Int GetHitPointWorldCood(Vector3 hitPoint)
    {
        return new Vector3Int(
            Mathf.FloorToInt(hitPoint.x),
            Mathf.FloorToInt(hitPoint.y),
            Mathf.FloorToInt(hitPoint.z));
    }
    private Vector3Int ConvertChunkPosToCood(Vector3 chunkPos)
    {
        return new Vector3Int(
            Mathf.FloorToInt(chunkPos.x / chunkSizeX),
            Mathf.FloorToInt(chunkPos.y / chunkSizeY),
            Mathf.FloorToInt(chunkPos.z / chunkSizeZ));
    }
    private Vector3Int ConvertWorldCoodToChunkCood(Vector3Int worldCood)
    {
        return new Vector3Int(
            Mathf.FloorToInt((float)worldCood.x / chunkSizeX),
            Mathf.FloorToInt((float)worldCood.y / chunkSizeY),
            Mathf.FloorToInt((float)worldCood.z / chunkSizeZ));
    }
    private void ConvertWorldCoodToChunkAndLocalCood(Vector3Int worldCood, out Vector3Int chunkCood, out Byte3 localCood)
    {
        chunkCood = new Vector3Int(
            Mathf.FloorToInt((float)worldCood.x / chunkSizeX),
            Mathf.FloorToInt((float)worldCood.y / chunkSizeY),
            Mathf.FloorToInt((float)worldCood.z / chunkSizeZ));

        localCood = new Byte3(
            (byte)(worldCood.x - (chunkCood.x * chunkSizeX)),
            (byte)(worldCood.y - (chunkCood.y * chunkSizeY)),
            (byte)(worldCood.z - (chunkCood.z * chunkSizeZ)));
    }
    private Vector3Int CalculatePlayerChunkPosition()
    {
        Vector3 playerPos = player.transform.position;
        return new Vector3Int(
            Mathf.FloorToInt(playerPos.x / chunkSizeX),
            Mathf.FloorToInt(playerPos.y / chunkSizeY),
            Mathf.FloorToInt(playerPos.z / chunkSizeZ));
    }
    public BiomeType GetWorldCoordBiomeType(Vector3Int worldCood)
    {
        ConvertWorldCoodToChunkAndLocalCood(worldCood, out Vector3Int chunkCoord, out Byte3 localCoord);
        return allTerrainChunkBiomes[new Vector2Int(chunkCoord.x * chunkSizeX, chunkCoord.z * chunkSizeZ)][localCoord.x, localCoord.z].biomeType;
    }
    #endregion
}
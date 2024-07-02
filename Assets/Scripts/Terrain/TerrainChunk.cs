using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class TerrainChunk : MonoBehaviour
{
    public static readonly Byte3[] vertexRef = 
    {
        new Byte3(0, 0, 0), // 0
        new Byte3(1, 0, 0), // 1
        new Byte3(1, 1, 0), // 2
        new Byte3(0, 1, 0), // 3
        new Byte3(0, 1, 1), // 4
        new Byte3(1, 1, 1), // 5
        new Byte3(1, 0, 1), // 6
        new Byte3(0, 0, 1)  // 7
    };

    // chunk size
    private byte width;
    private byte height;
    private byte depth;

    // basic density
    private AnimationCurve curve_DensitySquash;
    private AnimationCurve curve_DensityScale;

    // tree
    private float vegeGenerationScale_Small;
    private float vegeGenerationScale_Large;

    // terrain data
    public TerrainBlockData[,,] allTerrainBlockData;
    public MeshCollider meshCollider_forCollision;
    public MeshCollider meshCollider_forNonCollision;
    public MeshFilter meshFilterForCollision;
    public MeshFilter meshFilterForNonCollision;
    private Vector3Int chunkPos;

    public List<Vector3> allVerts_forCollision = new List<Vector3>();
    public List<int> allTris_forCollision = new List<int>();
    public List<Vector2> allUVs_forCollision = new List<Vector2>();

    public List<Vector3> allVerts_forNonCollision = new List<Vector3>();
    public List<int> allTris_forNonCollision = new List<int>();
    public List<Vector2> allUVs_forNonCollision = new List<Vector2>();

    // singleton refs
    private Dictionary<BlockDir, Vector3> blockDirVectors;
    private Dictionary<byte, Block> blocksDictRef;
    private Dictionary<Vector3Int, List<VegeBlock>> crossChunkVegeBuffer;
    private TerrainManager terrainManager;
    private ResourceAssets resourceAssets;

    private void Awake()
    {
        // refs
        terrainManager = TerrainManager.singleton;
        resourceAssets = ResourceAssets.singleton;
        blockDirVectors = terrainManager.BlockDirVectors;
        blocksDictRef = resourceAssets.blocks;
        crossChunkVegeBuffer = terrainManager.crossChunkVegeBuffer;

        // chunk size
        width = terrainManager.chunkSizeX;
        height = terrainManager.chunkSizeY;
        depth = terrainManager.chunkSizeZ;

        // basic density
        curve_DensitySquash = terrainManager.DensitySquashCurve;
        curve_DensityScale = terrainManager.DensityScaleCurve;

        // tree
        vegeGenerationScale_Small = terrainManager.vegeGenerationScale_Small;
        vegeGenerationScale_Large = terrainManager.vegeGenerationScale_Large;
    }

    public void GenerateChunkData(int[,] yMAX, Biome[,] biomes)
    {
        allTerrainBlockData = new TerrainBlockData[width, height, depth];
        chunkPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);

        // Pass 1-4: Basic Blocks
        for (byte x = 0; x < width; x++)
        {
            for (byte z = 0; z < depth; z++)
            {
                for (byte y = 0; y < height; y++)
                {
                    GetBlock(x, y, z, yMAX, biomes);
                }
            }
        }

        // Pass 5: Veges
        GetVegeBlocksFromBuffer();
        for (byte x = 0; x < width; x++)
        {
            for (byte z = 0; z < depth; z++)
            {
                int yMax = yMAX[x, z];
                if (yMax > chunkPos.y + height - 1 || yMax < chunkPos.y) continue;

                int rootY = yMax - chunkPos.y;

                // get vege
                GetVege(x, z, rootY, biomes[x,z]);
            }
        }
    }

    private void GetBlock(byte x, byte y, byte z, int[,] yMAX, Biome[,] biomes)
    {
        allTerrainBlockData[x, y, z] = new TerrainBlockData();

        int X = x + chunkPos.x;
        int Y = y + chunkPos.y;
        int Z = z + chunkPos.z;

        // Pass 1: Get Surface Height (from parameter)
        int yMax = yMAX[x, z];

        // Pass 2: Get Biome (from parameter)
        Biome biome = biomes[x, z];

        // Pass 3: Get Density
        float density = noise.cnoise(new float3(X, Y, Z) * curve_DensityScale.Evaluate(Y)) + curve_DensitySquash.Evaluate(Y);

        // Pass 4: Get Block
        if (Y <= yMax && density >= 0f)
        {
            allTerrainBlockData[x, y, z].ConstructBlock(x, y, z, biome.GetBlockIdByDepth(yMax - Y));
        }
        else
        {
            allTerrainBlockData[x, y, z].ConstructBlock(x, y, z, Block_Empty.refID);
        }
    }

    private void GetVegeBlocksFromBuffer()
    {
        if (crossChunkVegeBuffer.ContainsKey(chunkPos) && crossChunkVegeBuffer[chunkPos] != null)
        {
            foreach (VegeBlock bufferVegeBlock in crossChunkVegeBuffer[chunkPos])
            {
                Vector3Int vegePos = bufferVegeBlock.pos;
                byte vegeBlockId = bufferVegeBlock.blockID;

                allTerrainBlockData[vegePos.x, vegePos.y, vegePos.z] = new TerrainBlockData();
                allTerrainBlockData[vegePos.x, vegePos.y, vegePos.z].ConstructBlock((byte)vegePos.x, (byte)vegePos.y, (byte)vegePos.z, vegeBlockId);
            }

            crossChunkVegeBuffer.Remove(chunkPos);
        }
    }

    private void GetVege(byte x, byte z, int rootY, Biome biome)
    {
        Vegetation vege = biome.GetRandomVege();
        if (vege == null) return;

        float randomVegeValue;
        float vegeThreshold;
        if (vege.vegeSize == VegetationSize.Big)
        {
            randomVegeValue = Mathf.PerlinNoise((x + chunkPos.x) * vegeGenerationScale_Large, (z + chunkPos.z) * vegeGenerationScale_Large);
            vegeThreshold = biome.vegeThreshold_big;
        }
        else
        {
            randomVegeValue = Mathf.PerlinNoise((x + chunkPos.x) * vegeGenerationScale_Small, (z + chunkPos.z) * vegeGenerationScale_Small);
            vegeThreshold = biome.vegeThreshold_small;
        }

        // spawn vege
        if (randomVegeValue > vegeThreshold && !allTerrainBlockData[x, rootY, z].IsBlockEmpty())
        {
            foreach (VegeBlock vegePart in vege.vegeBlocks)
            {
                Vector3Int vegePos = vegePart.pos + new Vector3Int(x, rootY + 1, z);
                byte vegeBlockId = vegePart.blockID;

                // if vege falls outside this chunk
                bool isVegeOutSideChunk = false;
                Vector3Int neighborChunkPos = chunkPos;
                Vector3Int newVegePos = vegePos;

                if (vegePos.x < 0)
                {
                    neighborChunkPos += new Vector3Int(-width, 0, 0);
                    newVegePos += new Vector3Int(width, 0, 0);
                    isVegeOutSideChunk = true;
                }
                if (vegePos.x > width - 1)
                {
                    neighborChunkPos += new Vector3Int(+width, 0, 0);
                    newVegePos += new Vector3Int(-width, 0, 0);
                    isVegeOutSideChunk = true;
                }
                if (vegePos.y < 0)
                {
                    neighborChunkPos += new Vector3Int(0, -height, 0);
                    newVegePos += new Vector3Int(0, height, 0);
                    isVegeOutSideChunk = true;
                }
                if (vegePos.y > height - 1)
                {
                    neighborChunkPos += new Vector3Int(0, +height, 0);
                    newVegePos += new Vector3Int(0, -height, 0);
                    isVegeOutSideChunk = true;
                }
                if (vegePos.z < 0)
                {
                    neighborChunkPos += new Vector3Int(0, 0, -depth);
                    newVegePos += new Vector3Int(0, 0, depth);
                    isVegeOutSideChunk = true;
                }
                if (vegePos.z > depth - 1)
                {
                    neighborChunkPos += new Vector3Int(0, 0, +depth);
                    newVegePos += new Vector3Int(0, 0, -depth);
                    isVegeOutSideChunk = true;
                }

                if (isVegeOutSideChunk)
                {
                    if (!crossChunkVegeBuffer.ContainsKey(neighborChunkPos) || crossChunkVegeBuffer[neighborChunkPos] == null)
                    {
                        crossChunkVegeBuffer[neighborChunkPos] = new List<VegeBlock>();
                    }

                    crossChunkVegeBuffer[neighborChunkPos].Add(new VegeBlock(newVegePos, vegeBlockId));
                    continue;
                }

                allTerrainBlockData[vegePos.x, vegePos.y, vegePos.z] = new TerrainBlockData();
                allTerrainBlockData[vegePos.x, vegePos.y, vegePos.z].ConstructBlock((byte)vegePos.x, (byte)vegePos.y, (byte)vegePos.z, vegeBlockId);
            }
        }
    }

    public void RecalcMeshInfo()
    {
        int faceCount_forCollision = 0;
        int faceCount_forNonCollision = 0;
        allVerts_forCollision.Clear();
        allTris_forCollision.Clear();
        allUVs_forCollision.Clear();
        allVerts_forNonCollision.Clear();
        allTris_forNonCollision.Clear();
        allUVs_forNonCollision.Clear();

        for (byte x = 0; x < width; x++)
        {
            for (byte y = 0; y < height; y++)
            {
                for (byte z = 0; z < depth; z++)
                {
                    TerrainBlockData blockData = allTerrainBlockData[x, y, z];
                    if (blockData.IsBlockEmpty())
                    {
                        continue;
                    }

                    Vector3Int blockCoord = new Vector3Int(x, y, z);
                    byte blockType = blockData.blockType;
                    Block block = blocksDictRef[blockType];
                    Vector3 blockDir = blockDirVectors[blockData.blockDir];
                    bool isDirectionalBlock = block.directionalState != BlockDirectionalState.N;
                    bool isGeneratingCollision = block.isGeneratingCollision;
                    Face[] faces = block.faces;

                    if (block.isStateBlock)
                    {
                        faces = (block as IBlock_StateBlock).GetStateFaces(1);
                    }

                    foreach (Face face in faces)
                    {
                        Vector2[] uvs = face.GetUVs();
                        Vector3[] verts = face.verts;
                        int[] tris = face.tris;
                        if (isDirectionalBlock) verts = face.GetVertsDirectional(blockDir);

                        if (face.dir == Vector3Int.zero)
                        {
                            if (isGeneratingCollision)
                            {
                                // 添加顶点
                                allVerts_forCollision.Add(verts[0] + blockCoord);
                                allVerts_forCollision.Add(verts[1] + blockCoord);
                                allVerts_forCollision.Add(verts[2] + blockCoord);
                                allVerts_forCollision.Add(verts[3] + blockCoord);

                                // 添加三角形
                                allTris_forCollision.Add(tris[0] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[1] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[2] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[3] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[4] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[5] + faceCount_forCollision * 4);

                                // UV
                                allUVs_forCollision.Add(uvs[0]);
                                allUVs_forCollision.Add(uvs[1]);
                                allUVs_forCollision.Add(uvs[2]);
                                allUVs_forCollision.Add(uvs[3]);

                                faceCount_forCollision++;
                            }
                            else
                            {
                                // 添加顶点
                                allVerts_forNonCollision.Add(verts[0] + blockCoord);
                                allVerts_forNonCollision.Add(verts[1] + blockCoord);
                                allVerts_forNonCollision.Add(verts[2] + blockCoord);
                                allVerts_forNonCollision.Add(verts[3] + blockCoord);

                                // 添加三角形
                                allTris_forNonCollision.Add(tris[0] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[1] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[2] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[3] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[4] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[5] + faceCount_forNonCollision * 4);

                                // UV
                                allUVs_forNonCollision.Add(uvs[0]);
                                allUVs_forNonCollision.Add(uvs[1]);
                                allUVs_forNonCollision.Add(uvs[2]);
                                allUVs_forNonCollision.Add(uvs[3]);

                                faceCount_forNonCollision++;
                            }
                            continue;
                        }

                        Vector3Int drawDir = blockCoord + face.dir + chunkPos;
                        if (isDirectionalBlock || (terrainManager.ShouldDrawFace(drawDir) && terrainManager.IsBlockInBound(drawDir)))
                        {
                            if (isGeneratingCollision)
                            {
                                // 添加顶点
                                allVerts_forCollision.Add(verts[0] + blockCoord);
                                allVerts_forCollision.Add(verts[1] + blockCoord);
                                allVerts_forCollision.Add(verts[2] + blockCoord);
                                allVerts_forCollision.Add(verts[3] + blockCoord);

                                // 添加三角形
                                allTris_forCollision.Add(tris[0] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[1] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[2] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[3] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[4] + faceCount_forCollision * 4);
                                allTris_forCollision.Add(tris[5] + faceCount_forCollision * 4);

                                // UV
                                allUVs_forCollision.Add(uvs[0]);
                                allUVs_forCollision.Add(uvs[1]);
                                allUVs_forCollision.Add(uvs[2]);
                                allUVs_forCollision.Add(uvs[3]);

                                faceCount_forCollision++;
                            }
                            else
                            {
                                // 添加顶点
                                allVerts_forNonCollision.Add(verts[0] + blockCoord);
                                allVerts_forNonCollision.Add(verts[1] + blockCoord);
                                allVerts_forNonCollision.Add(verts[2] + blockCoord);
                                allVerts_forNonCollision.Add(verts[3] + blockCoord);

                                // 添加三角形
                                allTris_forNonCollision.Add(tris[0] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[1] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[2] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[3] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[4] + faceCount_forNonCollision * 4);
                                allTris_forNonCollision.Add(tris[5] + faceCount_forNonCollision * 4);

                                // UV
                                allUVs_forNonCollision.Add(uvs[0]);
                                allUVs_forNonCollision.Add(uvs[1]);
                                allUVs_forNonCollision.Add(uvs[2]);
                                allUVs_forNonCollision.Add(uvs[3]);

                                faceCount_forNonCollision++;
                            }
                        }
                    }
                }
            }
        }
    }

    public void DrawTerrainMesh()
    {
        // create new meshes
        Mesh mesh_forCollision = new Mesh();
        Mesh mesh_forNonCollision = new Mesh();
        RecalcMeshInfo();

        // mesh for collision
        mesh_forCollision.vertices = allVerts_forCollision.ToArray();
        mesh_forCollision.triangles = allTris_forCollision.ToArray();
        mesh_forCollision.uv = allUVs_forCollision.ToArray();
        mesh_forCollision.RecalculateNormals();
        meshFilterForCollision.mesh = mesh_forCollision;
        meshCollider_forCollision.sharedMesh = mesh_forCollision;

        // mesh for non-collision
        mesh_forNonCollision.vertices = allVerts_forNonCollision.ToArray();
        mesh_forNonCollision.triangles = allTris_forNonCollision.ToArray();
        mesh_forNonCollision.uv = allUVs_forNonCollision.ToArray();
        mesh_forNonCollision.RecalculateNormals();
        meshFilterForNonCollision.mesh = mesh_forNonCollision;
        meshCollider_forNonCollision.sharedMesh = mesh_forNonCollision;
    }
}
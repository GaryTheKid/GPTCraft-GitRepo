using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectSpawner : MonoBehaviour
{
    public const float POP_ITEM_PICK_UP_TIME = 3f;

    // singleton
    public static WorldObjectSpawner singleton;
    private TerrainManager terrainManager;
    private TimeManager timeManager;
    private ResourceAssets resourceAssets;


    [Header("====== General Spawn Settings ======")]
    public int maxSpawnDistance = 20;
    public int minSpawnDistance = 10;
    public int spawnHeightRange = 5;
    public int maxEnemies = 5;
    private Vector3 lastPlayerPos;


    [Space(15)]
    [Header("====== Mob Spawn Settings ======")]
    public int mobsPerFrame;
    public int mobSpawnMaxRuntime;
    private int mobLoadCounter;
    public List<GameObject> spawnedMobs = new List<GameObject>();
    public List<GameObject> activeSpawnedMobs = new List<GameObject>();
    private List<Vector3Int> availableSpawnPoints = new List<Vector3Int>();
    private Dictionary<Vector3Int, byte> availableSpawnPointMaxHeight = new Dictionary<Vector3Int, byte>();
    private HashSet<GameObject> mobsToLoad = new HashSet<GameObject>();
    private HashSet<GameObject> mobsToUnload = new HashSet<GameObject>();
    private IEnumerator currentMobUpdateCoroutine = null;


    [Space(15)]
    [Header("====== Item Spawn Settings ======")]
    public int itemsPerFrame;
    private int itemLoadCounter;
    public List<GameObject> spawnedItems = new List<GameObject>();
    public List<GameObject> activeSpawnedItems = new List<GameObject>();
    private HashSet<GameObject> itemsToLoad = new HashSet<GameObject>();
    private HashSet<GameObject> itemsToUnload = new HashSet<GameObject>();
    private IEnumerator currentItemUpdateCoroutine = null;


    #region Unity Functions
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }
    private void Start()
    {
        terrainManager = TerrainManager.singleton;
        timeManager = TimeManager.singleton;
        resourceAssets = ResourceAssets.singleton;
    }
    private void Update()
    {
        if (Vector3.Distance(PlayerStats.Global_playerWorldCoord, lastPlayerPos) > minSpawnDistance)
        {
            HandleMobDynamicLoading(PlayerStats.Global_playerWorldCoord);
            HandleItemDynamicLoading(PlayerStats.Global_playerWorldCoord);

            lastPlayerPos = PlayerStats.Global_playerWorldCoord;
        }
    }
    #endregion


    #region Mob Spawn
    public GameObject SpawnMob(Vector3Int worldCoord, byte mobID)
    {
        // 实例化AI
        Mob mob = resourceAssets.mobs[mobID];
        GameObject mobObj = Instantiate(mob.mobPrefab, Vector3.one * 0.5f + worldCoord, Quaternion.identity);
        MobWorldController mobWorld = mobObj.GetComponent<MobWorldController>();
        mobWorld.SetMobClass(mob);

        return mobObj;
    }
    public void SpawnMobsNearPlayer() // 后期引入难度、方块光照/类型、世界地形等因素，来影响不同的怪物生成
    {
        UpdateAvailableSpawnPointsNearPlayer();

        // check if no space to spawn
        if (availableSpawnPoints == null || availableSpawnPoints.Count == 0 || availableSpawnPointMaxHeight == null || availableSpawnPointMaxHeight.Count == 0)
        {
            Debug.Log("No Space to Spawn");
            return;
        }

        StartCoroutine(SpawnMobsNearPlayerCoroutine());
    }
    private IEnumerator SpawnMobsNearPlayerCoroutine()
    {
        int mobSpawnCounter = 0;
        int mobSpawnRuntime = 0;

        while (activeSpawnedMobs.Count < maxEnemies)
        {
            if (TrySpawnMob())
            {
                mobSpawnCounter++;
                if (mobSpawnCounter % mobsPerFrame == 0)
                {
                    yield return null; // 可以根据需要调整yield的使用，以平衡性能
                }
            }

            mobSpawnRuntime++;
            if (mobSpawnRuntime >= mobSpawnMaxRuntime) yield break;
        }
    }
    private bool TrySpawnMob()
    {
        if (availableSpawnPoints == null || availableSpawnPoints.Count == 0 || availableSpawnPointMaxHeight == null || availableSpawnPointMaxHeight.Count == 0) return false;

        Vector3Int spawnCoord = availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
        BiomeType spawnBiomeType = terrainManager.GetWorldCoordBiomeType(spawnCoord);
        byte spawnMobID = resourceAssets.GetMob(timeManager.ConvertToMobSpawnTime(), spawnBiomeType, availableSpawnPointMaxHeight[spawnCoord], out bool isQualifiedMobExist);

        print(isQualifiedMobExist);

        if (!isQualifiedMobExist) return false;

        GameObject enemy = SpawnMob(spawnCoord, spawnMobID);
        spawnedMobs.Add(enemy);

        availableSpawnPoints.Remove(spawnCoord);
        availableSpawnPointMaxHeight.Remove(spawnCoord);

        return true;
    }
    #endregion


    #region Mob Dynamic Loading
    public void HandleMobDynamicLoading(Vector3Int playerChunkPosition)
    {
        if (playerChunkPosition != lastPlayerPos)
        {
            UpdateMobsToLoadAndUnload(playerChunkPosition);
            if (currentMobUpdateCoroutine != null)
            {
                StopCoroutine(currentMobUpdateCoroutine);
            }
            currentMobUpdateCoroutine = UpdateMobLoadingCoroutine();
            StartCoroutine(currentMobUpdateCoroutine);
        }
    }
    private void UpdateMobsToLoadAndUnload(Vector3Int playerChunkPosition)
    {
        mobsToLoad.Clear();
        mobsToUnload.Clear();

        foreach (var AI in spawnedMobs)
        {
            if (AI == null && AI.activeInHierarchy)
            {
                continue;
            }

            if (Vector3.Distance(AI.transform.position, playerChunkPosition) <= maxSpawnDistance)
            {
                mobsToLoad.Add(AI);
            }
        }

        foreach (var activeAI in activeSpawnedMobs)
        {
            if (activeAI == null && !activeAI.activeInHierarchy)
            {
                continue;
            }

            if (Vector3.Distance(activeAI.transform.position, playerChunkPosition) > maxSpawnDistance)
            {
                mobsToUnload.Add(activeAI);
            }
        }
    }
    public IEnumerator UpdateMobLoadingCoroutine()
    {
        mobLoadCounter = 0;

        // 处理加载
        foreach (var ai in mobsToLoad)
        {
            ai.SetActive(true);

            mobLoadCounter++;
            if (mobLoadCounter % mobsPerFrame == 0)
            {
                yield return null; // 可以根据需要调整yield的使用，以平衡性能
            }
        }

        // 处理卸载
        foreach (var ai in mobsToUnload)
        {
            ai.SetActive(false);

            mobLoadCounter++;
            if (mobLoadCounter % mobsPerFrame == 0)
            {
                yield return null; // 可以根据需要调整yield的使用，以平衡性能
            }
        }
    }
    #endregion


    #region Item Spawn
    public void SpawnItem(Vector3Int worldCood, byte itemID, int amount)
    {
        // 实例化道具
        Item item = ResourceAssets.singleton.items[itemID];
        ItemData itemData = new ItemData(itemID);
        GameObject itemPrefab = item.itemPrefab;

        // 检测item是否为null
        if (itemPrefab == null)
        {
            return;
        }

        GameObject itemObj = Instantiate(itemPrefab, (Vector3.one * 0.5f) + worldCood, Quaternion.identity);
        ItemWorldObject itemWorld = itemObj.GetComponent<ItemWorldObject>();

        // 设置itemWorld信息
        itemWorld.itemData = itemData;
        itemWorld.amount = amount; // 设置amount属性

        // 获取刚体组件
        Rigidbody rb = itemObj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 计算一个随机方向
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
            randomDirection.Normalize();

            // 应用力
            float forceStrength = Random.Range(5f, 10f); // 这里的数值可以根据需要调整
            rb.AddForce(randomDirection * forceStrength, ForceMode.Impulse);
        }

        // 设置自我销毁Coroutine
        itemWorld.SetDestroyTimeCoroutine(0f, item.itemSelfDestroyTime);

        spawnedItems.Add(itemObj);
    }
    public void PopItemToDirection(Vector3 pos, Vector3 dir, ItemData itemData, int amount)
    {
        // 实例化道具
        ItemData itemDataCopy = itemData.GetCopy();
        Item item = resourceAssets.items[itemDataCopy.id];
        GameObject itemObj = Instantiate(item.itemPrefab, pos, Quaternion.identity);
        ItemWorldObject itemWorld = itemObj.GetComponent<ItemWorldObject>();

        // 设置itemWorld信息
        itemWorld.itemData = itemDataCopy;
        itemWorld.amount = amount; // 设置amount属性

        // 获取刚体组件
        Rigidbody rb = itemObj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 添加一个小的随机偏移以模拟左右和上下的随机性
            Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

            // 应用力，方向加上随机偏移
            float forceStrength = Random.Range(5f, 10f); // 这里的数值可以根据需要调整
            Vector3 forceDirection = dir + randomOffset;
            forceDirection.Normalize();
            rb.AddForce(forceDirection * forceStrength, ForceMode.Impulse);
        }

        // 设置自我销毁Coroutine
        itemWorld.SetDestroyTimeCoroutine(POP_ITEM_PICK_UP_TIME, item.itemSelfDestroyTime);

        spawnedItems.Add(itemObj);
    }
    #endregion


    #region Item Dynamic Loading
    public void HandleItemDynamicLoading(Vector3Int playerChunkPosition)
    {
        if (playerChunkPosition != lastPlayerPos)
        {
            UpdateItemsToLoadAndUnload(playerChunkPosition);
            if (currentItemUpdateCoroutine != null)
            {
                StopCoroutine(currentItemUpdateCoroutine);
            }
            currentItemUpdateCoroutine = UpdateItemLoadingCoroutine();
            StartCoroutine(currentItemUpdateCoroutine);
        }
    }
    private void UpdateItemsToLoadAndUnload(Vector3Int playerChunkPosition)
    {
        itemsToLoad.Clear();
        itemsToUnload.Clear();

        foreach (var item in spawnedItems)
        {
            if (item == null && item.activeInHierarchy)
            {
                continue;
            }

            if (Vector3.Distance(item.transform.position, playerChunkPosition) <= maxSpawnDistance)
            {
                itemsToLoad.Add(item);
            }
        }

        foreach (var activeitem in activeSpawnedItems)
        {
            if (activeitem == null && !activeitem.activeInHierarchy)
            {
                continue;
            }

            if (Vector3.Distance(activeitem.transform.position, playerChunkPosition) > maxSpawnDistance)
            {
                itemsToUnload.Add(activeitem);
            }
        }
    }
    public IEnumerator UpdateItemLoadingCoroutine()
    {
        itemLoadCounter = 0;

        // 处理加载
        foreach (var item in itemsToLoad)
        {
            item.SetActive(true);

            itemLoadCounter++;
            if (itemLoadCounter % itemsPerFrame == 0)
            {
                yield return null; // 可以根据需要调整yield的使用，以平衡性能
            }
        }

        // 处理卸载
        foreach (var item in itemsToUnload)
        {
            item.SetActive(false);

            itemLoadCounter++;
            if (itemLoadCounter % itemsPerFrame == 0)
            {
                yield return null; // 可以根据需要调整yield的使用，以平衡性能
            }
        }
    }
    #endregion


    #region Utitilies
    private void UpdateAvailableSpawnPointsNearPlayer()
    {
        availableSpawnPoints.Clear();
        availableSpawnPointMaxHeight.Clear();

        for (int x = -maxSpawnDistance; x <= maxSpawnDistance; x++)
        {
            for (int z = -maxSpawnDistance; z <= maxSpawnDistance; z++)
            {
                if (x >= -minSpawnDistance && x <= minSpawnDistance && z >= -minSpawnDistance && z <= minSpawnDistance)
                {
                    continue;
                }
                 
                for (int y = -spawnHeightRange; y <= spawnHeightRange; y++)
                {
                    Vector3Int coord = PlayerStats.Global_playerWorldCoord + new Vector3Int(x, y, z);
                    int spawnVal = IsSpawnPoint(coord);

                    switch (spawnVal)
                    {
                        case -1: { continue; }
                        default: { availableSpawnPoints.Add(coord); availableSpawnPointMaxHeight[coord] = (byte)spawnVal; } break;
                    }
                }
            }
        }
    }
    private int IsSpawnPoint(Vector3Int worldCoord)
    {
        int maxSpawnHeight = -1;

        if (!TerrainManager.singleton.IsBlockExist(worldCoord + new Vector3Int(0, -1, 0)))
        {
            return maxSpawnHeight;
        }
        
        for (int i = 0; i < 3; i++)
        {
            Vector3Int tileAbove = worldCoord + new Vector3Int(0, i, 0);
            if (!TerrainManager.singleton.IsBlockExist(tileAbove))
            {
                maxSpawnHeight = i + 1;
            }
            else 
            {
                break;
            }
        }

        return maxSpawnHeight;
    }
    #endregion
}

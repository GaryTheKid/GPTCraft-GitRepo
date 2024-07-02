using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectSpawner : MonoBehaviour
{
    public const float POP_ITEM_PICK_UP_TIME = 3f;

    // singleton
    public static WorldObjectSpawner singleton;

    public int maxSpawnDistance = 20;
    public int minSpawnDistance = 10;
    public int spawnHeightRange = 5;
    public int maxEnemies = 5;
    private Vector3 lastPlayerPos;

    // AI
    public List<GameObject> spawnedMobs = new List<GameObject>();
    public List<GameObject> activeSpawnedMobs = new List<GameObject>();
    private List<Vector3Int> availableSpawnPoints_H1 = new List<Vector3Int>();
    private List<Vector3Int> availableSpawnPoints_H2 = new List<Vector3Int>();
    private List<Vector3Int> availableSpawnPoints_H3 = new List<Vector3Int>();

    private int mobLoadCounter;
    public int mobsPerFrame;

    private HashSet<GameObject> mobsToLoad = new HashSet<GameObject>();
    private HashSet<GameObject> mobsToUnload = new HashSet<GameObject>();
    private IEnumerator currentMobUpdateCoroutine = null;

    // Item
    public List<GameObject> spawnedItems = new List<GameObject>();
    public List<GameObject> activeSpawnedItems = new List<GameObject>();

    private int itemLoadCounter;
    public int itemsPerFrame;

    private HashSet<GameObject> itemsToLoad = new HashSet<GameObject>();
    private HashSet<GameObject> itemsToUnload = new HashSet<GameObject>();
    private IEnumerator currentItemUpdateCoroutine = null;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
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
        Item item = ResourceAssets.singleton.items[itemDataCopy.id];
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

    public GameObject SpawnMob(Vector3Int worldCood, byte mobID)
    {
        // 实例化AI
        Mob mob = ResourceAssets.singleton.mobs[mobID];
        GameObject mobObj = Instantiate(mob.mobPrefab, Vector3.one * 0.5f + worldCood, Quaternion.identity);
        MobWorldController mobWorld = mobObj.GetComponent<MobWorldController>();
        mobWorld.SetMobClass(mob);

        return mobObj;
    }

    public void SpawnMobsNearPlayer() // 后期引入难度、方块光照/类型、世界地形等因素，来影响不同的怪物生成
    {
        UpdateAvailableSpawnPointsNearPlayer();

        // check if no space to spawn
        if (availableSpawnPoints_H1 == null || availableSpawnPoints_H1.Count == 0 ||
            availableSpawnPoints_H2 == null || availableSpawnPoints_H2.Count == 0 ||
            availableSpawnPoints_H3 == null || availableSpawnPoints_H3.Count == 0)
        {
            Debug.Log("No Space to Spawn");
            return;
        }

        StartCoroutine(SpawnMobsNearPlayerCoroutine());
    }

    private IEnumerator SpawnMobsNearPlayerCoroutine()
    {
        int mobSpawnCounter = 0;

        while (activeSpawnedMobs.Count < maxEnemies)
        {


            // TODO: roll a value for spawned enemy type
            // if enemy height == 3
            TrySpawnEnemy_H3();

            // if enemy height == 2
            TrySpawnEnemy_H2();

            // if enemy height == 1
            TrySpawnEnemy_H1();

            mobSpawnCounter++;
            if (mobSpawnCounter % mobsPerFrame == 0)
            {
                yield return null; // 可以根据需要调整yield的使用，以平衡性能
            }
        }
    }

    private void TrySpawnEnemy_H1()
    {
        Vector3Int spawnCoord = availableSpawnPoints_H1[Random.Range(0, availableSpawnPoints_H1.Count)];
        GameObject enemy = SpawnMob(spawnCoord, Mob_Hostile_Zombie.refID);
        spawnedMobs.Add(enemy);
        availableSpawnPoints_H1.Remove(spawnCoord);
        try { availableSpawnPoints_H2.Remove(spawnCoord); } catch { }
        try { availableSpawnPoints_H3.Remove(spawnCoord); } catch { }
    }
    private void TrySpawnEnemy_H2()
    {
        Vector3Int spawnCoord = availableSpawnPoints_H2[Random.Range(0, availableSpawnPoints_H1.Count)];
        GameObject enemy = SpawnMob(spawnCoord, Mob_Hostile_Zombie.refID);
        spawnedMobs.Add(enemy);
        availableSpawnPoints_H2.Remove(spawnCoord);
        try { availableSpawnPoints_H3.Remove(spawnCoord); } catch { }
    }
    private void TrySpawnEnemy_H3()
    {
        Vector3Int spawnCoord = availableSpawnPoints_H3[Random.Range(0, availableSpawnPoints_H1.Count)];
        GameObject enemy = SpawnMob(spawnCoord, Mob_Hostile_Zombie.refID);
        spawnedMobs.Add(enemy);
        availableSpawnPoints_H3.Remove(spawnCoord);
    }

    private void UpdateAvailableSpawnPointsNearPlayer()
    {
        availableSpawnPoints_H1.Clear();
        availableSpawnPoints_H2.Clear();
        availableSpawnPoints_H3.Clear();

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
                        case 1: { availableSpawnPoints_H1.Add(coord); } break;
                        case 2: { availableSpawnPoints_H1.Add(coord); availableSpawnPoints_H2.Add(coord); } break;
                        case 3: { availableSpawnPoints_H1.Add(coord); availableSpawnPoints_H2.Add(coord); availableSpawnPoints_H3.Add(coord); } break;
                    }
                }
            }
        }
    }

    private int IsSpawnPoint(Vector3Int worldCoord)
    {
        int maxSpawnHeight = -1;

        if (!TerrainManager.singleton.GetWorldCoordBlockExistence(worldCoord + new Vector3Int(0, -1, 0)))
        {
            return maxSpawnHeight;
        }
        
        for (int i = 0; i < 3; i++)
        {
            Vector3Int tileAbove = worldCoord + new Vector3Int(0, i, 0);
            if (!TerrainManager.singleton.GetWorldCoordBlockExistence(tileAbove))
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
}

using UnityEngine;
using System.Collections;

public class PlayerInteractionController : MonoBehaviour
{
    public delegate void OnConstructEventHandler(int index);
    public event OnConstructEventHandler OnConstructEvent;

    public delegate void OnBlockDestroyEventHandler(ItemData itemData, IDurable item, short durabilityChangeAmount);
    public event OnBlockDestroyEventHandler OnBlockDestroyEvent;

    public Transform blockSelectHighlight;
    public LayerMask interactionMask;
    public Transform head;

    private PlayerStats stats;
    private TerrainManager terrainManager;
    private RaycastHit hit; // 存储射线碰撞信息
    private bool isPointingEmpty;

    public bool canConstruct;
    public Transform constructDetector;
    private Coroutine destructionCoroutine; // 摧毁方块Co
    private bool isDestroyingBlock = false;
    private Vector3Int targetBlockWorldCood;
    private Vector3Int neighborBlockWorldCood;
    private TerrainBlockData targetBlockDataToDestroy;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        terrainManager = TerrainManager.singleton;
    }

    void Update()
    {
        // 在Update中检测玩家光标所瞄准的方块
        DetectTargetBlock();
    }

    private void DetectTargetBlock()
    {
        // 创建一条射线从摄像机位置射出，方向为摄像机正前方
        Ray ray = new Ray(head.position, head.forward);

        // 如果射线与某个物体相交
        if (stats.STATE_invPageState == InventoryPageState.None && Physics.Raycast(ray, out hit, stats.INTERACTION_interactionRange, interactionMask))
        {
            Vector3Int hitDir = GetHitPointAndDirection(out Vector3 hitPos);

            targetBlockWorldCood = TerrainManager.singleton.GetHitPointWorldCood(hitPos);
            neighborBlockWorldCood = targetBlockWorldCood + hitDir;
            blockSelectHighlight.position = targetBlockWorldCood + new Vector3(0.5f, 0.5f, 0.5f);
            blockSelectHighlight.rotation = Quaternion.identity;
            blockSelectHighlight.gameObject.SetActive(true);
            isPointingEmpty = false;

            // 切换Construct Detector位置
            constructDetector.position = neighborBlockWorldCood + new Vector3(0.5f, 0.5f, 0.5f);
            constructDetector.rotation = Quaternion.identity;

            // 处理方块破坏 && 交互 && 建造 
            HandleDestruction();
            HandleInteraction();
        }
        else
        {
            // 如果射线没有与物体相交，也绘制一个射线以表示最大交互距离
            //Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
            blockSelectHighlight.gameObject.SetActive(false);
            isPointingEmpty = true;
        }
    }

    private Vector3Int GetHitPointAndDirection(out Vector3 hitPos)
    {
        // 获取所碰到的物体的坐标
        hitPos = hit.point;

        // 获取方块的法线信息
        Vector3 normal = hit.normal;
        Vector3Int hitDir = Vector3Int.zero;

        // 判断射中的是哪个面
        if (normal == Vector3.up)
        {
            // 射中了方块的顶部
            hitPos -= Vector3.up * 0.5f; // 调整为方块中心点的坐标
            hitDir = new Vector3Int(0, 1, 0);
        }
        else if (normal == Vector3.down)
        {
            // 射中了方块的底部
            hitDir = new Vector3Int(0, -1, 0);
        }
        else if (normal == Vector3.left)
        {
            // 射中了方块的左侧面
            hitDir = new Vector3Int(-1, 0, 0);
        }
        else if (normal == Vector3.right)
        {
            // 射中了方块的右侧面
            hitPos -= Vector3.right * 0.5f;
            hitDir = new Vector3Int(1, 0, 0);
        }
        else if (normal == Vector3.forward)
        {
            // 射中了方块的前侧面
            hitPos -= Vector3.forward * 0.5f;
            hitDir = new Vector3Int(0, 0, 1);
        }
        else if (normal == Vector3.back)
        {
            // 射中了方块的后侧面
            hitDir = new Vector3Int(0, 0, -1);
        }

        return hitDir;
    }

    private void HandleInteraction()
    {
        if (HandleBlockInteraction()) return;
        if (HandleBlockConstruction(stats.INTERACTION_blockToBuild)) return;
    }

    private bool HandleBlockInteraction()
    {
        terrainManager.TryGetBlockInfoAt(targetBlockWorldCood, out TerrainBlockData targetBlockData);
        byte blockID = targetBlockData.blockType;

        if (!isPointingEmpty && Input.GetMouseButtonDown(1) && blockID != Block_Empty.refID && terrainManager.IsBlockInteractble(blockID))
        {
            terrainManager.GetBlockInteraction(blockID).Interact();
            return true;
        }

        return false;
    }

    private bool HandleBlockConstruction(byte blockID)
    {
        if (!isPointingEmpty && canConstruct && Input.GetMouseButtonDown(1) && blockID != Block_Empty.refID)
        {
            if (terrainManager.IsBlockDirectional(blockID)) terrainManager.ConstructBlockAt(neighborBlockWorldCood, blockID, CalculateBuildDirection(hit.point, head.position));
            else terrainManager.ConstructBlockAt(neighborBlockWorldCood, blockID);
            OnConstructEvent(stats.EQUIPMENT_toolbarIndex);

            return true;
        }

        return false;
    }

    private void HandleDestruction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (destructionCoroutine == null)
            {
                StartDestructionCoroutine();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopDestructionCoroutine();
        }
    }

    private void StartDestructionCoroutine()
    {
        destructionCoroutine = StartCoroutine(DestructionCoroutine());
    }

    private void StopDestructionCoroutine()
    {
        if (destructionCoroutine != null)
        {
            StopCoroutine(destructionCoroutine);
            isDestroyingBlock = false;
            destructionCoroutine = null;
        }
    }

    IEnumerator DestructionCoroutine()
    {
        // 检查是否已经在破坏方块
        if (isDestroyingBlock)
        {
            yield break;
        }

        // 尝试获取玩家瞄准的方块信息
        Vector3Int initCood = targetBlockWorldCood;
        if (terrainManager.TryGetBlockInfoAt(initCood, out targetBlockDataToDestroy))
        {
            // 在这里可以根据targetBlockData的toughness值来设置破坏时间
            Block blockData = ResourceAssets.singleton.blocks[targetBlockDataToDestroy.blockType];
            float blockToughness = GetBlockToughness(blockData);
            float destructionTime = GetDestructionTime(blockData, blockToughness);

            // 开始破坏
            isDestroyingBlock = true;
            var dt = 0f;

            while (true)
            {
                // 是否松开
                if (!Input.GetMouseButton(0))
                {
                    isDestroyingBlock = false;
                    destructionCoroutine = null;
                    yield break;
                }

                // 检查在鼠标按下的时候是否切换方块
                if (initCood != targetBlockWorldCood && terrainManager.TryGetBlockInfoAt(targetBlockWorldCood, out targetBlockDataToDestroy))
                {
                    if (targetBlockDataToDestroy.IsBlockEmpty())
                    {
                        yield break;
                    }

                    initCood = targetBlockWorldCood;
                    dt = 0f;
                    blockData = ResourceAssets.singleton.blocks[targetBlockDataToDestroy.blockType];
                    blockToughness = GetBlockToughness(blockData);
                    destructionTime = GetDestructionTime(blockData, blockToughness);
                }

                // 计算破坏进度  
                if (isPointingEmpty)
                {
                    dt = 0;
                }
                else
                {
                    dt += Time.deltaTime;
                }
                
                // 进度到了
                if (dt / destructionTime >= 1f)
                {
                    // 完成破坏
                    TerrainManager.singleton.DestroyBlockAt(targetBlockWorldCood);

                    // 扣除耐久
                    /*
                     * 后期根据方块类型扣除不通用的数值
                     * 目前暂且设置为1
                    */
                    if (stats.EQUIPMENT_equippedItem is IDurable)
                    {
                        OnBlockDestroyEvent(stats.EQUIPMENT_equippedItemData, stats.EQUIPMENT_equippedItem as IDurable, -1);
                    }
                }

                yield return null;
            }
        }
        else
        {
            isDestroyingBlock = false;
            destructionCoroutine = null;
            yield break;
        }
    }
    
    private float GetBlockToughness(Block blockData)
    {
        return blockData.toughness;
    }

    private float GetDestructionTime(Block blockData, float blockToughness)
    {
        if (blockData is IBlock_Invincible)
        {
            return Mathf.Infinity;
        }

        float destroyEfficiency = stats.INTERACTION_destroyEfficiency;

        if (blockData is IBlock_SandMud)
        {
            destroyEfficiency += stats.INTERACTION_destroyEfficiencyBonus_SandMud;
        }

        if (blockData is IBlock_Plant)
        {
            destroyEfficiency += stats.INTERACTION_destroyEfficiencyBonus_Plant;
        }

        if (blockData is IBlock_Stone)
        {
            destroyEfficiency += stats.INTERACTION_destroyEfficiencyBonus_Stone;
        }

        if (blockData is IBlock_Wood)
        {
            destroyEfficiency += stats.INTERACTION_destroyEfficiencyBonus_Wood;
        }

        return blockToughness / destroyEfficiency;
    }

    private Vector3 CalculateBuildDirection(Vector3 hitPoint, Vector3 headPosition)
    {
        Vector3 direction = headPosition - hitPoint;

        // 先决定在x或z方向上只保留一个非0值
        float absX = Mathf.Abs(direction.x);
        float absZ = Mathf.Abs(direction.z);
        bool useX = absX > absZ;

        // 使用条件运算符决定哪个方向分量归一化
        int dirX = useX ? NormalizeDirectionComponent(direction.x) : 0;
        int dirZ = useX ? 0 : NormalizeDirectionComponent(direction.z);
        int dirY = NormalizeDirectionComponent(direction.y);

        Vector3Int buildDirection = new Vector3Int(dirX, dirY, dirZ);

        return buildDirection;
    }

    private int NormalizeDirectionComponent(float component)
    {
        if (component > 0)
            return 1;
        else if (component < 0)
            return -1;
        else
            return 0;
    }

}
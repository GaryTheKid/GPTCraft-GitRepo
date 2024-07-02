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
    private RaycastHit hit; // �洢������ײ��Ϣ
    private bool isPointingEmpty;

    public bool canConstruct;
    public Transform constructDetector;
    private Coroutine destructionCoroutine; // �ݻٷ���Co
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
        // ��Update�м����ҹ������׼�ķ���
        DetectTargetBlock();
    }

    private void DetectTargetBlock()
    {
        // ����һ�����ߴ������λ�����������Ϊ�������ǰ��
        Ray ray = new Ray(head.position, head.forward);

        // ���������ĳ�������ཻ
        if (stats.STATE_invPageState == InventoryPageState.None && Physics.Raycast(ray, out hit, stats.INTERACTION_interactionRange, interactionMask))
        {
            Vector3Int hitDir = GetHitPointAndDirection(out Vector3 hitPos);

            targetBlockWorldCood = TerrainManager.singleton.GetHitPointWorldCood(hitPos);
            neighborBlockWorldCood = targetBlockWorldCood + hitDir;
            blockSelectHighlight.position = targetBlockWorldCood + new Vector3(0.5f, 0.5f, 0.5f);
            blockSelectHighlight.rotation = Quaternion.identity;
            blockSelectHighlight.gameObject.SetActive(true);
            isPointingEmpty = false;

            // �л�Construct Detectorλ��
            constructDetector.position = neighborBlockWorldCood + new Vector3(0.5f, 0.5f, 0.5f);
            constructDetector.rotation = Quaternion.identity;

            // �������ƻ� && ���� && ���� 
            HandleDestruction();
            HandleInteraction();
        }
        else
        {
            // �������û���������ཻ��Ҳ����һ�������Ա�ʾ��󽻻�����
            //Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.red);
            blockSelectHighlight.gameObject.SetActive(false);
            isPointingEmpty = true;
        }
    }

    private Vector3Int GetHitPointAndDirection(out Vector3 hitPos)
    {
        // ��ȡ�����������������
        hitPos = hit.point;

        // ��ȡ����ķ�����Ϣ
        Vector3 normal = hit.normal;
        Vector3Int hitDir = Vector3Int.zero;

        // �ж����е����ĸ���
        if (normal == Vector3.up)
        {
            // �����˷���Ķ���
            hitPos -= Vector3.up * 0.5f; // ����Ϊ�������ĵ������
            hitDir = new Vector3Int(0, 1, 0);
        }
        else if (normal == Vector3.down)
        {
            // �����˷���ĵײ�
            hitDir = new Vector3Int(0, -1, 0);
        }
        else if (normal == Vector3.left)
        {
            // �����˷���������
            hitDir = new Vector3Int(-1, 0, 0);
        }
        else if (normal == Vector3.right)
        {
            // �����˷�����Ҳ���
            hitPos -= Vector3.right * 0.5f;
            hitDir = new Vector3Int(1, 0, 0);
        }
        else if (normal == Vector3.forward)
        {
            // �����˷����ǰ����
            hitPos -= Vector3.forward * 0.5f;
            hitDir = new Vector3Int(0, 0, 1);
        }
        else if (normal == Vector3.back)
        {
            // �����˷���ĺ����
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
        // ����Ƿ��Ѿ����ƻ�����
        if (isDestroyingBlock)
        {
            yield break;
        }

        // ���Ի�ȡ�����׼�ķ�����Ϣ
        Vector3Int initCood = targetBlockWorldCood;
        if (terrainManager.TryGetBlockInfoAt(initCood, out targetBlockDataToDestroy))
        {
            // ��������Ը���targetBlockData��toughnessֵ�������ƻ�ʱ��
            Block blockData = ResourceAssets.singleton.blocks[targetBlockDataToDestroy.blockType];
            float blockToughness = GetBlockToughness(blockData);
            float destructionTime = GetDestructionTime(blockData, blockToughness);

            // ��ʼ�ƻ�
            isDestroyingBlock = true;
            var dt = 0f;

            while (true)
            {
                // �Ƿ��ɿ�
                if (!Input.GetMouseButton(0))
                {
                    isDestroyingBlock = false;
                    destructionCoroutine = null;
                    yield break;
                }

                // �������갴�µ�ʱ���Ƿ��л�����
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

                // �����ƻ�����  
                if (isPointingEmpty)
                {
                    dt = 0;
                }
                else
                {
                    dt += Time.deltaTime;
                }
                
                // ���ȵ���
                if (dt / destructionTime >= 1f)
                {
                    // ����ƻ�
                    TerrainManager.singleton.DestroyBlockAt(targetBlockWorldCood);

                    // �۳��;�
                    /*
                     * ���ڸ��ݷ������Ϳ۳���ͨ�õ���ֵ
                     * Ŀǰ��������Ϊ1
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

        // �Ⱦ�����x��z������ֻ����һ����0ֵ
        float absX = Mathf.Abs(direction.x);
        float absZ = Mathf.Abs(direction.z);
        bool useX = absX > absZ;

        // ʹ����������������ĸ����������һ��
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
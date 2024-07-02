using System.Collections.Generic;
using UnityEngine;

public class MobWorldController : MonoBehaviour
{
    private Pathfinding pf;
    private Rigidbody rb;
    private MobBrain mobBrain;

    public Transform avatar;
    public Mob mobClass;
    public MobData mobData;

    public Vector3Int worldCoord;
    public List<Vector3Int> path;
    public bool hasHillAtFront;
    private Pathfinding.Node[,,] allNodes;
    private List<Pathfinding.Node> walkableNodes;
    private int targetIndex = 0; // 当前路径点索引

    public Transform chaseTarget;
    private Vector3Int chaseTargetCoord;

    private void Awake()
    {
        pf = GetComponent<Pathfinding>();
        rb = GetComponent<Rigidbody>();
        mobBrain = GetComponent<MobBrain>();
    }

    private void Start()
    {
        mobData.hp = mobClass.maxHP; // 初始化当前生命值为最大生命值
    }

    private void OnEnable()
    {
        WorldObjectSpawner.singleton.activeSpawnedMobs.Add(gameObject);
    }

    private void OnDisable()
    {
        WorldObjectSpawner.singleton.activeSpawnedMobs.Remove(gameObject);

        ResetMob();
        mobBrain.ResetMobBrain();
    }

    private void OnDestroy()
    {
        WorldObjectSpawner.singleton.spawnedMobs.Remove(gameObject);
    }

    private void Update()
    {
        HandleMovement();
    }

    public void SetMobClass(Mob mob)
    {
        mobClass = mob;
        mobData = mobClass.CreateMobData();
        mobBrain.SetMobBrain(mobClass);

        ResetMobStats();
        ResetMob();
    }

    public void ResetMobStats()
    {
        mobData.hp = mobClass.maxHP;
    }

    public void ResetMob()
    {
        path = null;
        allNodes = null;
        walkableNodes = null;
        targetIndex = 0;

        chaseTarget = null;
        chaseTargetCoord = Vector3Int.zero;
    }

    public void UpdateWalkableNodes()
    {
        allNodes = pf.FindWalkNodes(worldCoord, mobClass.explorationRange, mobClass.mobHeight);
        walkableNodes = new List<Pathfinding.Node>();

        // 遍历所有节点，将可行走的节点录入到 walkableNodeList 中
        foreach (Pathfinding.Node node in allNodes)
        {
            if (node.walkable)
            {
                walkableNodes.Add(node);
            }
        }
    }

    public void RoamToRandomCoord()
    {
        if (allNodes == null || allNodes.Length == 0)
        {
            return;
        }

        if (walkableNodes == null || walkableNodes.Count == 0)
        {
            return;
        }

        // 从 walkableNodes 中随机选择一个 Node
        Vector3Int randomCoord = walkableNodes[Random.Range(0, walkableNodes.Count)].position;

        // 其他逻辑...
        targetIndex = 0;
        path = pf.FindPath(allNodes, worldCoord, randomCoord, mobClass.explorationRange);
    }

    public void UpdateChaseTargetCoord()
    {
        // 定义射线起点为当前位置
        Vector3 rayOrigin = chaseTarget.position;

        // 定义射线方向为正下方
        Vector3 rayDirection = Vector3.down;

        // 定义射线长度为无限大
        float rayLength = Mathf.Infinity;

        // 执行射线检测
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, LayerMask.GetMask("Default")))
        {
            // 如果射线击中了地面格子
            var hitPoint = hit.point;

            // 将击中点的位置设置为 aiClass 的世界坐标
            chaseTargetCoord = new Vector3Int(Mathf.FloorToInt(hitPoint.x), Mathf.FloorToInt(hitPoint.y), Mathf.FloorToInt(hitPoint.z));
        }
    }

    public void ChaseTargetCoord()
    {
        if (allNodes == null || allNodes.Length == 0)
        {
            return;
        }

        if (chaseTarget == null)
        {
            return;
        }

        targetIndex = 0;
        path = pf.FindPath(allNodes, worldCoord, chaseTargetCoord, mobClass.explorationRange);
    }

    public void HandleMovement()
    {
        if (path == null || path.Count == 0)
        {
            // 如果没有路径或者路径为空，则不进行移动
            return;
        }

        // 获取当前目标路径点
        Vector3Int targetPos = path[targetIndex];
        Vector3 targetWorldPos = new Vector3(targetPos.x + 0.5f, transform.position.y, targetPos.z + 0.5f);

        // 计算当前位置到目标位置的方向
        Vector3 dir = (targetWorldPos - transform.position).normalized;

        // 计算目标旋转方向
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        // 平滑地改变朝向 (AVATAR)
        avatar.rotation = Quaternion.Lerp(avatar.rotation, targetRotation, mobClass.turnSpeed * Time.deltaTime);

        // 如果目标点在当前位置的上方，则执行跳跃
        if (hasHillAtFront)
        {
            Jump();
        }

        // 水平移动
        transform.Translate(dir * mobClass.moveSpeed * Time.deltaTime, Space.World);

        // 如果敌人接近目标位置，则更新目标索引到下一个路径点
        if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        {
            targetIndex++;
            // 如果已经到达路径的最后一个点，则清空路径
            if (targetIndex >= path.Count)
            {
                path.Clear();
                targetIndex = 0;
            }
        }
    }

    private void Jump()
    {
        // 施加一个向上的力来执行跳跃
        rb.AddForce(Vector3.up * mobClass.jumpForce * Time.deltaTime);
    }

    public void TakeDamage(short damage)
    {
        if (mobData.hp > 0)
        {
            mobData.hp -= damage; // 减去伤害值
            if (mobData.hp <= 0)
            {
                Die(); // 如果生命值小于等于0，死亡
            }
            else
            {
                Debug.Log("Enemy took " + damage + " damage. Current HP: " + mobData.hp);
            }
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        // 此处可以添加敌人死亡的逻辑，例如播放死亡动画、产生死亡效果等
        // 你可以在这里销毁敌人对象或进行其他你需要的处理

        Destroy(gameObject);
    }

    public void SetWorldCoord(Vector3 worldPosXZ)
    {
        worldCoord = new Vector3Int(Mathf.FloorToInt(worldPosXZ.x), Mathf.FloorToInt(worldPosXZ.y), Mathf.FloorToInt(worldPosXZ.z));
    }
}

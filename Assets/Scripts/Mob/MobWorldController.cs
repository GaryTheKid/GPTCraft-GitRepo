using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobWorldController : MonoBehaviour
{
    // singletons
    private TimeManager timeManager;
    private WorldObjectSpawner worldObjectSpawner;

    // binding properties
    private Pathfinding pf;
    private Rigidbody rb;
    private MobBrain mobBrain;

    [Header("====== Mob Properties ======")]
    public Transform avatar;
    public Mob mobClass;
    public MobData mobData;
    private Vector3 lastDamageTakenDir;
    private Coroutine terrifiedCoroutine;
    private Coroutine sunburnCoroutine;

    [Header("====== Path Finding ======")]
    public Vector3Int worldCoord;
    public List<Vector3Int> path;
    public bool hasHillAtFront;
    private Pathfinding.Node[,,] allNodes;
    private List<Pathfinding.Node> walkableNodes;
    private int targetIndex = 0; // ��ǰ·��������
    public Transform chaseTarget;
    private Vector3Int chaseTargetCoord;


    #region Unity Functions
    private void Awake()
    {
        pf = GetComponent<Pathfinding>();
        rb = GetComponent<Rigidbody>();
        mobBrain = GetComponent<MobBrain>();
    }
    private void Start()
    {
        mobData.hp = mobClass.maxHP; // ��ʼ����ǰ����ֵΪ�������ֵ
        timeManager = TimeManager.singleton;
        worldObjectSpawner = WorldObjectSpawner.singleton;
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
        HandleSunburn();
    }
    #endregion


    #region Mob Setting
    public void SetMobClass(Mob mob)
    {
        mobClass = mob;
        mobData = mobClass.CreateMobData();
        mobBrain.SetMobBrain(mobClass, mobData);

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
    #endregion


    #region Pathfinding
    public void UpdateWalkableNodes()
    {
        allNodes = pf.FindWalkNodes(worldCoord, mobClass.explorationRange, mobClass.mobHeight);
        walkableNodes = new List<Pathfinding.Node>();

        // �������нڵ㣬�������ߵĽڵ�¼�뵽 walkableNodeList ��
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

        // �� walkableNodes �����ѡ��һ�� Node
        Vector3Int randomCoord = walkableNodes[Random.Range(0, walkableNodes.Count)].position;

        // �����߼�...
        targetIndex = 0;
        path = pf.FindPath(allNodes, worldCoord, randomCoord, mobClass.explorationRange);
    }
    public void FleeToRandomCoord()
    {
        if (allNodes == null || allNodes.Length == 0)
        {
            return;
        }

        if (walkableNodes == null || walkableNodes.Count == 0)
        {
            return;
        }

        List<Pathfinding.Node> filteredNodes = new List<Pathfinding.Node>();

        // �����ϴ��յ��˺��ķ���������ѡ�񲿷ֿ��ƶ��ĸ���
        if (lastDamageTakenDir != Vector3.zero)
        {
            foreach (Pathfinding.Node node in walkableNodes)
            {
                Vector3Int dirInt = node.position - worldCoord;
                Vector3 nodeDir = new Vector3(dirInt.x, dirInt.y, dirInt.z).normalized;
                // �������һ�£����뵽 filteredNodes
                if (Vector3.Dot(nodeDir, lastDamageTakenDir.normalized) > 0.5f) // ����� 0.5 ��һ����ֵ�����Ը�����Ҫ����
                {
                    filteredNodes.Add(node);
                }
            }
        }

        // ���û�з��������Ľڵ㣬�����ѡ��һ���ڵ�
        if (filteredNodes.Count == 0)
        {
            filteredNodes = walkableNodes;
        }

        Vector3Int randomCoord = filteredNodes[Random.Range(0, filteredNodes.Count)].position;

        // �����߼�...
        targetIndex = 0;
        path = pf.FindPath(allNodes, worldCoord, randomCoord, mobClass.explorationRange);
    }
    public void UpdateChaseTargetCoord()
    {
        // �����������Ϊ��ǰλ��
        Vector3 rayOrigin = chaseTarget.position;

        // �������߷���Ϊ���·�
        Vector3 rayDirection = Vector3.down;

        // �������߳���Ϊ���޴�
        float rayLength = Mathf.Infinity;

        // ִ�����߼��
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayLength, LayerMask.GetMask("Default")))
        {
            // ������߻����˵������
            var hitPoint = hit.point;

            // �����е��λ������Ϊ aiClass ����������
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
    #endregion


    #region Movement
    private void HandleMovement()
    {
        if (path == null || path.Count == 0)
        {
            // ���û��·������·��Ϊ�գ��򲻽����ƶ�
            return;
        }

        // ��ȡ��ǰĿ��·����
        Vector3Int targetPos = path[targetIndex];
        Vector3 targetWorldPos = new Vector3(targetPos.x + 0.5f, transform.position.y, targetPos.z + 0.5f);

        // ���㵱ǰλ�õ�Ŀ��λ�õķ���
        Vector3 dir = (targetWorldPos - transform.position).normalized;

        // ����Ŀ����ת����
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        // ƽ���ظı䳯�� (AVATAR)
        avatar.rotation = Quaternion.Lerp(avatar.rotation, targetRotation, mobClass.turnSpeed * Time.deltaTime);

        // ���Ŀ����ڵ�ǰλ�õ��Ϸ�����ִ����Ծ
        if (hasHillAtFront)
        {
            Jump();
        }

        // ˮƽ�ƶ�
        transform.Translate(dir * mobClass.moveSpeed * Time.deltaTime, Space.World);

        // ������˽ӽ�Ŀ��λ�ã������Ŀ����������һ��·����
        if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        {
            targetIndex++;
            // ����Ѿ�����·�������һ���㣬�����·��
            if (targetIndex >= path.Count)
            {
                path.Clear();
                targetIndex = 0;
            }
        }
    }
    private void Jump()
    {
        // ʩ��һ�����ϵ�����ִ����Ծ
        rb.AddForce(Vector3.up * mobClass.jumpForce * Time.deltaTime);
    }
    public void UpdateWorldCoord(Vector3 worldPosXZ)
    {
        worldCoord = new Vector3Int(Mathf.FloorToInt(worldPosXZ.x), Mathf.FloorToInt(worldPosXZ.y), Mathf.FloorToInt(worldPosXZ.z));
    }
    #endregion


    #region Survival
    public void TakeDamage(Transform attacker, short damage)
    {
        if (mobData.hp > 0)
        {
            lastDamageTakenDir = transform.position - attacker.position; // �����յ��˺��ķ���
            mobData.hp -= damage; // ��ȥ�˺�ֵ
            if (mobData.hp <= 0)
            {
                Die(attacker); // �������ֵС�ڵ���0������
            }
            else
            {
                Debug.Log("Enemy took " + damage + " damage. Current HP: " + mobData.hp);
                StartTerrified(mobClass.terrifiedStateLength);
            }
        }
    }
    public void StartTerrified(float duration)
    {
        if (terrifiedCoroutine != null)
        {
            StopCoroutine(terrifiedCoroutine);
        }
        terrifiedCoroutine = StartCoroutine(Terrified(duration));
    }
    private IEnumerator Terrified(float duration)
    {
        mobData.isTerrified = true;
        mobData.moveSpeed = mobData.terrifiedMoveSpeed;
        mobData.turnSpeed = mobData.terrifiedturnSpeed;
        Debug.Log("Enemy is terrified!");

        yield return new WaitForSeconds(duration);

        mobData.isTerrified = false;
        mobData.moveSpeed = mobClass.moveSpeed;
        mobData.turnSpeed = mobClass.turnSpeed;
        Debug.Log("Enemy is no longer terrified!");

        terrifiedCoroutine = null;
    }
    private void HandleSunburn()
    {
        if (mobClass.isSunlightSensitive && timeManager.CurrentDayNightState == TimeManager.DayNightState.Day) InitSunburn();
        else StopSunburn();
    }
    private void InitSunburn()
    {
        if (sunburnCoroutine == null)
        {
            sunburnCoroutine = StartCoroutine(Sunburn());
        }
    }
    private void StopSunburn()
    {
        if (sunburnCoroutine != null)
        {
            StopCoroutine(sunburnCoroutine);
            sunburnCoroutine = null;
        }
    }
    private IEnumerator Sunburn()
    {
        while (mobClass.isSunlightSensitive && timeManager.CurrentDayNightState == TimeManager.DayNightState.Day)
        {
            TakeDamage(transform, DefaultStats.GLOBAL_STATS_SUNBURN_DAMAGE);
            yield return new WaitForSeconds(DefaultStats.GLOBAL_STATS_SUNBURN_CD);
        }

        sunburnCoroutine = null;
    }
    private void Die(Transform attacker)
    {
        Debug.Log("Enemy died!");
        // �˴�������ӵ����������߼������粥��������������������Ч����
        // ��������������ٵ��˶���������������Ҫ�Ĵ���

        // �����ɱ�߾���ֵ
        PlayerEXPController playerEXPController = attacker.GetComponent<PlayerEXPController>();
        if (playerEXPController != null) playerEXPController.GainExperience(mobClass.killEXPWorth);

        // ������
        worldObjectSpawner.SpawnItem(GetMobPosInt(), mobClass.mobDeathDropItem.id, 1);

        Destroy(gameObject);
    }
    #endregion


    #region Utilities
    private Vector3Int GetMobPosInt()
    {
        Vector3 mobPos = transform.position;
        return new Vector3Int((int)mobPos.x, (int)mobPos.y, (int)mobPos.z);
    }
    #endregion
}

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
    private int targetIndex = 0; // ��ǰ·��������

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
        mobData.hp = mobClass.maxHP; // ��ʼ����ǰ����ֵΪ�������ֵ
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

    public void HandleMovement()
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

    public void TakeDamage(short damage)
    {
        if (mobData.hp > 0)
        {
            mobData.hp -= damage; // ��ȥ�˺�ֵ
            if (mobData.hp <= 0)
            {
                Die(); // �������ֵС�ڵ���0������
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
        // �˴�������ӵ����������߼������粥��������������������Ч����
        // ��������������ٵ��˶���������������Ҫ�Ĵ���

        Destroy(gameObject);
    }

    public void SetWorldCoord(Vector3 worldPosXZ)
    {
        worldCoord = new Vector3Int(Mathf.FloorToInt(worldPosXZ.x), Mathf.FloorToInt(worldPosXZ.y), Mathf.FloorToInt(worldPosXZ.z));
    }
}

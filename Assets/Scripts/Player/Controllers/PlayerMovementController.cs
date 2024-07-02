using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public LayerMask groundLayer; // 地面层

    private PlayerStats stats;
    private CharacterController controller;

    private float lastTapTime = 0f;
    private bool isSprinting = false;
    private float verticalVelocity = 0f;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 更新全局玩家坐标位置
        PlayerStats.Global_playerWorldCoord = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));

        // 获取玩家输入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 检测双击W键
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastTapTime < stats.MOVEMENT_doubleTapTime)
            {
                isSprinting = true;
            }
            lastTapTime = Time.time;
        }

        // 松开W键后重置跑步状态
        if (Input.GetKeyUp(KeyCode.W))
        {
            isSprinting = false;
        }

        // 计算移动向量
        Vector3 moveDirection = (stats.STATE_invPageState != InventoryPageState.None) ? Vector3.zero : (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        Vector3 moveVelocity = isSprinting ? moveDirection * stats.MOVEMENT_sprintSpeed : moveDirection * stats.MOVEMENT_moveSpeed;

        // 检测是否在地面上
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, stats.MOVEMENT_groundCheckDistance, groundLayer);

        // 跳跃
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(2 * stats.MOVEMENT_jumpForce * stats.MOVEMENT_gravity);
            }
            else
            {
                verticalVelocity = -0.5f * stats.MOVEMENT_gravity * Time.deltaTime;
            }
        }
        else
        {
            verticalVelocity -= stats.MOVEMENT_gravity * Time.deltaTime;
        }

        // 合并移动向量和垂直速度
        Vector3 finalVelocity = new Vector3(moveVelocity.x, verticalVelocity, moveVelocity.z);

        // 应用移动力到角色控制器
        controller.Move(finalVelocity * Time.deltaTime);
    }
}

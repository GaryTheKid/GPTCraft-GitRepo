using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    public LayerMask groundLayer; // �����

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
        // ����ȫ���������λ��
        PlayerStats.Global_playerWorldCoord = new Vector3Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y), Mathf.FloorToInt(transform.position.z));

        // ��ȡ�������
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // ���˫��W��
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastTapTime < stats.MOVEMENT_doubleTapTime)
            {
                isSprinting = true;
            }
            lastTapTime = Time.time;
        }

        // �ɿ�W���������ܲ�״̬
        if (Input.GetKeyUp(KeyCode.W))
        {
            isSprinting = false;
        }

        // �����ƶ�����
        Vector3 moveDirection = (stats.STATE_invPageState != InventoryPageState.None) ? Vector3.zero : (transform.forward * verticalInput + transform.right * horizontalInput).normalized;
        Vector3 moveVelocity = isSprinting ? moveDirection * stats.MOVEMENT_sprintSpeed : moveDirection * stats.MOVEMENT_moveSpeed;

        // ����Ƿ��ڵ�����
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, stats.MOVEMENT_groundCheckDistance, groundLayer);

        // ��Ծ
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

        // �ϲ��ƶ������ʹ�ֱ�ٶ�
        Vector3 finalVelocity = new Vector3(moveVelocity.x, verticalVelocity, moveVelocity.z);

        // Ӧ���ƶ�������ɫ������
        controller.Move(finalVelocity * Time.deltaTime);
    }
}

using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Global")]
    public static Vector3Int Global_playerWorldCoord;

    [Header("Survival")]
    public byte SURVIVAL_maxHP = 20;
    public byte SURVIVAL_HP = 20;

    [Header("Movement")]
    public float MOVEMENT_moveSpeed = 5.0f; // 移动速度
    public float MOVEMENT_sprintSpeed = 10.0f; // 跑步速度
    public float MOVEMENT_gravity = 9.81f; // 重力大小
    public float MOVEMENT_jumpForce = 5.0f; // 跳跃力大小
    public float MOVEMENT_groundCheckDistance = 0.2f; // 地面检测距离
    public float MOVEMENT_doubleTapTime = 0.2f; // 双击间隔时间

    [Header("View")]
    public float VIEW_sensitivity = 2.0f; // 控制视角旋转的灵敏度

    [Header("Interaction")]
    public float INTERACTION_interactionRange = 5f; // 设置玩家与方块交互的最大距离
    public float INTERACTION_destroyEfficiency = 1f;
    public float INTERACTION_destroyEfficiencyBonus_Wood;
    public float INTERACTION_destroyEfficiencyBonus_Stone;
    public float INTERACTION_destroyEfficiencyBonus_SandMud;
    public float INTERACTION_destroyEfficiencyBonus_Plant;
    public short INTERACTION_attackDamage; // attack
    public float INTERACTION_attackSpeed;
    public byte INTERACTION_blockToBuild = 0;

    [Header("Equipment")]
    public byte EQUIPMENT_toolbarIndex;
    public Item EQUIPMENT_equippedItem;
    public ItemData EQUIPMENT_equippedItemData;
    public short EQUIPMENT_maxDurability;
    public short EQUIPMENT_durability;

    [Header("Flag")]
    public bool FLAG_isInventoryFull;

    [Header("State")]
    public InventoryPageState STATE_invPageState;
}

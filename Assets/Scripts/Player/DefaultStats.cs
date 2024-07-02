public class DefaultStats
{
    // movement
    public const float MOVEMENT_moveSpeed = 5.0f; // 移动速度
    public const float MOVEMENT_sprintSpeed = 10.0f; // 跑步速度
    public const float MOVEMENT_gravity = 20f; // 重力大小
    public const float MOVEMENT_jumpForce = 2.0f; // 跳跃力大小
    public const float MOVEMENT_groundCheckDistance = 0.84f; // 地面检测距离
    public const float MOVEMENT_doubleTapTime = 0.2f; // 双击间隔时间

    // view
    public const float VIEW_sensitivity = 2.0f;

    // interaction
    public const float INTERACTION_interactionRange = 5f; // 设置玩家与方块交互的最大距离
    public const float INTERACTION_destroyEfficiency = 1f;
    public const float INTERACTION_destroyEfficiencyBonus_Wood = 0f;
    public const float INTERACTION_destroyEfficiencyBonus_Stone = 0f;
    public const float INTERACTION_destroyEfficiencyBonus_SandMud = 0f;
    public const float INTERACTION_destroyEfficiencyBonus_Plant = 0f;
    public const short INTERACTION_attackDamage = 1;
    public const float INTERACTION_attackSpeed = 0.5f;

    // equipment
    public const byte EQUIPMENT_toolbarIndex = 0;
    public static ItemData EQUIPMENT_equippedItem = ResourceAssets.singleton.items[0].CreateItemData();
    public const short EQUIPMENT_maxDurability = 0;
    public const short EQUIPMENT_durability = 0;
}

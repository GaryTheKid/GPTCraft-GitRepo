using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Global")]
    public static Vector3Int Global_playerWorldCoord;

    [Header("Survival")]
    public byte SURVIVAL_maxHP = 20;
    public byte SURVIVAL_HP = 20;

    [Header("Movement")]
    public float MOVEMENT_moveSpeed = 5.0f; // �ƶ��ٶ�
    public float MOVEMENT_sprintSpeed = 10.0f; // �ܲ��ٶ�
    public float MOVEMENT_gravity = 9.81f; // ������С
    public float MOVEMENT_jumpForce = 5.0f; // ��Ծ����С
    public float MOVEMENT_groundCheckDistance = 0.2f; // ���������
    public float MOVEMENT_doubleTapTime = 0.2f; // ˫�����ʱ��

    [Header("View")]
    public float VIEW_sensitivity = 2.0f; // �����ӽ���ת��������

    [Header("Interaction")]
    public float INTERACTION_interactionRange = 5f; // ��������뷽�齻����������
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

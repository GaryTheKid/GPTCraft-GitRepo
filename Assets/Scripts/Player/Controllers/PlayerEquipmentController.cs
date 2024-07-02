using System.Collections;
using UnityEngine;

public class PlayerEquipmentController : MonoBehaviour
{
    public delegate ItemData SwitchEquipmentEventHandler(int index);
    public event SwitchEquipmentEventHandler SwitchEquipmentEvent;

    private PlayerStats stats;

    // 定义一个映射表，将数字键对应到工具栏的索引
    private KeyCode[] toolbarKeys = {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
    };

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        InitializeEquipment();
    }

    private void Update()
    {
        // 检测数字键 1 到 9 和 0
        for (int i = 0; i < toolbarKeys.Length; i++)
        {
            if (Input.GetKeyDown(toolbarKeys[i]))
            {
                HandleEquipment(i);
            }
        }

        /*// Test
        if (Input.GetKeyDown(KeyCode.O))
        {
            Item item = GetComponent<PlayerInventoryController>().Toolbar[0].item;
            if (item is IDurable)
            {
                (item as IDurable).UpdateDurability(+10);
                GetComponent<PlayerInventoryController>().OnInventoryUIUpdate.Invoke();
                Debug.Log(stats.EQUIPMENT_durability + "/" + stats.EQUIPMENT_maxDurability);
            }
            else
            {
                Debug.Log("Item not Durable");
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Item item = GetComponent<PlayerInventoryController>().Toolbar[0].item;
            if (item is IDurable)
            {
                (item as IDurable).UpdateDurability(-10);
                GetComponent<PlayerInventoryController>().OnInventoryUIUpdate.Invoke();
                Debug.Log(stats.EQUIPMENT_durability + "/" + stats.EQUIPMENT_maxDurability);
            }
            else
            {
                Debug.Log("Item not Durable");
            }
        }*/
    }

    private void InitializeEquipment()
    {
        StartCoroutine(InitializeEquipmentCoroutine());
    }
    private IEnumerator InitializeEquipmentCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // 装备对应位置的工具栏道具
        ItemData itemToEquip = SwitchEquipmentEvent(0);
        if (itemToEquip == null)
        {
            UpdateEquipment(0, DefaultStats.EQUIPMENT_equippedItem);
        }
        else
        {
            UpdateEquipment(0, itemToEquip);
        }
    }

    private void HandleEquipment(int index)
    {
        if (stats.EQUIPMENT_toolbarIndex == index)
        {
            return;
        }

        // 装备对应位置的工具栏道具
        ItemData itemToEquip = SwitchEquipmentEvent(index);
        if (itemToEquip == null)
        {
            return;
        }

        UpdateEquipment((byte)index, itemToEquip);
    }

    public void UpdateEquipment(byte newIndex, ItemData itemData)
    {
        Item itemToEquip = ResourceAssets.singleton.items[itemData.id];

        // index !=
        if (stats.EQUIPMENT_toolbarIndex != newIndex)
        {
            stats.EQUIPMENT_toolbarIndex = newIndex;
        }

        // item !=
        if (stats.EQUIPMENT_equippedItem == null || (stats.EQUIPMENT_equippedItem.GetType() != itemToEquip.GetType()))
        {
            if (itemToEquip is IConstructable)
            {
                stats.INTERACTION_blockToBuild = (itemToEquip as IConstructable).GetItemBlockID();
            }
            else
            {
                stats.INTERACTION_blockToBuild = Block_Empty.refID;
            }

            if (itemToEquip is ITool)
            {
                stats.INTERACTION_destroyEfficiency = (itemToEquip as ITool).GetDestroyEfficiency();
                stats.INTERACTION_destroyEfficiencyBonus_Plant = (itemToEquip as ITool).GetDestroyEffiencyBonus_Plant();
                stats.INTERACTION_destroyEfficiencyBonus_SandMud = (itemToEquip as ITool).GetDestroyEffiencyBonus_SandMud();
                stats.INTERACTION_destroyEfficiencyBonus_Stone = (itemToEquip as ITool).GetDestroyEffiencyBonus_Stone();
                stats.INTERACTION_destroyEfficiencyBonus_Wood = (itemToEquip as ITool).GetDestroyEffiencyBonus_Wood();
            }
            else
            {
                stats.INTERACTION_destroyEfficiency = DefaultStats.INTERACTION_destroyEfficiency;
                stats.INTERACTION_destroyEfficiencyBonus_Plant = DefaultStats.INTERACTION_destroyEfficiencyBonus_Plant;
                stats.INTERACTION_destroyEfficiencyBonus_SandMud = DefaultStats.INTERACTION_destroyEfficiencyBonus_SandMud;
                stats.INTERACTION_destroyEfficiencyBonus_Stone = DefaultStats.INTERACTION_destroyEfficiencyBonus_Stone;
                stats.INTERACTION_destroyEfficiencyBonus_Wood = DefaultStats.INTERACTION_destroyEfficiencyBonus_Wood;
            }

            if (itemToEquip is IWeapon)
            {
                stats.INTERACTION_attackDamage = (itemToEquip as IWeapon).GetAttackDamage();
                stats.INTERACTION_attackSpeed = (itemToEquip as IWeapon).GetAttackSpeed();
            }
            else
            {
                stats.INTERACTION_attackDamage = DefaultStats.INTERACTION_attackDamage;
                stats.INTERACTION_attackSpeed = DefaultStats.INTERACTION_attackSpeed;
            }

            if (itemToEquip is IDurable)
            {
                stats.EQUIPMENT_durability = itemData.durability;
                stats.EQUIPMENT_maxDurability = (itemToEquip as IDurable).GetMaxDurability();
            }
            else
            {
                stats.EQUIPMENT_durability = DefaultStats.EQUIPMENT_durability;
                stats.EQUIPMENT_maxDurability = DefaultStats.EQUIPMENT_maxDurability;
            }

            stats.EQUIPMENT_equippedItem = itemToEquip;
            stats.EQUIPMENT_equippedItemData = itemData;
        }

        //print(stats.EQUIPMENT_toolbarIndex + " " + stats.EQUIPMENT_equippedItem);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttractor : MonoBehaviour
{
    public PlayerInventoryController inventoryController;
    public PlayerStats stats;

    public float attractionForce = 10f; // 吸引力的大小
    public float pickupRange = 2f; // 拾取范围
    public LayerMask itemLayer; // 物品所在的Layer

    private List<Rigidbody> attractedItems = new List<Rigidbody>();
    private List<Rigidbody> disabledRigidbodies = new List<Rigidbody>();

    private void Update()
    {
        // 查看Inventory是否已满
        if (stats.FLAG_isInventoryFull) return;

        // 循环吸引物品列表中的物品
        for (int i = attractedItems.Count - 1; i >= 0; i--)
        {
            if (attractedItems[i] == null)
            {
                attractedItems.RemoveAt(i);
                continue;
            }

            if (!attractedItems[i].GetComponent<ItemWorldObject>().isPickable)
            {
                continue;
            }

            // 计算玩家和物品之间的方向
            Vector3 direction = attractedItems[i].transform.position - transform.position;

            // 如果物品在拾取范围内，将其吸引到玩家
            if (direction.magnitude <= pickupRange)
            {
                if (!disabledRigidbodies.Contains(attractedItems[i]))
                {
                    attractedItems[i].isKinematic = true;
                    disabledRigidbodies.Add(attractedItems[i]);
                }

                attractedItems[i].transform.position = Vector3.MoveTowards(attractedItems[i].transform.position, transform.position, Time.deltaTime * attractionForce);

                // 如果物品已经非常靠近，执行拾取操作
                if (direction.magnitude < 0.5f)
                {
                    PickupItem(attractedItems[i].gameObject);
                    attractedItems.RemoveAt(i);
                }
            }
            else
            {
                // 如果物品超出拾取范围，恢复Rigidbody
                if (disabledRigidbodies.Contains(attractedItems[i]))
                {
                    attractedItems[i].isKinematic = false;
                    disabledRigidbodies.Remove(attractedItems[i]);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 使用LayerMask检测物品是否在指定的Layer中
        if (itemLayer == (itemLayer | (1 << other.gameObject.layer)))
        {
            Rigidbody itemRigidbody = other.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                attractedItems.Add(itemRigidbody);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 使用LayerMask检测物品是否在指定的Layer中
        if (itemLayer == (itemLayer | (1 << other.gameObject.layer)))
        {
            Rigidbody itemRigidbody = other.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                attractedItems.Remove(itemRigidbody);

                // 如果物品超出拾取范围，恢复Rigidbody
                if (disabledRigidbodies.Contains(itemRigidbody))
                {
                    itemRigidbody.isKinematic = false;
                    disabledRigidbodies.Remove(itemRigidbody);
                }
            }
        }
    }

    private void PickupItem(GameObject itemWorldObject)
    {
        ItemWorldObject itemWorld = itemWorldObject.GetComponent<ItemWorldObject>();
        int amount = itemWorld.amount;

        // 在这里执行拾取物品的操作，将其添加到玩家的工具栏/背包中，你可以根据自己的游戏逻辑来实现
        Debug.Log("拾取了物品: " + itemWorldObject.name);

        inventoryController.AddItemToInventory(itemWorld.itemData, amount);

        Destroy(itemWorldObject); // 为了示例，这里简单地销毁了物品，你可以根据需求进行修改
    }
}

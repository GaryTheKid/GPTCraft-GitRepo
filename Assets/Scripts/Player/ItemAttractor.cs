using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttractor : MonoBehaviour
{
    public PlayerInventoryController inventoryController;
    public PlayerStats stats;

    public float attractionForce = 10f; // �������Ĵ�С
    public float pickupRange = 2f; // ʰȡ��Χ
    public LayerMask itemLayer; // ��Ʒ���ڵ�Layer

    private List<Rigidbody> attractedItems = new List<Rigidbody>();
    private List<Rigidbody> disabledRigidbodies = new List<Rigidbody>();

    private void Update()
    {
        // �鿴Inventory�Ƿ�����
        if (stats.FLAG_isInventoryFull) return;

        // ѭ��������Ʒ�б��е���Ʒ
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

            // ������Һ���Ʒ֮��ķ���
            Vector3 direction = attractedItems[i].transform.position - transform.position;

            // �����Ʒ��ʰȡ��Χ�ڣ��������������
            if (direction.magnitude <= pickupRange)
            {
                if (!disabledRigidbodies.Contains(attractedItems[i]))
                {
                    attractedItems[i].isKinematic = true;
                    disabledRigidbodies.Add(attractedItems[i]);
                }

                attractedItems[i].transform.position = Vector3.MoveTowards(attractedItems[i].transform.position, transform.position, Time.deltaTime * attractionForce);

                // �����Ʒ�Ѿ��ǳ�������ִ��ʰȡ����
                if (direction.magnitude < 0.5f)
                {
                    PickupItem(attractedItems[i].gameObject);
                    attractedItems.RemoveAt(i);
                }
            }
            else
            {
                // �����Ʒ����ʰȡ��Χ���ָ�Rigidbody
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
        // ʹ��LayerMask�����Ʒ�Ƿ���ָ����Layer��
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
        // ʹ��LayerMask�����Ʒ�Ƿ���ָ����Layer��
        if (itemLayer == (itemLayer | (1 << other.gameObject.layer)))
        {
            Rigidbody itemRigidbody = other.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                attractedItems.Remove(itemRigidbody);

                // �����Ʒ����ʰȡ��Χ���ָ�Rigidbody
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

        // ������ִ��ʰȡ��Ʒ�Ĳ�����������ӵ���ҵĹ�����/�����У�����Ը����Լ�����Ϸ�߼���ʵ��
        Debug.Log("ʰȡ����Ʒ: " + itemWorldObject.name);

        inventoryController.AddItemToInventory(itemWorld.itemData, amount);

        Destroy(itemWorldObject); // Ϊ��ʾ��������򵥵���������Ʒ������Ը�����������޸�
    }
}

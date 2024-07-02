using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test : MonoBehaviour
{
    public TextMeshProUGUI testTimerText;

    public Pathfinding pathfinding;
    public MobWorldController enemy;

    // Terrain Test
    /*public TerrainManager terrainManager;
    public int x;
    public int y;
    public int z;
    public enum State{ 
        Load,
        Unload
    }
    public State loadingState;

    public void testTerrain()
    {
        switch (loadingState)
        {
            case State.Load:
                terrainManager.LoadChunk(new Vector3Int(x, y, z)); break;
            case State.Unload:
                terrainManager.UnloadChunk(new Vector3Int(x, y, z)); break;
        }
    }*/



    // Inventory Test
    public PlayerInventoryController inventoryController;

    public int slotIndex_X;
    public int slotIndex_y;

    public int count;

    /*private void Start()
    {
        int testHashCode = "Wooden PlankNoneNoneWooden PlankNoneNoneNoneNoneNone".GetHashCode();
        print(ResourceAssets.singleton.craftingRecipes[testHashCode]);
    }*/

    private void Update()
    {
        // 获取 TimeManager 单例
        TimeManager timeManager = TimeManager.singleton;

        // 获取当前时间并更新 TextMeshProUGUI
        if (timeManager != null)
        {
            int minutes = timeManager.minutes;
            int hours = timeManager.hours;
            int days = timeManager.days;

            // 格式化时间并更新 TextMeshProUGUI
            testTimerText.text = "Time: " + days.ToString("D2") + ":" + hours.ToString("D2") + ":" + minutes.ToString("D2");
        }
    }

    public void AddItem()
    {
        Debug.Log("Added Item: " + inventoryController.AddItemToInventory(ResourceAssets.singleton.items[Item_Block_TestBlock.refID].CreateItemData()));
    }

    public void AddItem_Tool()
    {
        Debug.Log("Added Item_Tool: " + inventoryController.AddItemToInventory(ResourceAssets.singleton.items[Item_Tool_TestTool.refID].CreateItemData()));
    }

    public void AddItem_Weapon()
    {
        Debug.Log("Added Item_Weapon: " + inventoryController.AddItemToInventory(ResourceAssets.singleton.items[Item_Weapon_TestWeapon.refID].CreateItemData()));
    }

    public void RemoveItem()
    {
        Debug.Log("Removed Item");
        inventoryController.RemoveItemFromInventory(ResourceAssets.singleton.items[Item_Block_TestBlock.refID].CreateItemData(), false);
    }

    public void FindPath()
    {
        //var enemyPos = pathfinding.gameObject.GetComponent<Enemy_Test>().worldCoord;
        //enemy.path = pathfinding.FindPath(enemyPos, enemyPos + new Vector3Int(7, 0, 7), 8, 1);
    }

    public void SpawnAIsNearPlayer()
    {
        WorldObjectSpawner.singleton.SpawnMobsNearPlayer();
    }

    public void MoveItem_InToolBar()
    {
        inventoryController.MoveItemInInventory(slotIndex_X, slotIndex_y, count);
    }

    public void MoveItem_InBagPack()
    {
        inventoryController.MoveItemInInventory(slotIndex_X, slotIndex_y, count);
    }

    public void MoveItem_ToolBarToBagPack()
    {
        inventoryController.MoveItemInInventory(slotIndex_X, slotIndex_y, count);
    }

    public void MoveItem_BagPackToToolBar()
    {
        inventoryController.MoveItemInInventory(slotIndex_X, slotIndex_y, count);
    }
}

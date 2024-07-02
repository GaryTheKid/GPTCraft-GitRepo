using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_FurnanceSource : UI_InventoryBase, IUIPage
{
    public GameObject backpackCanvas;
    public GameObject furnanceCanvas;
    public GameObject furnanceCanvas_Background;
    
    public Transform furnanceSourceFrameParent;

    private bool isPageActive;

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_FurnanceSource.SIZE;
        int slotsIDHead = Inventory_FurnanceSource.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(furnanceSourceFrameParent, slotsSize, slotsIDHead);
    }

    public bool IsPageActive()
    {
        return isPageActive;
    }

    public void PageOn()
    {
        isPageActive = true;
        backpackCanvas.SetActive(true);
        furnanceCanvas.SetActive(true);
        furnanceCanvas_Background.SetActive(true);
    }

    public void PageOff()
    {
        isPageActive = false;
        backpackCanvas.SetActive(false);
        furnanceCanvas.SetActive(false);
        furnanceCanvas_Background.SetActive(false);
    }

    public InventoryPageState GetPageState()
    {
        return InventoryPageState.Furnance;
    }
}
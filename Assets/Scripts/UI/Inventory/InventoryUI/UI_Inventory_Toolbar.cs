using UnityEngine;

public class UI_Inventory_Toolbar : UI_InventoryBase
{
    public Transform toolbarFrameParent;
    private RectTransform[] toolbarEquipmentAnchors;
    public RectTransform[] ToolbarEquipmentAnchors 
    {
        get { return toolbarEquipmentAnchors; }
    }

    public override void InitializeSlots()
    {
        int slotsSize = Inventory_Toolbar.SIZE;
        int slotsIDHead = Inventory_Toolbar.ID_HEAD;

        // run common initialization
        InitializeSlots_Common(toolbarFrameParent, slotsSize, slotsIDHead);

        // run custom initialization
        toolbarEquipmentAnchors = new RectTransform[slotsSize];

        for (int i = 0; i < slotsSize; i++)
        {
            var frame = toolbarFrameParent.GetChild(i);
            toolbarEquipmentAnchors[i] = frame.GetComponent<RectTransform>();
        }
    }
}

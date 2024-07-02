using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UI_InventoryBase : MonoBehaviour
{
    // Common fields for all inventories
    protected Inventory inv;
    public Inventory Inventory { get { return inv; } }

    protected InventorySlot[] slots;
    public InventorySlot[] Slots{ get { return slots; } }

    protected Image[] slotIcons;
    protected TextMeshProUGUI[] stackCountTexts;
    protected UI_ItemInteractor[] itemInteractors;
    protected Image[] durabilityBars;
    protected int IDHead;

    #region Initialization
    protected void InitializeSlots_Common(Transform frameParent, int size, int idHead)
    {
        slotIcons = new Image[size];
        stackCountTexts = new TextMeshProUGUI[size];
        itemInteractors = new UI_ItemInteractor[size];
        durabilityBars = new Image[size];
        IDHead = idHead;

        for (int i = 0; i < size; i++)
        {
            var frame = frameParent.GetChild(i);
            slotIcons[i] = frame.Find("Slot").GetComponent<Image>();
            durabilityBars[i] = slotIcons[i].transform.Find("DurabilityBar").GetComponent<Image>();
            stackCountTexts[i] = slotIcons[i].transform.Find("StackCountText").GetComponent<TextMeshProUGUI>();
            itemInteractors[i] = slotIcons[i].transform.Find("TooltipActivation").GetComponent<UI_ItemInteractor>();

            // assign slot ids
            itemInteractors[i].id = i + IDHead;
        }
    }
    public abstract void InitializeSlots();
    public virtual void SetInventory(Inventory inv)
    {
        this.inv = inv;
        slots = inv.GetInventorySlots;
    }
    #endregion

    #region Events Binding
    public void BindEventsForSlots(UI_ItemInteractor.DraggingEndedEventHandler TryMoveItems,
        UI_ItemInteractor.RightClickDuringDraggingEventHandler TryMoveOneItem,
        UI_ItemInteractor.RightClickEventHandler RemoveAllAtIndex,
        UI_ItemInteractor.LeftClickEventHandler RemoveOneCraftingAreaRecipe,
        UI_ItemInteractor.LeftClickEventHandler InterruptFurnanceProduction,
        UI_ItemInteractor.DraggingEndFailEventHandler HandleDragFailEvent)
    {
        for (int i = 0; i < itemInteractors.Length; i++)
        {
            BindEventsForSlot(itemInteractors[i], TryMoveItems, TryMoveOneItem, RemoveAllAtIndex, RemoveOneCraftingAreaRecipe, InterruptFurnanceProduction, HandleDragFailEvent);
        }
    }
    protected void BindEventsForSlot(UI_ItemInteractor slotsItemInteractor,
        UI_ItemInteractor.DraggingEndedEventHandler TryMoveItems,
        UI_ItemInteractor.RightClickDuringDraggingEventHandler TryMoveOneItem,
        UI_ItemInteractor.RightClickEventHandler RemoveAllAtIndex,
        UI_ItemInteractor.LeftClickEventHandler RemoveOneCraftingAreaRecipe,
        UI_ItemInteractor.LeftClickEventHandler InterruptFurnanceProduction,
        UI_ItemInteractor.DraggingEndFailEventHandler HandleDragFailEvent)
    {
        slotsItemInteractor.DraggingEndedEvent += TryMoveItems;
        slotsItemInteractor.RightClickDuringDraggingEvent += TryMoveOneItem;
        slotsItemInteractor.RightClickEvent += RemoveAllAtIndex;
        slotsItemInteractor.LeftClickEvent += RemoveOneCraftingAreaRecipe;
        slotsItemInteractor.LeftClickEvent += InterruptFurnanceProduction;
        slotsItemInteractor.DraggingEndFailEvent += HandleDragFailEvent;
        slotsItemInteractor.isActive = true;
    }
    public void UnBindEventsForSlots(UI_ItemInteractor.DraggingEndedEventHandler TryMoveItems,
        UI_ItemInteractor.RightClickDuringDraggingEventHandler TryMoveOneItem,
        UI_ItemInteractor.RightClickEventHandler RemoveAllAtIndex,
        UI_ItemInteractor.LeftClickEventHandler RemoveOneCraftingAreaRecipe,
        UI_ItemInteractor.LeftClickEventHandler InterruptFurnanceProduction,
        UI_ItemInteractor.DraggingEndFailEventHandler HandleDragFailEvent)
    {
        for (int i = 0; i < itemInteractors.Length; i++)
        {
            UnBindEventsForSlot(itemInteractors[i], TryMoveItems, TryMoveOneItem, RemoveAllAtIndex, RemoveOneCraftingAreaRecipe, InterruptFurnanceProduction, HandleDragFailEvent);
        }
    }
    protected void UnBindEventsForSlot(UI_ItemInteractor slotsItemInteractor, 
        UI_ItemInteractor.DraggingEndedEventHandler TryMoveItems,
        UI_ItemInteractor.RightClickDuringDraggingEventHandler TryMoveOneItem,
        UI_ItemInteractor.RightClickEventHandler RemoveAllAtIndex,
        UI_ItemInteractor.LeftClickEventHandler RemoveOneCraftingAreaRecipe,
        UI_ItemInteractor.LeftClickEventHandler InterruptFurnanceProduction,
        UI_ItemInteractor.DraggingEndFailEventHandler HandleDragFailEvent)
    {
        slotsItemInteractor.DraggingEndedEvent -= TryMoveItems;
        slotsItemInteractor.RightClickDuringDraggingEvent -= TryMoveOneItem;
        slotsItemInteractor.RightClickEvent -= RemoveAllAtIndex;
        slotsItemInteractor.LeftClickEvent -= RemoveOneCraftingAreaRecipe;
        slotsItemInteractor.LeftClickEvent -= InterruptFurnanceProduction;
        slotsItemInteractor.DraggingEndFailEvent -= HandleDragFailEvent;
        slotsItemInteractor.isActive = false;
    }
    #endregion

    #region UIs Update
    public void UpdateSlotsUI(Action<int> RemoveOneItemAtIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            UpdateSlotUI(i + IDHead, slots[i], slotIcons[i], stackCountTexts[i], itemInteractors[i], durabilityBars[i], RemoveOneItemAtIndex);
        }
    }
    public void ResetAllInteractorHighlights()
    {
        foreach (UI_ItemInteractor interator in itemInteractors)
        {
            interator.ResetHightlight();
        }
    }
    protected void UpdateSlotUI(int slotID, InventorySlot slot, Image slotIcon, TextMeshProUGUI stackCountText, UI_ItemInteractor itemInteractor, Image durabilityBar, Action<int> RemoveOneItemAtIndex)
    {
        if (slot != null)
        {
            Item item = ResourceAssets.singleton.items[slot.itemData.id];
            slotIcon.sprite = item.itemIcon;

            // ���¶ѵ������ı�
            if (slot.stackCount > 1)
            {
                stackCountText.text = slot.stackCount.ToString();
            }
            else
            {
                stackCountText.text = "";
            }

            // ����tooltip���ı�
            if (itemInteractor != null)
            {
                itemInteractor.tooltipText.text = item.itemName;
            }

            // �������Ƿ�ʵ����IDurable�ӿ�
            UpdateDurabilityBar(slotID, durabilityBar, slot.itemData, item as IDurable, RemoveOneItemAtIndex);
        }
        else // clear slot
        {
            slotIcon.sprite = ResourceAssets.singleton.ui_emptySlot;

            // ��նѵ������ı�
            stackCountText.text = "";

            // ���tooltip���ı�
            if (itemInteractor != null)
            {
                itemInteractor.tooltipText.text = "";
            }

            // ����;ö�UI
            durabilityBar.fillAmount = 0;
        }
    }
    protected void UpdateDurabilityBar(int index, Image durabilityBar, ItemData itemData, IDurable itemWithDurability, Action<int> RemoveOneItemAtIndex)
    {
        if (itemWithDurability != null)
        {
            short currentDurability = itemData.durability;
            short maxDurability = itemWithDurability.GetMaxDurability();

            // ������ߵ��;ö�С�ڵ���0�����߲���IDurable�ӿڵ�ʵ�֣������;ö�UIΪ0
            if (currentDurability <= 0)
            {
                durabilityBar.fillAmount = 0;

                RemoveOneItemAtIndex(index);
            }
            else
            {
                // �����;öȰٷֱȣ��������;ö�UI��fillAmount
                float durabilityPercentage = (float)currentDurability / maxDurability;
                durabilityBar.fillAmount = durabilityPercentage;
            }
        }
        else
        {
            // ������߲���IDurable��ʵ�֣������;ö�UIΪ0
            durabilityBar.fillAmount = 0;
        }
    }
    #endregion
}

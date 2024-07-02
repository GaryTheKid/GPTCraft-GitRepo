using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public enum ItemInteractorState
{
    Idle,
    Hover,
    Dragging
}

public class UI_ItemInteractor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void DraggingEndedEventHandler(int sourceId, int itemInteractorId);
    public event DraggingEndedEventHandler DraggingEndedEvent;

    public delegate void DraggingEndFailEventHandler(int index);
    public event DraggingEndFailEventHandler DraggingEndFailEvent;

    public delegate int RightClickDuringDraggingEventHandler (int sourceId, int itemInteractorId);
    public event RightClickDuringDraggingEventHandler RightClickDuringDraggingEvent;

    public delegate void RightClickEventHandler(int index);
    public event RightClickEventHandler RightClickEvent;

    public delegate void LeftClickEventHandler(int index);
    public event LeftClickEventHandler LeftClickEvent;

    public int id;

    public GameObject highlight;
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    public bool isActive;
    private bool isMouseOver;
    private bool isTooltipVisible;

    private float hoverTime = 3f;
    private float hoverTimer;

    private Vector3 originalAnchoredPosition; // 储存起始位置
    private Vector3 offset = Vector3.zero;

    private Transform parentTrans;

    private ItemInteractorState currentState = ItemInteractorState.Idle;

    private void Start()
    {
        parentTrans = transform.parent;
        originalAnchoredPosition = parentTrans.GetComponent<RectTransform>().anchoredPosition; // 保存起始位置
        tooltipPanel.SetActive(false);
        isTooltipVisible = false;
    }

    private void Update()
    {
        switch (currentState)
        {
            case ItemInteractorState.Idle:
                {
                    // 处理静止状态的逻辑
                    if (isMouseOver)
                    {
                        hoverTimer += Time.deltaTime;
                        if (hoverTimer >= hoverTime && !isTooltipVisible)
                        {
                            ShowTooltip();
                            currentState = ItemInteractorState.Hover;
                        }
                    }
                    else
                    {
                        hoverTimer = 0f;
                    }

                    if (Input.GetMouseButtonDown(0) && isMouseOver)
                    {
                        // 鼠标按下左键时检测是否为合成结果
                        OnLeftClick();

                        // 鼠标左键按下时开始拖拽
                        offset = parentTrans.position - Input.mousePosition;

                        // 添加Canvas，设置最高Sorting Order
                        if (parentTrans.GetComponent<Canvas>() == null)
                        {
                            var subCanvas = parentTrans.gameObject.AddComponent<Canvas>();
                            subCanvas.overrideSorting = true;
                            subCanvas.sortingOrder = 999;
                        }

                        currentState = ItemInteractorState.Dragging;
                    }

                    if (!Input.GetMouseButton(0) && Input.GetMouseButtonDown(1) && isMouseOver)
                    {
                        // idle时右击
                        OnRightClick();
                    }
                }
                break;

            case ItemInteractorState.Hover:
                {
                    // 处理悬停状态的逻辑
                    if (!isMouseOver || Input.GetMouseButtonDown(0))
                    {
                        HideTooltip();
                        currentState = ItemInteractorState.Idle;
                    }
                }
                break;

            case ItemInteractorState.Dragging:
                {
                    // 处理拖拽状态的逻辑
                    OnDuringDragging();

                    // 获取鼠标下方的UI元素
                    UI_ItemInteractor itemUnderMouse = GetUIItemUnderMouse();

                    // 拖拽中的事件, 如果源为0，立刻停止
                    if (OnRightClickDuringDragging(itemUnderMouse) == 0)
                    {
                        currentState = ItemInteractorState.Idle;
                        ResetDragging();
                        break;
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        currentState = ItemInteractorState.Idle;
                        OnDraggingEnded(itemUnderMouse);
                    }
                }
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnableHighlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHightlight();
    }

    public void EnableHighlight()
    {
        isMouseOver = true;
        highlight.SetActive(true);
    }

    public void ResetHightlight()
    {
        isMouseOver = false;
        highlight.SetActive(false);
    }

    private void ShowTooltip()
    {
        // 在这里更新物品信息的内容
        // 可以通过检测当前悬停的Slot获取物品信息，并设置tooltipText的文本内容
        // 例如：tooltipText.text = currentSlot.item.GetItemInfo();

        // 显示物品信息
        tooltipPanel.SetActive(true);
        isTooltipVisible = true;
    }

    private void HideTooltip()
    {
        // 隐藏物品信息
        tooltipPanel.SetActive(false);
        isTooltipVisible = false;
    }

    private void OnLeftClick()
    {
        LeftClickEvent(id);
    }

    private void OnRightClick()
    {
        RightClickEvent(id);
    }

    private void OnDuringDragging()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.z;
        parentTrans.position = mousePos + offset;
    }

    private int OnRightClickDuringDragging(UI_ItemInteractor itemUnderMouse)
    {
        if (Input.GetMouseButtonDown(1) && itemUnderMouse != null)
        {
            // 触发右击事件
            return RightClickDuringDraggingEvent(id, itemUnderMouse.id);
        }

        return -1;
    }

    private void OnDraggingEnded(UI_ItemInteractor itemUnderMouse)
    {
        // 如果鼠标下方的UI元素为null，将正在拖拽的UI复原
        if (itemUnderMouse != null)
        {
            // 触发物品移动事件
            DraggingEndedEvent(id, itemUnderMouse.id);
        }
        else
        {
            DraggingEndFailEvent(id);
        }

        ResetDragging();
    }

    private void ResetDragging()
    {
        // 将UI的位置重置为原始位置
        parentTrans.GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;

        // 卸载Canvas
        if (parentTrans.GetComponent<Canvas>() != null)
        {
            Destroy(parentTrans.gameObject.GetComponent<Canvas>());
        }
    }

    private UI_ItemInteractor GetUIItemUnderMouse()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            UI_ItemInteractor item = result.gameObject.GetComponent<UI_ItemInteractor>();
            if (item != null && item != this && item.isActive)
            {
                return item;
            }
        }

        return null;
    }
}

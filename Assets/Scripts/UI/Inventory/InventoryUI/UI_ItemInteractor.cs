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

    private Vector3 originalAnchoredPosition; // ������ʼλ��
    private Vector3 offset = Vector3.zero;

    private Transform parentTrans;

    private ItemInteractorState currentState = ItemInteractorState.Idle;

    private void Start()
    {
        parentTrans = transform.parent;
        originalAnchoredPosition = parentTrans.GetComponent<RectTransform>().anchoredPosition; // ������ʼλ��
        tooltipPanel.SetActive(false);
        isTooltipVisible = false;
    }

    private void Update()
    {
        switch (currentState)
        {
            case ItemInteractorState.Idle:
                {
                    // ����ֹ״̬���߼�
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
                        // ��갴�����ʱ����Ƿ�Ϊ�ϳɽ��
                        OnLeftClick();

                        // ����������ʱ��ʼ��ק
                        offset = parentTrans.position - Input.mousePosition;

                        // ���Canvas���������Sorting Order
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
                        // idleʱ�һ�
                        OnRightClick();
                    }
                }
                break;

            case ItemInteractorState.Hover:
                {
                    // ������ͣ״̬���߼�
                    if (!isMouseOver || Input.GetMouseButtonDown(0))
                    {
                        HideTooltip();
                        currentState = ItemInteractorState.Idle;
                    }
                }
                break;

            case ItemInteractorState.Dragging:
                {
                    // ������ק״̬���߼�
                    OnDuringDragging();

                    // ��ȡ����·���UIԪ��
                    UI_ItemInteractor itemUnderMouse = GetUIItemUnderMouse();

                    // ��ק�е��¼�, ���ԴΪ0������ֹͣ
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
        // �����������Ʒ��Ϣ������
        // ����ͨ����⵱ǰ��ͣ��Slot��ȡ��Ʒ��Ϣ��������tooltipText���ı�����
        // ���磺tooltipText.text = currentSlot.item.GetItemInfo();

        // ��ʾ��Ʒ��Ϣ
        tooltipPanel.SetActive(true);
        isTooltipVisible = true;
    }

    private void HideTooltip()
    {
        // ������Ʒ��Ϣ
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
            // �����һ��¼�
            return RightClickDuringDraggingEvent(id, itemUnderMouse.id);
        }

        return -1;
    }

    private void OnDraggingEnded(UI_ItemInteractor itemUnderMouse)
    {
        // �������·���UIԪ��Ϊnull����������ק��UI��ԭ
        if (itemUnderMouse != null)
        {
            // ������Ʒ�ƶ��¼�
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
        // ��UI��λ������Ϊԭʼλ��
        parentTrans.GetComponent<RectTransform>().anchoredPosition = originalAnchoredPosition;

        // ж��Canvas
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

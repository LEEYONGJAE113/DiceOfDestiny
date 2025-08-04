using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableSticker : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    bool isDragging = false;

    ClassSticker classSticker;

    private Transform originalParent;
    RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void Initialize()
    {

    }

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isDragging = true;
        gameObject.GetComponent<Image>().raycastTarget = false;

        originalParent = transform.parent;

        RectTransform parentRectTransform = transform.parent.GetComponent<RectTransform>();

        Canvas rootCanvas = GetComponentInParent<Canvas>();
        transform.SetParent(rootCanvas.transform, false);
        rectTransform.anchoredPosition += parentRectTransform.anchoredPosition;
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        isDragging = false;

        GameObject target = eventData.pointerEnter;
        if (target == null)
        {
            ReturnToOriginalPosition();
            return;
        }

        var stickerFace = target.GetComponentInParent<StickerFace>();
        if (stickerFace != null)
        {
            HandleDropOnEmptyFace(stickerFace);
            return;
        }

        var stickerDrawer = target.GetComponentInParent<StickerDrawer>();
        if (stickerDrawer != null)
        {
            HandleDropInDrawer(stickerDrawer);
            return;
        }

        ReturnToOriginalPosition();
    }

    public void HandleDropOnEmptyFace(StickerFace stickerFace)
    {
        if(stickerFace.draggableSticker == null)
        {

        }
        else
        {
            Debug.Log("스티커가 이미 존재하는 얼굴에 드래그했습니다. 원래 위치로 되돌립니다.");
            ReturnToOriginalPosition();
            return;
        }

    }

    private void HandleDropInDrawer(StickerDrawer stickerDrawer)
    {
        Debug.Log("드래그한 스티커를 스티커 서랍에 넣었습니다.");
        InventoryManager.Instance.AddSticker(classSticker);
        Destroy(gameObject);

    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent, false);
        rectTransform.anchoredPosition = Vector2.zero;
        Debug.Log("드래그한 스티커를 원래 위치로 되돌렸습니다.");
        gameObject.GetComponent<Image>().raycastTarget = true;
    }


}


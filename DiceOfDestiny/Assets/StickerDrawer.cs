using UnityEngine;
using UnityEngine.EventSystems;


public class StickerDrawer : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dragged = eventData.pointerDrag;

        Debug.Log("StickerDrawer OnDrop: " + dragged.name);
    }
}

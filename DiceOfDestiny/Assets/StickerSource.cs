using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerSource : MonoBehaviour, IPointerDownHandler
{
    public Image stickerSprite;

    void Start()
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Sticker Source MouseDown: " + stickerSprite.sprite.name);
    }
}

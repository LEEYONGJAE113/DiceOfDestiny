using UnityEngine;

public class StickerFace : MonoBehaviour
{
    [HideInInspector] public DraggableSticker draggableSticker;

    private void Start()
    {
        
    }

    public void Initialize(DraggableSticker sticker = null)
    {
        draggableSticker = sticker;
    }
}

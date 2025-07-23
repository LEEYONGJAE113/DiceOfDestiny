using UnityEngine;

public class DiceCustomizeManager : MonoBehaviour
{
    [SerializeField] private GameObject pieceCarouselUI;
    [SerializeField] private GameObject pieceNetCarouselUI;


    public void OnClickPieceCaruselUIButton()
    {
        if (pieceCarouselUI.activeSelf == false)
        {
            pieceNetCarouselUI.SetActive(false);
            pieceCarouselUI.SetActive(true);
        }
    }

    public void OnClickPieceNetCaruselUIButton()
    {
        if (pieceNetCarouselUI.activeSelf == false)
        {
            pieceCarouselUI.SetActive(false);
            pieceNetCarouselUI.SetActive(true);
        }
    }
}

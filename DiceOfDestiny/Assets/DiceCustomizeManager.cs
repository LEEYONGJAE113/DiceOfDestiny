using UnityEngine;

public class DiceCustomizeManager : MonoBehaviour
{
    [SerializeField] private GameObject cutomizePanel;
    [SerializeField] private GameObject carouselUIPanel;

    [SerializeField] private GameObject pieceCarouselUI;
    [SerializeField] private GameObject pieceNetCarouselUI;

    [SerializeField] private GameObject piecesContent;
    [SerializeField] private GameObject piecePreviewButtonPrefab;


    public void InitializePiecesCaruselUI()
    {

    }


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

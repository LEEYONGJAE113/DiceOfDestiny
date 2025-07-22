using UnityEngine;

public class PieceListButton : MonoBehaviour
{
    [SerializeField] private GameObject pieceListUI;

    public void OnClick()
    {
        pieceListUI.SetActive(!pieceListUI.activeSelf);
    }
}

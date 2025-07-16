using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceListUI : MonoBehaviour
{
    private List<PieceController> pieces;
    [SerializeField] private GameObject buttonPrefab;

    void Start()
    {
        pieces = PieceManager_LYJ.Instance.Pieces;
    }

    void OnEnable() // 나중에 최적화
    {
        for (int i = 0; i < pieces.Count; ++i)
        {
            Button piece = Instantiate(buttonPrefab).GetComponent<Button>();
            Image pieceTopImage = piece.gameObject.GetComponent<Image>();
            pieceTopImage.sprite = pieces[i].GetTopFace().classData.sprite;
            if (pieces[i].IsinGame)
            {
                pieceTopImage.color = new Color(pieceTopImage.color.r, pieceTopImage.color.g, pieceTopImage.color.b, 0.3f);
            }
            piece.onClick.AddListener(() => PieceManager_LYJ.Instance.AddPiece(pieces[i]));
        }
    }
}

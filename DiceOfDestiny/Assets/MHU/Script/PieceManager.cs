using UnityEngine;

public class PieceManager : Singletone<PieceManager>
{
    [SerializeField] private Piece piece; // 관리할 Piece 데이터
    [SerializeField] private SpriteRenderer topClassRenderer; // 윗면 직업 렌더러
    [SerializeField] private SpriteRenderer topColorRenderer; // 윗면 색상 렌더러

    private void Awake()
    {
        // // 싱글톤 설정
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        // Piece 초기화
        if (piece == null)
            piece = new Piece();
    }

    public Piece GetPiece()
    {
        return piece;
    }

    public void SetPiece(Piece newPiece)
    {
        piece = newPiece;
        UpdateAllFacesVisual();
    }

    public void UpdateAllFacesVisual()
    {
        if (piece == null) return;
        int topFaceIndex = piece.GetTopFaceIndex();
        Face topFace = piece.GetFace(topFaceIndex);
        if (topClassRenderer != null /*&& topFace.tileColor != null*/ && topFace.classData != null)
        {
            topClassRenderer.sprite = topFace.classData.sprite;
            // topColorRenderer.color = topFace.tileColor.color;
            switch (topFace.tileColor) // temp
            {
                case TileColor.Red:
                    topColorRenderer.color = Color.red;
                    break;
                case TileColor.Blue:
                    topColorRenderer.color = Color.blue;
                    break;
                case TileColor.Yellow:
                    topColorRenderer.color = Color.yellow;
                    break;
                case TileColor.Green:
                    topColorRenderer.color = Color.green;
                    break;
                case TileColor.Gray:
                    topColorRenderer.color = Color.gray;
                    break;
                case TileColor.Purple:
                    topColorRenderer.color = Color.magenta;
                    break;
            }
        }
    }

    public void ChangeFaceColor(int faceIndex, TileColor newColorData)
    {
        if (piece != null)
        {
            Face face = piece.GetFace(faceIndex);
            piece.SetFace(faceIndex, face.classData, newColorData);
            UpdateAllFacesVisual();
        }
    }
}
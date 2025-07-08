using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
<<<<<<< Updated upstream
    public static PieceManager Instance { get; private set; }

    [SerializeField] private Piece piece; // 관리할 Piece 데이터
    [SerializeField] private SpriteRenderer topClassRenderer; // 윗면 직업 렌더러
    [SerializeField] private SpriteRenderer topColorRenderer; // 윗면 색상 렌더러

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        if (topClassRenderer != null && topFace.colorData != null && topFace.classData != null)
        {
            topClassRenderer.sprite = topFace.classData.sprite;
            topColorRenderer.color = topFace.colorData.color;
        }
    }

    public void ChangeFaceColor(int faceIndex, ColorData newColorData)
    {
        if (piece != null)
        {
            Face face = piece.GetFace(faceIndex);
            piece.SetFace(faceIndex, face.classData, newColorData);
            UpdateAllFacesVisual();
        }
    }
=======
    List<PieceController> pieces = new List<PieceController>();

    public GameObject piecePrefab;

>>>>>>> Stashed changes
}
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singletone<InventoryManager>
{
    public List<Piece> pieces = new List<Piece>();
    public List<PieceNet> pieceNets = new List<PieceNet>();
    public List<ClassSticker> classStickers = new List<ClassSticker>();

    private void Awake()
    {
        TestInitialize();
        Debug.Log(pieceNets.Count);
    }

    void TestInitialize()
    {
        // 테스트용 초기화
        Debug.Log("InventoryManager 초기화");

        PieceNet testPieceNet = new PieceNet();
        testPieceNet.faces[0].color = TileColor.Red;
        testPieceNet.faces[1].color = TileColor.Blue;
        testPieceNet.faces[2].color = TileColor.Green;
        testPieceNet.faces[3].color = TileColor.Yellow;
        testPieceNet.faces[4].color = TileColor.Yellow;
        testPieceNet.faces[5].color = TileColor.Purple;
        AddPieceNet(testPieceNet);

        testPieceNet = new PieceNet();
        testPieceNet.faces[0].color = TileColor.Red;
        testPieceNet.faces[1].color = TileColor.Red;
        testPieceNet.faces[2].color = TileColor.Green;
        testPieceNet.faces[3].color = TileColor.Green;
        testPieceNet.faces[4].color = TileColor.Blue;
        testPieceNet.faces[5].color = TileColor.Blue;
        AddPieceNet(testPieceNet);

        testPieceNet = new PieceNet();
        testPieceNet.faces[0].color = TileColor.Red;
        testPieceNet.faces[1].color = TileColor.Blue;
        testPieceNet.faces[2].color = TileColor.Purple;
        testPieceNet.faces[3].color = TileColor.Yellow;
        testPieceNet.faces[4].color = TileColor.Blue;
        testPieceNet.faces[5].color = TileColor.Purple;
        AddPieceNet(testPieceNet);

        testPieceNet = new PieceNet();
        testPieceNet.faces[0].color = TileColor.Blue;
        testPieceNet.faces[1].color = TileColor.Blue;
        testPieceNet.faces[2].color = TileColor.Green;
        testPieceNet.faces[3].color = TileColor.Yellow;
        testPieceNet.faces[4].color = TileColor.Blue;
        testPieceNet.faces[5].color = TileColor.Red;
        AddPieceNet(testPieceNet);

    }

    public void AddPiece(Piece piece)
    {
        if (!pieces.Contains(piece))
        {
            pieces.Add(piece);
            Debug.Log($"Piece 추가: {piece.name}");
        }
        else
        {
            Debug.LogWarning($"이미 존재하는 Piece입니다: {piece.name}");
        }
    }

    public bool RemovePiece(Piece piece)
    {
        bool removed = pieces.Remove(piece);
        if (removed)
            Debug.Log($"Piece 제거: {piece.name}");
        else
            Debug.LogWarning($"제거 실패 - Piece를 찾을 수 없음: {piece.name}");

        return removed;
    }

    public List<Piece> GetPieces()
    {
        return new List<Piece>(pieces);
    }

    public void AddPieceNet(PieceNet pieceNet)
    {
        pieceNets.Add(pieceNet);
    }

    public bool RemovePieceNet(PieceNet pieceNet)
    {
        bool removed = pieceNets.Remove(pieceNet);
        if (removed)
            Debug.Log($"PieceNet 제거: {pieceNet}");
        else
            Debug.LogWarning($"제거 실패 - PieceNet을 찾을 수 없음: {pieceNet}");

        return removed;
    }

    public List<PieceNet> GetPieceNets()
    {
        return new List<PieceNet>(pieceNets);
    }

    public void AddSticker(ClassSticker sticker)
    {
        if (!classStickers.Contains(sticker))
        {
            classStickers.Add(sticker);
            Debug.Log($"Sticker 추가: {sticker.name}");
        }
        else
        {
            Debug.LogWarning($"이미 존재하는 Sticker입니다: {sticker.name}");
        }
    }

    public bool RemoveSticker(ClassSticker sticker)
    {
        bool removed = classStickers.Remove(sticker);
        if (removed)
            Debug.Log($"Sticker 제거: {sticker.name}");
        else
            Debug.LogWarning($"제거 실패 - Sticker를 찾을 수 없음: {sticker.name}");

        return removed;
    }

    public List<ClassSticker> GetStickers()
    {
        return new List<ClassSticker>(classStickers);
    }

}

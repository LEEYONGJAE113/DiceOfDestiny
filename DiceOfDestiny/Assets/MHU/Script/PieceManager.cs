using System.Collections.Generic;
using UnityEngine;

public class PieceManager : Singletone<PieceManager>
{
    List<PieceController> pieces = new List<PieceController>();
    public List<PieceController> Pieces
    {
        get => pieces;
        set
        {
            pieces = value;
            EventManager.Instance.TriggerEvent(AllEventNames.PIECE_COUNT_CHANGED);
        }
    }
    private List<PieceState> pieceStates = new();
    public GameObject piecePrefab;
    [SerializeField]private PieceController currentPiece; // 현재 내가 조종중인 말

    void Awake()
    {
        UpdatePieceManagerList();
    }

    void Start()
    {
        EventManager.Instance.AddListener(AllEventNames.PIECE_COUNT_CHANGED, UpdatePieceManagerList);
    }

    public void DrawAllPieceUIs()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            PieceUIManager.Instance.CreatePieceUI(pieceStates[i].CurrentState, pieces[i].gameObject);
        }
    }

    private void UpdatePieceManagerList(object data = null)
    {
        int count = pieces.Count;
        if (pieceStates.Count < count)
        {
            PieceState newElement = new PieceState();
            newElement.ChangeState(States.Selectable);
            newElement.ChangeSelectable(true);
            pieceStates.Add(newElement);
        }
        if (pieceStates.Count > count)
        {
            pieceStates.RemoveAt(pieceStates.Count - 1);
        }
    }

    public void DecreaseDebuffAllPieces()
    {
        foreach (var piece in pieces)
        {
            if(piece.GetPiece().debuff.IsStun)
                piece.GetPiece().debuff.DecreaseStunTurn();
        }
        if (currentPiece.GetPiece().debuff.IsStun)
            currentPiece.GetPiece().debuff.DecreaseStunTurn();
    }
}
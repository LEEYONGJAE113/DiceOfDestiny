using System.Collections.Generic;
using UnityEngine;

public class PieceManager_LYJ : Singletone<PieceManager_LYJ>
{
    List<PieceController> pieces = new List<PieceController>();
    public List<PieceController> Pieces
    {
        get => pieces;
        private set
        {
            pieces = value;
            EventManager.Instance.TriggerEvent(AllEventNames.PIECE_COUNT_CHANGED);
        }
    }
    private List<PieceState> pieceStates = new();
    public GameObject piecePrefab;
    private PieceController currentPiece; // 현재 내가 조종중인 말

    void Awake()
    {
        UpdatePieceManagerLists();
    }

    void Start()
    {
        EventManager.Instance.AddListener(AllEventNames.PIECE_COUNT_CHANGED, UpdatePieceManagerLists);
    }

    public void DrawAllPieceUIs()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            PieceUIManager.Instance.CreatePieceUI(pieceStates[i].CurrentState, pieces[i].gameObject);
        }
    }

    private void UpdatePieceManagerLists(object data = null)
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

    //

    public void AddPiece(PieceController newPiece)
    {
        pieces.Add(newPiece);
        Pieces = pieces;
        // newPiece.Init();
        newPiece.SetInGame(true);
    }

    public void RemovePiece(PieceController targetPiece)
    {
        pieces.Remove(targetPiece);
        Pieces = pieces;
        targetPiece.SetInGame(false);
    }

    

}
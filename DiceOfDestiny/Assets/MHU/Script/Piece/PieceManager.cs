using System.Collections.Generic;
using UnityEngine;

// 기물 관리하는 매니저
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
    public GameObject[] piecePrefabs;
    [SerializeField] public PieceController currentPiece; // 현재 내가 조종중인 말

    public PieceInventory pieceInventory;

    void Awake()
    {
        UpdatePieceManagerList();

        pieceInventory = GetComponent<PieceInventory>();
    }

    void Start()
    {
        EventManager.Instance.AddListener(AllEventNames.PIECE_COUNT_CHANGED, UpdatePieceManagerList);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (pieces.Count <= 0 || pieces[0] == null) return;
            currentPiece = pieces[0];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (pieces.Count <= 1 || pieces[1] == null) return;
            currentPiece = pieces[1];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (pieces.Count <= 2 || pieces[2] == null) return;
            currentPiece = pieces[2];
        }
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
            piece.statusEffectController.EndTurn();
        }
    }

    public PieceController GetCurrentPiece()
    {
        return currentPiece;
    }

    public void SetCurrentPiece(PieceController pieceController)
    {
        currentPiece = pieceController;
    }

}
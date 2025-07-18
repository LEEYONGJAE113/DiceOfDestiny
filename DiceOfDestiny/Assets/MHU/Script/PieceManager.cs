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
    public GameObject[] piecePrefabs;
    [SerializeField] private PieceController currentPiece; // 현재 내가 조종중인 말

    void Awake()
    {
        UpdatePieceManagerList();
    }

    void Start()
    {
        EventManager.Instance.AddListener(AllEventNames.PIECE_COUNT_CHANGED, UpdatePieceManagerList);

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            pieces.Add(piecePrefabs[i].GetComponent<PieceController>());
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
        currentPiece.statusEffectController.EndTurn();
    }

    // public void AddDebuffPiece(ObstacleType obstacleType, PieceController pieceController) // 어떤 기물인지?, 클래스 데이터
    // {
    //     if (obstacleType == ObstacleType.PoisonousHerb)
    //     {
    //         if (pieceController.GetTopFace().classData.className == "Demon")
    //         {
    //             GameManager.Instance.actionPointManager.AddAP(1);
    //             return;
    //         }
    //         GameManager.Instance.actionPointManager.RemoveAP(1);
    //     }
    // }
}
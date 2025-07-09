using System.Collections.Generic;
using UnityEngine;

public class PieceManager : Singletone<PieceManager>
{
    List<PieceController> pieces = new List<PieceController>();
    private List<PieceState> pieceStates = new();
    public GameObject piecePrefab;
    private PieceController currentPiece; // 현재 내가 조종중인 말

    // public void TurnStart()
    // {
    //     foreach (var item in pieces)
    //     {
    //         var uiController = item.gameObject.GetComponentInChildren<PieceUIManager>();
    //         uiController?.ChangeUI(States.Selectable);
    //     }
    // }

    

}
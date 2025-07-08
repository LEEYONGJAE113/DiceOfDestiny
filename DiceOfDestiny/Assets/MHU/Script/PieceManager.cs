using System.Collections.Generic;
using UnityEngine;

public class PieceManager : Singletone<PieceManager>
{
    List<PieceController> pieces = new List<PieceController>();

    public GameObject piecePrefab;
}
    
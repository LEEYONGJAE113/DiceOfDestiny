using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class PieceInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        [SerializeField] private int pieceNumber;
        [SerializeField] private Piece piece;

        public void AddPiece(Piece _piece)
        {
            this.pieceNumber = _piece.PieceNumber;
            this.piece = _piece;
        }

        public void RemovePiece()
        {
            pieceNumber = 0;
            piece = null;
        }

        public bool IsActivePiece()
        {
            return piece != null;
        }

        public Piece GetPiece()
        {
            return piece;
        }
        public void SetPiece(Piece _piece)
        {
            this.piece = _piece;
        }
    }

    public List<Slot> slots = new List<Slot>();
    public Slot selectedSlot = null;

    private void Awake()
    {
        for (int i = 0; i < 3; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }
}

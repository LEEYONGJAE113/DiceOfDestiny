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
        [SerializeField] private Image image;

        [SerializeField] private Piece piece;

        public void AddPiece(Piece _piece)
        {
            this.pieceNumber = _piece.pieceNumber;
            this.image.sprite = _piece.sprite;
            this.piece = _piece;
        }

        public void RemovePiece()
        {
            pieceNumber = 0;
            image.sprite = null;
            piece = null;
        }

        public bool IsActivePiece()
        {
            return piece != null;
        }
    }

    public List<Slot> slots = new List<Slot>();
    public Slot selectedSlot = null;

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }
}

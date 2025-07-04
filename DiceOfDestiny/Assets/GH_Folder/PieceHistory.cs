using UnityEngine;

public class PieceHistory
{
    private Vector2 prevPos;
    private int prevJob; // Job?

    public PieceHistory(int _x, int _y, int _prevJob)
    {
        prevPos.x = _x;
        prevPos.y = _y;
        prevJob = _prevJob;
    }
}
